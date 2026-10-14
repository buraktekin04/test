İLÇE TANIMLARI — KLMN TEKNİK KURULUM
=====================================
Eski TownDef ekranındaki alanlar:
ObjectId, Code, Name, ExternalCode, Guid? CityDef, int? MernisCode,
CreationDate, CreatedBy, LastUpdate, UpdatedBy, ConcurrencyToken, IsActive.

Yeni TownDef : BaseEntity. Yalnızca Code, Name, ExternalCode?,
Guid? CityId, CityDef? City ve int? MernisCode içerir. Audit,
Id, IsActive, IsDeleted, Version (PostgreSQL xmin) BaseEntity'dedir.
ValidFrom ve ValidTo eski ilçede olmadığından eklenmemiştir.
CityDef.Towns 1:N ilişkisi nullable CityId ile Restrict FK olarak
eşlendi. Silinmemiş ilçesi olan şehir soft-delete edilemez.
Aktif ilçesi bulunan şehir pasifleştirilemez. Yeni şehir bağlantısı
ve ilçe aktivasyonu aktif şehri gerektirir; eski pasif referansı
bağlantı değiştirilmediği sürece korunur.

Backend:
GET /api/towns?search=...&cityId=...&isActive=...&pageNumber=...
GET /api/towns/{id}
GET /api/towns/city-options?selectedCityId=...
POST /api/towns
PUT /api/towns/{id}
PATCH /api/towns/{id}/status
DELETE /api/towns/{id}?version=...
GET /api/towns/export (Towns.Query, en çok 5000 satır)
Her CRUD command/handler/validator ayrıdır; 404/409 hataları
ProblemDetails olarak yönetilir. Towns.View/Query/Create/Update/
Delete seed edilir; modülün herhangi bir izni menü/route/listeyi
açar fakat mutation iznini genişletmez. Şehir dropdown'u Cities
yetkisi gerektirmez, yalnızca Towns yetkisi kullanılır.

Frontend:
Tanımlamalar -> İlçe Tanımları (/app/towns). Kod, İlçe Adı,
Harici Kod, Şehir ve MERNİS alanları. Backend arama/filtre/
sayfalama, yalnızca önyüzde PrimeNG kolon sıralama, alan altında
hata, genel kaydetme hatası, Excel, silme/durum/audit.
TypeScript ve C# sınıflarında Türkçe yorumlar eklenmiştir.

MIGRATION GEREKLİ: Repo gerçek yerel migrations snapshot'ını
içermiyor. Eski migration geçmişini koruyun. CountryDefs/CityDefs
zincirinden sonra TownDefs ve nullable CityId FK/unique indeksleri:
cd KLMN/Backend
dotnet ef migrations add AddTownDefinitions --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
Version için normal kolon oluşturmayın: xmin kullanılacak.
MSKBS şehir GUID'lerini doğrulamadan veri geçişi yapmayın.

Kontrol:
dotnet run --project KLMN/Tests/Towns.Regression/Towns.Regression.csproj
dotnet build KLMN/Backend/KLMN.slnx
npm --prefix KLMN/Frontend run build
Bu commit'te dotnet/Angular build, migration ve DB testleri
çalıştırılmamıştır. Statik kontrol dışında tam test gereklidir.
