# ProcedureDef ve dinamik Tetkik Türü Tanımları — 20.10.2026

## Legacy alan adları korunur
Ekran adı Tetkik Tanımları, entity **ProcedureDef**, DB tablosu **ProcedureDefs**.
Eski modelde alanlar: `ProcedureCode` (string), `Name` (string),
`RequiresNumericResult` (bool), `RequiresReferenceRange` (bool),
`RequiresDeviationInfo` (bool), `StartDate` (nullable UTC datetime),
`EndDate` (nullable UTC datetime), `ProcedureType` (eski enumun int kodu).

UI checkbox başlıkları **Numerik Sonuç Gerektirir**, **Referans Aralığı Gerektirir**,
**Sapma Bilgisi Gerektirir** olarak aynen gösterilir. Liste, form, Excel, API, audit
ve sorgu DTO'ları bu alanları kullanır.

KLMN.BaseEntity'deki audit/IsActive/IsDeleted/xmin alanları yeniden eklenmedi.
Önceden geçici uygulanan `Code`, `ValidFrom`, `ValidTo` alanları bu entity'de
kaldırılarak eski modelin `ProcedureCode`, `StartDate`, `EndDate` yapısına geçildi.

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
`ProcedureDefs` alan değişimi ve `ProcedureTypeDefs` tablo/FK migration'ı hazırlanmalı.
Önceden oluşturulmuş `ProcedureDefs` kayıtlarının yeni tür FK'sine uygun kodları
olduğu doğrulanmadan NOT NULL/FOREIGN KEY kısıtı uygulanmamalıdır.
Eski canlı DB'yi sıfırlamayın, `EnsureCreated` veya `DropDatabase` kullanmayın.

## Test durumu
`Tests/ProcedureTypes.Regression` statik test kaynakları eklendi.
Bu ortamda `dotnet` SDK yok; build/migration/DB/browser testleri **çalıştırılmadı**.
