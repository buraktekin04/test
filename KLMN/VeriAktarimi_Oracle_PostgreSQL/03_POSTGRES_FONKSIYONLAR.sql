-- PostgreSQL 15+ üzerinde PL/pgSQL. Yalnızca tanım/eşleme staging'i için.
-- Her çalıştırmada idempotent CREATE OR REPLACE kullanılır.
CREATE OR REPLACE FUNCTION legacy_import.make_id(target_table text, legacy_id text)
RETURNS uuid LANGUAGE sql IMMUTABLE STRICT AS $$
 SELECT md5('KLMN_LEGACY_V1|' || target_table || '|' || btrim(legacy_id))::uuid
$$;

CREATE OR REPLACE FUNCTION legacy_import.source_column(p_source_table text, p_wanted text)
RETURNS text LANGUAGE sql STABLE AS $$
 SELECT c.column_name::text
 FROM information_schema.columns c
 WHERE c.table_schema='legacy_raw' AND c.table_name=p_source_table
   AND regexp_replace(lower(c.column_name),'[^a-z0-9]','','g')
       =regexp_replace(lower(p_wanted),'[^a-z0-9]','','g')
 ORDER BY c.ordinal_position LIMIT 1
$$;

CREATE OR REPLACE PROCEDURE legacy_import.auto_match_tables()
LANGUAGE plpgsql AS $$
BEGIN
 IF NOT EXISTS (SELECT 1 FROM legacy_import.oracle_columns) THEN
   RAISE EXCEPTION 'Önce Oracle oracle_columns.csv envanterini içe aktarın';
 END IF;
 WITH candidates AS (
  SELECT m.target_table, min(o.table_name) AS source_table, count(*) AS candidate_count
  FROM legacy_import.table_map m
  JOIN (SELECT DISTINCT table_name FROM legacy_import.oracle_columns) o
  ON regexp_replace(lower(o.table_name),'[^a-z0-9]','','g') IN
   (regexp_replace(lower(m.target_table),'[^a-z0-9]','','g'),
    regexp_replace(lower(regexp_replace(m.target_table,'s$','')),'[^a-z0-9]','','g'))
  WHERE m.source_table IS NULL
  GROUP BY m.target_table
 )
 UPDATE legacy_import.table_map m SET source_table=c.source_table
 FROM candidates c
 WHERE m.target_table=c.target_table AND c.candidate_count=1;
END $$;

CREATE OR REPLACE PROCEDURE legacy_import.prepare_staging()
LANGUAGE plpgsql AS $$
DECLARE x record; cols text;
BEGIN
 FOR x IN SELECT DISTINCT m.source_table
          FROM legacy_import.table_map m
          WHERE m.enabled AND m.source_table IS NOT NULL
 LOOP
   SELECT string_agg(format('%I text', z.column_name), ', ' ORDER BY z.column_id)
   INTO cols
   FROM (
    SELECT DISTINCT ON (column_name) column_name, column_id
    FROM legacy_import.oracle_columns
    WHERE table_name=x.source_table
    ORDER BY column_name, column_id
   ) z;
   IF cols IS NULL THEN
     RAISE EXCEPTION 'Oracle metadata bulunamadı: %', x.source_table;
   END IF;
   EXECUTE format('CREATE TABLE IF NOT EXISTS legacy_raw.%I (%s)', x.source_table, cols);
 END LOOP;
END $$;

-- Hedef tablonun FK'sini ve referans tablosunu migration'ın gerçek PostgreSQL
-- kataloglarından bulur. Sadece tek sütunlu FK'ler desteklenir.
CREATE OR REPLACE FUNCTION legacy_import.referenced_table(p_schema text, p_table text, p_column text)
RETURNS text LANGUAGE sql STABLE AS $$
 SELECT ref.relname::text
 FROM pg_constraint k
 JOIN pg_class src ON src.oid=k.conrelid
 JOIN pg_namespace n ON n.oid=src.relnamespace
 JOIN pg_class ref ON ref.oid=k.confrelid
 JOIN pg_attribute a ON a.attrelid=src.oid AND a.attnum=k.conkey[1]
 WHERE k.contype='f' AND array_length(k.conkey,1)=1
   AND n.nspname=p_schema AND src.relname=p_table AND a.attname=p_column
 LIMIT 1
$$;

CREATE OR REPLACE FUNCTION legacy_import.resolve_code(
 p_schema text, p_reference text, p_code text
) RETURNS uuid LANGUAGE plpgsql STABLE AS $$
DECLARE id_value uuid; row_count integer;
BEGIN
 IF p_code IS NULL OR btrim(p_code)='' THEN RETURN NULL; END IF;
 EXECUTE format('SELECT count(*) FROM %I.%I WHERE "Code"::text = $1 AND "IsDeleted"=false',
                p_schema,p_reference)
 INTO row_count USING btrim(p_code);
 IF row_count = 1 THEN
   EXECUTE format('SELECT "Id" FROM %I.%I WHERE "Code"::text = $1 AND "IsDeleted"=false',
                  p_schema,p_reference)
   INTO id_value USING btrim(p_code);
 END IF;
 IF row_count <> 1 THEN
   RAISE EXCEPTION 'FK Code bulunamadı veya tekil değil: %.% Code=% (adet=%)',
       p_schema,p_reference,p_code,row_count;
 END IF;
 RETURN id_value;
END $$;

CREATE OR REPLACE FUNCTION legacy_import.to_bool(p_value text)
RETURNS boolean LANGUAGE plpgsql IMMUTABLE AS $$
BEGIN
 IF p_value IS NULL OR btrim(p_value)='' THEN RETURN NULL; END IF;
 CASE upper(btrim(p_value))
 WHEN '1' THEN RETURN true;
 WHEN '0' THEN RETURN false;
 WHEN 'Y' THEN RETURN true;
 WHEN 'N' THEN RETURN false;
 WHEN 'E' THEN RETURN true;
 WHEN 'H' THEN RETURN false;
 WHEN 'TRUE' THEN RETURN true;
 WHEN 'FALSE' THEN RETURN false;
 WHEN 'T' THEN RETURN true;
 WHEN 'F' THEN RETURN false;
 WHEN 'YES' THEN RETURN true;
 WHEN 'NO' THEN RETURN false;
 WHEN 'EVET' THEN RETURN true;
 WHEN 'HAYIR' THEN RETURN false;
 ELSE RAISE EXCEPTION 'Bilinmeyen Oracle boolean değeri: %',p_value;
 END CASE;
END $$;

CREATE OR REPLACE FUNCTION legacy_import.to_timestamp(p_value text)
RETURNS timestamptz LANGUAGE plpgsql STABLE AS $$
DECLARE tz text;
BEGIN
 IF p_value IS NULL OR btrim(p_value)='' THEN RETURN NULL; END IF;
 SELECT value INTO tz FROM legacy_import.settings WHERE key='source_timezone';
 IF btrim(p_value) ~ '(Z|[+-][0-9]{2}:[0-9]{2})$' THEN
   RETURN btrim(p_value)::timestamptz;
 END IF;
 RETURN btrim(p_value)::timestamp AT TIME ZONE tz;
END $$;

CREATE OR REPLACE FUNCTION legacy_import.load_one(p_target text)
RETURNS bigint LANGUAGE plpgsql AS $$
DECLARE m legacy_import.table_map%ROWTYPE;
        v_schema text; v_pk text; v_source text; v_expr text;
        v_cols text := '"Id"'; v_vals text; v_missing text := '';
        v_ref text; v_mode text; v_override text; v_notnull boolean;
        v_default text; v_dtype text; v_dataudt text; v_count bigint; v_existing bigint;
        r record; q text;
BEGIN
 SELECT * INTO m FROM legacy_import.table_map WHERE target_table=p_target;
 IF NOT FOUND OR NOT m.enabled THEN RAISE EXCEPTION 'Hedef etkin değil: %',p_target; END IF;
 IF m.source_table IS NULL THEN RAISE EXCEPTION 'Oracle kaynak eşleştirmesi yok: %',p_target; END IF;
 SELECT value INTO v_schema FROM legacy_import.settings WHERE key='target_schema';
 IF to_regclass(format('%I.%I',v_schema,p_target)) IS NULL THEN
   RAISE EXCEPTION 'PostgreSQL hedef tablosu bulunamadı: %.%',v_schema,p_target;
 END IF;
 IF to_regclass(format('legacy_raw.%I',m.source_table)) IS NULL THEN
   RAISE EXCEPTION 'Önce staging oluşturup CSV yükleyin: %',m.source_table;
 END IF;
 v_pk := legacy_import.source_column(m.source_table,coalesce(m.source_pk,'ID'));
 IF v_pk IS NULL THEN
   RAISE EXCEPTION '% tablosunda kaynak tekil kimlik sütunu % bulunamadı',m.source_table,m.source_pk;
 END IF;
 EXECUTE format('SELECT count(*) FROM legacy_raw.%I',m.source_table) INTO v_count;
 IF v_count=0 THEN
   RAISE NOTICE 'Boş kaynak tablosu atlandı: %',m.source_table;
   RETURN 0;
 END IF;
 EXECUTE format('SELECT count(*) FROM %I.%I',v_schema,p_target) INTO v_existing;
 IF v_existing>0 THEN
   RAISE EXCEPTION 'Hedef boş değil: %.% (% kayıt). Otomatik overwrite/upsert yapılmaz.',
                    v_schema,p_target,v_existing;
 END IF;
 EXECUTE format('SELECT count(*) FROM (SELECT %I FROM legacy_raw.%I GROUP BY %I HAVING count(*)>1 OR %I IS NULL OR btrim(%I)='''') d',
                v_pk,m.source_table,v_pk,v_pk,v_pk)
 INTO v_existing;
 IF v_existing>0 THEN
   RAISE EXCEPTION 'Kaynak eski ID boş veya tekrarlı: %.% / %',m.source_table,v_pk,v_existing;
 END IF;
 v_vals := format('legacy_import.make_id(%L, s.%I)',p_target,v_pk);
 FOR r IN
    SELECT c.column_name,c.data_type,c.udt_name,c.is_nullable,c.column_default
    FROM information_schema.columns c
    WHERE c.table_schema=v_schema AND c.table_name=p_target
      AND c.column_name NOT IN ('Id','Version','CreatedBy','UpdatedBy','DeletedBy')
      AND c.is_generated='NEVER'
    ORDER BY c.ordinal_position
 LOOP
    v_dtype:=r.data_type; v_dataudt:=r.udt_name;
    v_notnull:=(r.is_nullable='NO'); v_default:=r.column_default;
    SELECT cm.source_column,cm.mode INTO v_override,v_mode
    FROM legacy_import.column_map cm
    WHERE cm.target_table=p_target AND cm.target_column=r.column_name;
    IF NOT FOUND THEN v_override:=NULL; v_mode:=NULL; END IF;
    IF v_mode='skip' THEN
      IF v_notnull AND v_default IS NULL THEN
        RAISE EXCEPTION 'Zorunlu kolon skip olamaz: %.%',p_target,r.column_name;
      END IF;
      CONTINUE;
    END IF;
    v_ref:=legacy_import.referenced_table(v_schema,p_target,r.column_name);
    v_source:=legacy_import.source_column(m.source_table,coalesce(v_override,r.column_name));
    IF v_source IS NULL AND v_override IS NULL AND v_ref IS NOT NULL
       AND right(r.column_name,2)='Id' THEN
      v_source:=legacy_import.source_column(m.source_table,
                      left(r.column_name,length(r.column_name)-2));
    END IF;
    IF v_override IS NOT NULL AND v_source IS NULL THEN
      RAISE EXCEPTION 'Kaynak kolon eşleştirmesi bulunamadı: %.% <- %',
                  p_target,r.column_name,v_override;
    END IF;
    -- Aynı tabloda nullable self FK, diğer satırlar eklendikten sonra bağlanacak.
    IF v_ref=p_target AND r.is_nullable='YES' THEN CONTINUE; END IF;
    IF v_source IS NULL THEN
       IF r.column_name='CreatedDate' THEN
         v_expr:='clock_timestamp()';
       ELSIF v_notnull AND v_default IS NULL THEN
         v_missing:=v_missing || r.column_name || ', ';
         CONTINUE;
       ELSE
         CONTINUE;
       END IF;
    ELSE
       IF v_ref IS NOT NULL THEN
         IF coalesce(v_mode,'legacy_id')='code' THEN
            v_expr:=format('legacy_import.resolve_code(%L,%L, NULLIF(btrim(s.%I),''''))',
                           v_schema,v_ref,v_source);
         ELSE
            v_expr:=format('legacy_import.make_id(%L,NULLIF(btrim(s.%I),''''))',
                           v_ref,v_source);
         END IF;
       ELSIF v_dataudt='bool' THEN
         v_expr:=format('legacy_import.to_bool(s.%I)',v_source);
       ELSIF v_dataudt='timestamptz' THEN
         v_expr:=format('legacy_import.to_timestamp(s.%I)',v_source);
       ELSIF v_dataudt='timestamp' THEN
         v_expr:=format('NULLIF(btrim(s.%I),'''')::timestamp',v_source);
       ELSIF v_dataudt='date' THEN
         v_expr:=format('NULLIF(btrim(s.%I),'''')::date',v_source);
       ELSIF v_dataudt='uuid' THEN
         RAISE EXCEPTION 'FK olmayan UUID sütunu için açık mapping gerekli: %.%',p_target,r.column_name;
       ELSIF v_dataudt IN ('int2','int4','int8','numeric','float4','float8') THEN
         v_expr:=format('NULLIF(btrim(s.%I),'''')::%s',v_source,
                  CASE v_dataudt WHEN 'int2' THEN 'smallint'
                  WHEN 'int4' THEN 'integer' WHEN 'int8' THEN 'bigint'
                  WHEN 'numeric' THEN 'numeric' WHEN 'float4' THEN 'real'
                  ELSE 'double precision' END);
       ELSE
         v_expr:=format('NULLIF(s.%I,'''')',v_source);
       END IF;
    END IF;
    v_cols:=v_cols||format(',%I',r.column_name);
    v_vals:=v_vals||','||v_expr;
 END LOOP;
 IF v_missing<>'' THEN
    RAISE EXCEPTION 'Hedef % için kaynağı olmayan zorunlu alanlar: %',p_target,v_missing;
 END IF;
 q:=format('INSERT INTO %I.%I (%s) SELECT %s FROM legacy_raw.%I s',
           v_schema,p_target,v_cols,v_vals,m.source_table);
 EXECUTE q;
 GET DIAGNOSTICS v_count = ROW_COUNT;
 RAISE NOTICE 'Yüklendi: % (% satır)',p_target,v_count;
 RETURN v_count;
END $$;

-- Self FK'ler, hedef tablodaki bütün kayıtlar oluşturulduktan sonra bağlanır.
CREATE OR REPLACE PROCEDURE legacy_import.relink_self(p_target text)
LANGUAGE plpgsql AS $$
DECLARE m legacy_import.table_map%ROWTYPE; v_schema text; pk text; src_fk text;
        r record; q text;
BEGIN
 SELECT * INTO m FROM legacy_import.table_map WHERE target_table=p_target;
 IF NOT FOUND OR NOT m.enabled OR m.source_table IS NULL THEN RETURN; END IF;
 SELECT value INTO v_schema FROM legacy_import.settings WHERE key='target_schema';
 pk:=legacy_import.source_column(m.source_table,coalesce(m.source_pk,'ID'));
 FOR r IN
   SELECT a.attname AS target_column
   FROM pg_constraint c
   JOIN pg_class t ON t.oid=c.conrelid
   JOIN pg_namespace n ON n.oid=t.relnamespace
   JOIN pg_attribute a ON a.attrelid=t.oid AND a.attnum=c.conkey[1]
   WHERE c.contype='f' AND c.conrelid=c.confrelid
     AND array_length(c.conkey,1)=1
     AND n.nspname=v_schema AND t.relname=p_target
 LOOP
   SELECT cm.source_column INTO src_fk FROM legacy_import.column_map cm
   WHERE cm.target_table=p_target AND cm.target_column=r.target_column;
   src_fk:=legacy_import.source_column(m.source_table,coalesce(src_fk,r.target_column));
   IF src_fk IS NULL AND right(r.target_column,2)='Id' THEN
     src_fk:=legacy_import.source_column(m.source_table,
                       left(r.target_column,length(r.target_column)-2));
   END IF;
   IF src_fk IS NULL THEN CONTINUE; END IF;
   q:=format(
    'UPDATE %I.%I t SET %I=legacy_import.make_id(%L,NULLIF(btrim(s.%I),'''')) '||
    'FROM legacy_raw.%I s WHERE t."Id"=legacy_import.make_id(%L,s.%I) '||
    'AND NULLIF(btrim(s.%I),'''') IS NOT NULL',
    v_schema,p_target,r.target_column,p_target,src_fk,
    m.source_table,p_target,pk,src_fk
   );
   EXECUTE q;
 END LOOP;
END $$;

CREATE OR REPLACE FUNCTION legacy_import.load_all()
RETURNS bigint LANGUAGE plpgsql AS $$
DECLARE r record; total bigint:=0;
BEGIN
 IF EXISTS (SELECT 1 FROM legacy_import.table_map WHERE enabled AND source_table IS NULL) THEN
   RAISE EXCEPTION 'Önce etkin tüm hedeflerin Oracle kaynak tablo eşlemelerini yapın veya kaynakta olmayanları enabled=false yapın';
 END IF;
 FOR r IN SELECT target_table FROM legacy_import.table_map
          WHERE enabled ORDER BY priority,target_table
 LOOP
   total:=total+legacy_import.load_one(r.target_table);
 END LOOP;
 FOR r IN SELECT target_table FROM legacy_import.table_map
          WHERE enabled ORDER BY priority,target_table
 LOOP
   CALL legacy_import.relink_self(r.target_table);
 END LOOP;
 RETURN total;
END $$;

CREATE OR REPLACE FUNCTION legacy_import.report_counts()
RETURNS TABLE(target_table text,source_table text,stage_count bigint,target_count bigint,status text)
LANGUAGE plpgsql AS $$
DECLARE r record; v_schema text; src_count bigint; dst_count bigint; msg text;
BEGIN
 SELECT value INTO v_schema FROM legacy_import.settings WHERE key='target_schema';
 FOR r IN SELECT m.* FROM legacy_import.table_map m ORDER BY m.priority,m.target_table
 LOOP
   target_table:=r.target_table;source_table:=r.source_table;
   stage_count:=NULL;target_count:=NULL;status:='OK';
   IF NOT r.enabled THEN status:='DISABLED'; END IF;
   IF r.source_table IS NULL THEN status:='SOURCE_NOT_MAPPED'; END IF;
   IF r.source_table IS NOT NULL AND to_regclass(format('legacy_raw.%I',r.source_table)) IS NOT NULL THEN
     EXECUTE format('SELECT count(*) FROM legacy_raw.%I',r.source_table) INTO stage_count;
   ELSIF r.enabled AND r.source_table IS NOT NULL THEN
     status:='STAGING_MISSING';
   END IF;
   IF to_regclass(format('%I.%I',v_schema,r.target_table)) IS NOT NULL THEN
     EXECUTE format('SELECT count(*) FROM %I.%I',v_schema,r.target_table) INTO target_count;
   ELSE
     status:='TARGET_MISSING';
   END IF;
   RETURN NEXT;
 END LOOP;
END $$;
