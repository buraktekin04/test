# Tetkik Tanımları — KLMN geçerlilik tarihi standardı (21.10.2026)

**KESİN MİMARİ KARAR:** Eski MSKBS sistemindeki `ProcedureDef.StartDate/EndDate`
modern KLMN'de kullanılmaz. `ProcedureDef : EffectiveDatedEntity` olur ve
`ValidFrom/ValidTo` hem entity, PostgreSQL fiziksel kolonları, CQRS API/DTO,
Angular form/modeli, Excel hem de audit için tek kaynak isimleridir.
`BaseEntity`'deki Id, IsActive, IsDeleted, CreatedBy/UpdatedBy ve
PostgreSQL xmin/Version tekrar entity'ye yazılmaz.

İş alanları eski modelle aynı kalır: `ProcedureCode`, `Name`,
`RequiresNumericResult`, `RequiresReferenceRange`,
`RequiresDeviationInfo`, `ProcedureType`.
Üç boolean alan kullanıcıya "Numerik Sonuç Gerektirir",
"Referans Aralığı Gerektirir", "Sapma Bilgisi Gerektirir" diye gösterilir.

Eski veritabanında StartDate/EndDate mevcutsa silmeden önce iki kolonu
sırasıyla yeni ValidFrom/ValidTo kolonlarına taşımak gerekir. Repo'da EF
migration geçmişi olmadığı için göç SQL'i çalıştırılmadı; yerel migration
zinciri bilinmeden rename/drop yapılmaz.

## Dinamik Tetkik Türü
Yeni DB tablosu `ProcedureTypeDefs` (BaseEntity Id + `Code` int, `Name` string).
`ProcedureDefs.ProcedureType` kolonunun fiziksel adı ve int tipi değişmez.
`ProcedureType` FK ile `ProcedureTypeDefs.Code` alternate key'ine bağlıdır.
Silinmiş türün numarası bile başka anlamda tekrar kullanılamaz.
Tür adı güncellenebilir, kod güncellenemez. Kullanılan tür silinemez,
aktif tetkikleri olan tür pasifleştirilemez.
Dropdown `GET /api/procedure-types/options` üzerinden aktif değerleri okur;
düzenlemede eski seçili pasif değer de gösterilebilir.

`/app/procedure-types`: **Tetkik Türü Tanımları** ekranı.
`/app/procedures`: **Tetkik Tanımları** ekranı.
Her iki modülde ayrı permission, audit, CRUD, Excel, PrimeNG sorting/resize,
sunucu filtreleme ve sayfalama korunur.

## Enum aktarımı
Eski ProcedureType enumunun sabit SAYISAL ÜYELERİ bu konuşmadaki sistem
tarafından güvenilir şekilde okunamadı; **hiçbir enum kodu / isim seed edilmedi**.
Kodlar kullanıcı tarafından ayrı Tetkik Türü ekranında eklenebilir.
Eski verinin taşınmasından önce enumun her sayısal değeri ve Türkçe etiketi
`ProcedureTypeDefs.Code/Name` tablosuna birebir eklenmelidir.
Özellikle enumun 0 kodu varsa farklı bir anlama çevrilmemelidir.

## Migration — uygulanmadı
Repo'da EF Migrations/ModelSnapshot yok. Kod DB'ye kendiliğinden şema uygulamaz.
Yerel migration zinciri doğrulandıktan sonra **mevcut veriyi silmeden**
`ProcedureDefs` için kalıtımla gelen `ValidFrom/ValidTo` kolonları ve `ProcedureTypeDefs` tablo/FK migration'ı hazırlanmalı. Eski `StartDate/EndDate` verisi varsa açık veri taşıma planı yapılmalı.
Önceden oluşturulmuş `ProcedureDefs` kayıtlarının yeni tür FK'sine uygun kodları
olduğu doğrulanmadan NOT NULL/FOREIGN KEY kısıtı uygulanmamalıdır.
Eski canlı DB'yi sıfırlamayın, `EnsureCreated` veya `DropDatabase` kullanmayın.

## Test durumu
`Tests/ProcedureTypes.Regression` statik test kaynakları eklendi.
Bu ortamda `dotnet` SDK yok; build/migration/DB/browser testleri **çalıştırılmadı**.
