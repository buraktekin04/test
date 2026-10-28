# 05 — Özel zorunlu alanlı 3 tanım

Bu paket üç özel tanımı **tek Oracle sonuç dosyası, tek PostgreSQL yardımcı tablo ve tek PostgreSQL aktarım SQL'i** ile taşır:
- HcAdmissionTypeDefs: 14 zorunlu bool
- HcDecisionDefs: 6 zorunlu bool
- ReasonForExaminationDefs: 3 zorunlu bool ve Description.

**Önce Oracle tablo/kolon isimlerini `00_ORACLE_KOLON_KONTROL.sql` ile doğrulayın.** Eski Oracle şeması bağlanmadığı için kaynak kolonların şu isimlerde olduğu kesin değil. Kaynakta boolean değeri olmayan veya farklı adlandırılmış tablolara varsayılan false yazmak veri kaybı yaratacağından bu SQL öyle yapmaz; eksik kolon halinde Oracle sorgusu hata verir. Kaynak tablosu olmayan yeni modül varsa SELECT bloğu kaldırılmalıdır.

1. Oracle'da 00 kontrol, ardından 01 Oracle export'u çalıştırın.
2. XLS → Excel Unicode Metin TXT → VS Code **Save with Encoding → UTF-8**. TAB ayırıcı kalsın.
3. `02_POSTGRES_STAGING.sql` ile `mskbs."_SpecialDefinitionsImport"` oluşturun. DBeaver CSV import, UTF-8, TAB, Header Top, Existing mapping. **26 sütun** görünmeli.
4. `03_POSTGRES_INSERT.sql` dosyasını PostgreSQL test ortamında **tek script** olarak çalıştırın. Bir boolean NULL/INVALID ise baştan hata verir. Hata olursa açık transaction'ı ROLLBACK yapın.
5. `04_POSTGRES_KONTROL.sql` ile satır sayısı/durum kontrol edin.

Yükleme yalnız kaydı olmayan Code'lar için INSERT yapar, mevcut GUID'leri ve ilişkili kayıtları değiştirmez. Yeni Id'ler kaynak tür+Code üzerinden deterministik MD5 UUID'dir. CreatedDate import anı, IsDeleted=false. Eski gerçek tarih, audit ve ek kolonlar kaynak verisi doğrulanmadan aktarılmaz. Test/backup şarttır; gerçek veriler public GitHub'a yüklenmez.
