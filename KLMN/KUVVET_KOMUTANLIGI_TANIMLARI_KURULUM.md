# Kuvvet Komutanlığı Tanımları

## Model ve ilişki

`ForcesCommandDef : BaseEntity` yeni tanım tablosudur. Alanlar `Code`, `Name`, `ValidFrom`, `ValidTo`.
Kimlik, audit, aktiflik, soft-delete ve `uint Version` / PostgreSQL `xmin` mevcut BaseEntity'den gelir.
Fotoğraftaki ObjectId/CreationDate/LastUpdate/ConcurrencyToken alanları tekrar oluşturulmaz.

Mevcut `MilitaryUnitDef.ForcesCommandDef` nullable GUID kolonu korunur ve
`ForcesCommandDefs.Id` alanına **Restrict** foreign key ile bağlanır.
Navigation: `MilitaryUnitDef.ForceCommand`; ters koleksiyon: `ForcesCommandDef.MilitaryUnits`.
Kullanıcı görev yeri ilişkisi (`User.MilitaryUnitDefId`) aynı kalır.

## Ekran ve API

- Menü: **Kuvvet Komutanlığı Tanımları**, route: `/app/forces-commands`.
- API: `/api/forces-commands`; liste, detay, create, update, status, delete ve Excel.
- Her işlem ayrı Command / Handler / Validator klasöründedir; sorgular ayrı Query / Handler dosyalarındadır.
- Arama, aktif/pasif filtre, sayfalama, ad/kod, geçerlilik tarihleri, bağlı birlik sayısı ve ortak audit geçmişi vardır.
- Birlik formundaki serbest GUID alanı kaldırılmıştır. Aramalı PrimeNG dropdown kuvvet tablosundan beslenir.
- `/api/military-units/force-options` yalnızca aktif, silinmemiş, o an geçerli kuvvetleri döndürür.
  Birlik sorgulama yetkisi yeterlidir; kullanıcının ayrıca kuvvet tanımlarını yönetme yetkisi gerekmez.
- Yeni kuvvet bağlantısı ve birliği aktifleştirme işlemi API'de de doğrulanır. Var olan pasif/süresi dolmuş bağlantı başka alanlar düzenlenirken korunabilir.
- Bağlı silinmemiş birliği bulunan kuvvet silinemez. Aktif birliği bulunan kuvvet pasifleştirilemez.
- Kuvvet kodu silinmemiş tanımlar arasında benzersizdir; trim/uppercase uygulanır.
- Tarihler UTC saklanır, ekranda yerel saatle düzenlenir. Boş uç sınırsız; başlangıç dahil, bitiş hariçtir.
- Güncelleme, durum değişimi ve silme işlemleri mevcut xmin sürümünü ister.
- `ForcesCommands.View/Query/Create/Update/Delete` permission seed kayıtları eklenir.
- Birlik audit geçmişinde kuvvet GUID'i yerine ad/kod gösterilir; audit'in özgün JSON'u değişmez.

## Mevcut veritabanında kurulum

Depoda migration geçmişi/snapshot paylaşılmadığından kendi yerel migration dosyalarınızı koruyun.
Önce Birlik dönüşümü uygulanmış olmalıdır. Yeni değişiklikleri aldıktan sonra Package Manager Console'da:

```powershell
Add-Migration AddForcesCommandDefinitions -Project KLMN.Persistence -StartupProject KLMN.Api
Script-Migration -Project KLMN.Persistence -StartupProject KLMN.Api
Update-Database -Project KLMN.Persistence -StartupProject KLMN.Api
```

Migration yeni `ForcesCommandDefs` tablosunu, Code/Name indekslerini ve mevcut birlik kolonuna FK eklemelidir.
Mevcut `MilitaryUnitDefs`, kullanıcılar veya `ForcesCommandDef` kolonu silinmemelidir.
`Version` için normal sütun oluşturulmaz; Npgsql xmin kullanır.
`MigrationSupport/ForcesCommandDefMigrationOperations.cs` aynı geçişin açık Up/Down karşılığını içerir;
isteğe bağlı olarak üretilen migration gövdelerinden çağrılabilir. Özel şema varsa ikinci parametreyi verin.

### Önceden serbest kuvvet GUID'i girildiyse

Yeni tablo ilk oluşturulduğunda boş olur. Eski birliklerde dolu kuvvet referansı varsa,
FK eklenmeden önce karşılık gelen **gerçek** kuvvet kayıtları aynı kimliklerle aktarılmalıdır.
Kontrol sorgusu (kurum şemasına göre tablo adını niteleyin):

```sql
SELECT DISTINCT "ForcesCommandDef"
FROM "MilitaryUnitDefs"
WHERE "ForcesCommandDef" IS NOT NULL;
```

Sonuç varsa üretilen migration'da `CreateTable` ile `AddForeignKey` arasına gerçek kuvvet kayıtlarının
`InsertData` işlemlerini ekleyin (Id, Code, Name, CreatedDate, IsActive, IsDeleted).
Kayıt adlarını/kodlarını tahmin ederek otomatik seed yapılmaz; mevcut referanslar null yapılmaz veya silinmez.
Eşleşmeyen referanslar varsa PostgreSQL FK kurulmasını reddeder ve migration transaction'ı geri alınır.

API açılınca permission seed çalışır. Güncel izinlerin alınması için yeniden giriş yapın.
Önce kuvvet tanımlarını oluşturun, ardından Birlik formundan seçim yapın.

## Doğrulama

```bash
dotnet build KLMN/Backend/KLMN.Api/KLMN.Api.csproj
npm --prefix KLMN/Frontend run build:dev
dotnet run --project KLMN/Tests/MilitaryUnits.Regression
```

Regresyonlar tarih/UTC kurallarını, xmin, nullable Restrict FK'yi, shadow FK oluşmamasını,
kuvvet adlarının SQL projeksiyonunu ve varsayılan/özel şemada migration SQL'ini doğrular.
Bu kontroller canlı veritabanına bağlanmaz. Gerçek PostgreSQL veri geçişi kullanıcı ortamında uygulanmalıdır.
