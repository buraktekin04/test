# Ülke ve Şehir Tanımları — KLMN (14.10.2026)

## Eski MSKBS modeli -> KLMN
- CountryDef: Code, Name, ExternalCode?, IsTownRequired? (bool?).
- CityDef: Code, Name, ExternalCode?, Country (Guid?), MernisCode? (int?),
  StartDate/EndDate. KLMN'de isimler CountryId (nullable FK), ValidFrom,
  ValidTo'dur. CityDef : EffectiveDatedEntity.
- Eski ObjectId, CreationDate, LastUpdate, CreatedBy, UpdatedBy, IsActive,
  ConcurrencyToken tekrar entity'ye eklenmez: BaseEntity.Id, CreatedDate,
  UpdatedDate, CreatedBy, UpdatedBy, IsActive, Version (PostgreSQL xmin).
- IsTownRequired üç değer alır: true/false/null, boş veya null "belirtilmedi".
- Şehir ülkesiz oluşturulabilir. CountryId doluysa ülke aktif ve silinmemiş
  olmalıdır. City.Country optional navigation / Country.Cities koleksiyonu
  üzerinden FK Restrict ilişkisidir.
- Ülke kodu ve şehir kodu, kendi tablolarındaki silinmemiş kayıtlar arasında
  case-insensitive tekil olmalıdır. Kodlar trim ve uppercase ile saklanır.
- Silinmemiş şehri olan ülke silinemez. Aktif şehri olan ülke pasif olamaz.
  Şehir aktifleştiriliyorsa bağlı ülkenin de aktif olması gerekir.
  PostgreSQL FK, yarım kalmış relation migration ve doğrudan DB işlemlerinde
  ikinci savunma hattıdır.
- Her tablonun create/update/status/delete command/handler/validator
  sınıfları ayrıdır. Update/status/delete güncel uint xmin ister.
  Global exception handler: bulunamadı 404, çatışma 409.
- City.ValidFrom/ValidTo UTC, nullable; başlangıç dahil, bitiş hariç.

## HTTP ve yetki
- GET /api/countries ve /api/cities: search, includeInactive, isActive,
  pageNumber/pageSize; ülke için isTownRequired, şehir için countryId filtresi.
- GET /api/countries/{id} ve /api/cities/{id}: detay ve Version.
- GET /api/cities/country-options: şehir oluşturma/düzenlemede aktif
  ülke seçimi; yalnızca Cities modülündeki herhangi bir yetki yeterli,
  kullanıcıya ülkeleri yönetme hakkı vermez.
- POST/PUT/PATCH status/DELETE: kendi Countries.* / Cities.* eylem yetkileri.
- GET /api/countries/export ve /api/cities/export: XLSX, sadece Query.
  En fazla 5000 kayıt ve listeyle aynı backend filtreleri.
- Ekranı açmak için modülün herhangi bir izni yeterli (View/Query/
  Create/Update/Delete). İşlem yetkileri ayrıca API'de kontrol edilir.
- Permissions iki platformda ve IdentitySeedData seed dosyasında tanımlıdır.

## Angular
- Tanımlamalar -> Ülke Tanımları: /app/countries
- Tanımlamalar -> Şehir Tanımları: /app/cities
- Reactice Forms, minimalist iki form, alan bazlı ve altta kaydetme hatası,
  p-table kolon sıralama yalnızca önyüzde; backend filtre/sayfalama.
- Şehir formunda ülke select ve MERNİS sayısal doğrulaması, nullable
  geçerlilik tarihi ve UTC dönüşümü bulunur.
- Ülke formunda IsTownRequired Belirtilmedi/Evet/Hayır select.
- Ortak audit dialog'u CountryDef / CityDef entityType kullanır.

## Migration — KRİTİK
Bu public GitHub deposu KLMN yerel migration geçmişini ve ModelSnapshot
dosyasını içermiyor. Canlı/veri dolu DB'de var olan zinciri bozmayın,
InitialCreate'i yeniden oluşturmayın, var olan tabloları silmeyin.
Yerel KLMN/Backend klasöründe mevcut snapshot üzerinden:
  dotnet ef migrations add AddCountryAndCityDefinitions --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
  dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
  dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
Üretilen migrasyonda CountryDefs, CityDefs, unique/index'ler, nullable
CountryId ve Restrict FK olmalı. ValidFrom/ValidTo yalnızca CityDefs'e
nullable timestamptz olarak açılır; Version diye normal sütun açılmaz.
İlişkili eski ülke/şehir verileri göç ettirilecekse gerçek Country GUID'leri
doğrulanmalı; düzmece veri veya varsayımsal eşleştirme yapılmamalıdır.
PostgreSQL + tarayıcı canlı kabul testleri bu commit'te çalıştırılmadı.

## Test
  dotnet run --project KLMN/Tests/Geography.Regression/Geography.Regression.csproj
  dotnet build KLMN/Backend/KLMN.slnx
  npm --prefix KLMN/Frontend run build:dev
Bu değişiklikte test kaynakları eklendi ama bu ortamda dotnet SDK ve
kurulmuş Angular workspace olmadığından çalıştırılmış olarak gösterilmemelidir.
İşlevsel ve ilişkisel veritabanı testleri ayrıca yapılmalıdır.
