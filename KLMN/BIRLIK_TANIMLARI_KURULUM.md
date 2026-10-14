# Birlik Tanımları — MilitaryUnitDef

Organizasyon ekranı kaldırıldı. Görev yeri artık `MilitaryUnitDef` tablosundan seçilir.

- Entity: `Backend/KLMN.Domain/Entities/MilitaryUnit/MilitaryUnitDef.cs`
- Kullanıcı ilişkisi: `User.MilitaryUnitDefId` → `MilitaryUnitDef.Id` (nullable, Restrict).
- Alanlar: `Code`, `Name`, `ExternalCode`, `ForcesCommandDef`, `ConnectedTopUnity`, `ValidFrom`, `ValidTo`.
- Kimlik, audit, aktiflik, soft-delete ve PostgreSQL xmin `Version` alanları BaseEntity'den gelir.
- `ConnectedTopUnity` üst birliğin kimliğidir; `ConnectedTopUnit` navigation'ıdır. Kendine/alt birliğe bağlanma API'de engellenir.
- `ForcesCommandDef`, kuvvet komutanlığı tablosuna nullable FK olarak bağlıdır. Birlik formunda aramalı dropdown kullanılır. [Kuvvet Komutanlığı kurulum adımları](KUVVET_KOMUTANLIGI_TANIMLARI_KURULUM.md).
- Tarihler kullanıcı ekranındaki ortak UTC dönüşümüyle çalışır. Boş uç sınırsızdır; başlangıç dahildir, bitiş hariçtir. Ters/eşit dönem reddedilir.
- Yeni kullanıcı görev yeri atamalarında aktif, silinmemiş ve geçerli dönem içindeki birlikler seçilebilir. Mevcut atama süresi dolduğunda geçmiş bağlantı korunur.
- Liste/CRUD/aktiflik/soft-delete, filtre, Excel, üst birlik seçimi ve ortak audit geçmişi vardır.
- Command, Handler, Validator ve Query dosyaları işlem bazlı ayrı klasörlerdedir.
- Menü/route: **Birlik Tanımları** / `/app/military-units`; API: `/api/military-units`.
- Kullanıcı seçenek API'si: `/api/users/military-unit-options`.

## Mevcut veritabanını güncelleme

Bu depo ilk migration/snapshot dosyalarını içermiyor; önceki migration geçmişi yerel projededir. Mevcut migration dosyalarını koruyun. Eski Organization entity/configuration/controller/feature klasörlerini silen Git değişikliklerini de alın; yalnızca yeni dosyaları kopyalamayın.

Package Manager Console (varsayılan proje KLMN.Persistence, startup KLMN.Api):

```powershell
Add-Migration ReplaceOrganizationWithMilitaryUnitDef
```

Oluşan migration'ın **Up ve Down gövdelerini** aşağıdakiyle değiştirin; üretilen Designer ve ModelSnapshot dosyaları güncel model olarak kalmalıdır. EF'nin önerdiği DropTable/CreateTable işlemlerini çalıştırmayın; görev yeri kimlikleri ve atamalar korunmalıdır.

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
    => KLMN.Persistence.MigrationSupport.MilitaryUnitDefMigrationOperations.Up(migrationBuilder);

protected override void Down(MigrationBuilder migrationBuilder)
    => KLMN.Persistence.MigrationSupport.MilitaryUnitDefMigrationOperations.Down(migrationBuilder);
```

Modelinizde `mskbs` gibi özel şema varsa iki çağrıya da ikinci parametre olarak bu şemayı verin. Yardımcı, depodaki önceki EF tablo/constraint/index adlarını esas alır; yerel adlar farklıysa üretilen script ile karşılaştırın.

```powershell
Script-Migration
Update-Database
```

Tablo ve FK kolonu yeniden adlandırılır; satır kimlikleri, kullanıcı bağlantıları ve hiyerarşi korunur. Yeni alanlar nullable olarak eklenir. Geri dönüş yeni dört alandaki verileri kaldırır.

API yeniden açıldığında permission seeder `Organizations.*` izinlerini kimliklerini koruyarak `MilitaryUnits.*` kodlarına taşır; mevcut rol/kullanıcı izin ilişkileri korunur. Eski ve yeni aynı izin birlikte varsa otomatik birleştirme yapılmaz. Yeni claim ve yetki kodlarının alınması için yeniden giriş yapın. Eski audit kayıtlarının JSON'u değiştirilmez; eski entity/alan adları okuma sırasında desteklenir.

## Yeni veritabanı

Önceden migration uygulanmamış tamamen yeni veritabanında README'deki `InitialCreate` akışını kullanın. Yeniden adlandırma yardımcısını ilk migration'a eklemeyin; güncel model doğrudan `MilitaryUnitDefs` tablosunu oluşturur.

## Doğrulama

- `.NET 10` API derlemesi ve Angular development derlemesi başarılı.
- `dotnet run --project KLMN/Tests/MilitaryUnits.Regression`: 18 kontrol; tarih sınırları/UTC, boş referans, concurrency, EF kullanıcı/üst birlik ilişkileri, varsayılan ve özel şemada veri koruyan migration SQL'i.
- Testler veritabanı bağlantısı açmaz; gerçek PostgreSQL veri geçişi bu ortamda uygulanmadı.
- Tarayıcı testi ortamın süreç/socket kısıtlaması nedeniyle çalıştırılamadı.
- Mevcut bağımlılıklardaki NuGet güvenlik uyarıları derlemede devam ediyor; bu değişiklik paket sürümlerini değiştirmez.
- Derlemeyi engelleyen mevcut RefreshToken namespace çakışması, eksik EF/HTTP using'leri ve iki TypeScript `as` satır sonu hatası da düzeltildi.

## Sadece görüntüleme yetkisi olan kullanıcılar

Rol panelinde yalnızca `Birlikleri Görüntüleme (MilitaryUnits.View)` atanması,
`Tanımlamalar > Birlik Tanımları` menüsünü ve sayfayı açmak için yeterlidir.
`MilitaryUnits.Query` izni olan mevcut rollerin erişimi de korunur.
Liste ve detay API'leri bu iki permission'dan herhangi birini kabul eder;
oluşturma, düzenleme ve silme kendi ayrı permission'larını gerektirir.
Excel dışa aktarma yalnızca `MilitaryUnits.Query` izniyle görünür ve sunucuda da bu izinle korunur.
Yetki seti login/refresh sırasında yeniden hesaplandığından, rol değişikliği sonrası
kullanıcı oturumunu yenilemelidir. Yeni migration gerekmiyor.
