# Askerlik Şubesi Tanımları — Teknik Kurulum ve Kontrol

## Eski MSKBS fotoğrafından alan eşleşmesi

| Eski alan | KLMN karşılığı | Not |
|---|---|---|
| ObjectId | `BaseEntity.Id` | GUID |
| Code | `MilitaryOfficeDef.Code` | 100 karakter, zorunlu, silinmemiş kayıtlar arasında tekil |
| Name | `MilitaryOfficeDef.Name` | 250 karakter, zorunlu |
| ExternalCode | `MilitaryOfficeDef.ExternalCode` | Nullable, en fazla 100 karakter |
| City (Guid?) | `MilitaryOfficeDef.CityId` | Nullable `CityDef` ilişkisi |
| RelatedCity (Guid?) | **Eklenmedi** | Kullanıcı tarafından gereksiz bulundu |
| CreationDate / CreatedBy | `BaseEntity.CreatedDate/CreatedBy` | Audit interceptor |
| LastUpdate / UpdatedBy | `BaseEntity.UpdatedDate/UpdatedBy` | Audit interceptor |
| ConcurrencyToken | `BaseEntity.Version` | Npgsql PostgreSQL `xmin` |
| IsActive | `BaseEntity.IsActive` | Ayrı Active alanı yok |
| StartDate / EndDate | `EffectiveDatedEntity.ValidFrom/ValidTo` | Nullable UTC zamanları |

## İlişki ve veri bütünlüğü

`MilitaryOfficeDef : EffectiveDatedEntity` ve
`CityDef.MilitaryOffices` navigation koleksiyonu kullanılır.
`MilitaryOfficeDefs.CityId` için nullable FK ve `DeleteBehavior.Restrict`
uygulanır. Referansı bulunmayan/soft-delete olmuş şehirle yeni ilişki kurulamaz.
Yeni seçilen şehir aktif olmalıdır; değişmeyen geçmiş pasif şehir bağlantısı
düzenlemede kalabilir. Şube yeniden aktifleştirilirken ilişkili şehir aktiftir.
Silinmemiş şube bağlantısı varsa şehir silinemez; aktif şube varsa şehir
pasifleştirilemez. `RelatedCity` veya kuvvet ilişkisi eklenmez.

## Backend CQRS ve API

`KLMN.Application/MilitaryOffices` altında her işlem için kendi
Command/Handler/Validator dosyaları:
- CreateMilitaryOfficeDef
- UpdateMilitaryOfficeDef
- DeleteMilitaryOfficeDef
- ChangeStatusMilitaryOfficeDef
- GetMilitaryOfficeDefs, GetMilitaryOfficeDefById, GetMilitaryOfficeOptions

`GET /api/military-offices` ile search/includeInactive/isActive/cityId/pageNumber/pageSize.
`GET /api/military-offices/{id}` ile detay ve xmin Version.
`GET /api/military-offices/options?selectedCityId=...` ile şehir seçimi;
ayrı şehir yönetim yetkisine gerek yoktur.
`POST`, `PUT /{id}`, `PATCH /{id}/status`, `DELETE /{id}?version=...`.
`GET /api/military-offices/export` yalnız `MilitaryOffices.Query` izniyle,
filtrelenmiş en fazla 5000 kaydı Excel'e aktarır.

Domain, Angular ve IdentitySeedData'da `MilitaryOffices.View/Query/Create/Update/Delete`
izinleri birlikte tanımlıdır. Herhangi bir modül izni listeyi ve sayfayı açar;
Create/Update/Delete eylemleri ve Excel Query ayrıca korunur. Audit kaydı
sadece izinli alanları yazar; genel audit filtresi `MilitaryOfficeDef` içerir.

## Angular ekranı — mevcut tanım şablonu korunur

Tanımlamalar → Askerlik Şubesi Tanımları: `/app/military-offices`.
Askerî Okul Tanımları'nda kullanılan **aynı** arama, Pasifleri dahil et,
başlangıçta kapalı Kolon Filtreleri, Durum (Tümü/Aktif/Pasif),
Şehir dropdown'u, Filtreleri Uygula/Tümünü Temizle, PrimeNG p-table,
frontend sütun sıralaması, backend sayfalama, popup, alan bazlı hata,
Excel, işlem geçmişi ve eylem butonlarıdır.

Şube alanları: Kod, Ad, Harici Kod, Şehir ve geçerlilik başlangıç/bitişi.
Durum popup'ta yinelenmez; liste durum işlemlerinden yönetilir.

## EF Core migration — ZORUNLU, henüz yapılmadı

Bu GitHub çalışma kopyasında gerçek mevcut EF Migrations/ModelSnapshot
zinciri bulunmuyor. Uydurma initial migration veya yeni veritabanı
oluşturma işlemi YAPMAYIN. Mevcut migration zincirinde, CityDefs
tablosu zaten bulunuyorken:

```powershell
cd KLMN/Backend
dotnet ef migrations add AddMilitaryOfficeDefinitions --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
# Script, FK ve indeksleri doğrulayıp yalnız TEST veritabanına uygula:
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
```

Şema: `MilitaryOfficeDefs` tablosu, `CityId` nullable Restrict FK,
`Code` filtered unique index, nullable UTC `ValidFrom/ValidTo`,
`ExternalCode` ve BaseEntity alanları. Version bağımsız kolon değil
PostgreSQL `xmin` olacaktır. Gerçek migration/DB bu commit'te
oluşturulup uygulanmamıştır.

## Kontroller

```powershell
dotnet build KLMN/Backend/KLMN.slnx
dotnet run --project KLMN/Tests/MilitaryOffices.Regression/MilitaryOffices.Regression.csproj
npm --prefix KLMN/Frontend run build
```

Ek kabul testleri: farklı permission'larla erişim, şehir ilişki koruması,
pasif şehir seçimi, concurrent update 409, kayıt soft-delete,
audit/Excel, desktop/mobile liste/filtre/popup. Kaynaklar statik
denetlense de gerçek derleme, PostgreSQL migration, otomatik regresyon
çalıştırma ve tarayıcı testleri burada yapılmış kabul edilmez.
