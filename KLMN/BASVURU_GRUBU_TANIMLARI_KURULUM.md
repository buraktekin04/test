# Başvuru Grubu Tanımları — Kurulum ve Test

## Fotoğraftaki HcAdmissionTypeDef alanlarının karşılığı

`HcAdmissionTypeDef : EffectiveDatedEntity : BaseEntity` kullanılır.

- **Code**: `int`, pozitif, soft-delete olmayan kayıtlarda tekil.
- **Name**: `string`, zorunlu, en çok 250 karakter.
- **StartDate / EndDate**: ortak nullable UTC `ValidFrom / ValidTo`.
- **ObjectId, CreationDate, CreatedBy, LastUpdate, UpdatedBy, ConcurrencyToken, IsActive**:
  mevcut `BaseEntity` ile karşılanır; fiziksel ayrı Version kolonu değil `xmin`.
- `HcAdmissionTypeDefs`: 14 adet bağımsız, varsayılan `false` boolean alan:

| Eski bool property | Formda görünen alan |
|---|---|
| `ForcesCommandRequired` | Kuvvet Komutanlığı |
| `RankRequired` | Rütbe |
| `MilitaryClassRequired` | Askerî Sınıf |
| `MilitaryUnitRequired` | Birlik |
| `SenderChairRequired` | Gönderen Makam |
| `MilitaryOfficeRequired` | Askerlik Şubesi |
| `RegistryNumberRequired` | Sicil Numarası |
| `MilitarySchoolRequired` | Askerî Okul |
| `LastNominationDateRequired` | Son Adaylık Tarihi |
| `HeightWeightRequired` | Boy ve Kilo |
| `NthAdmissionRequired` | Kaçıncı Başvuru |
| `DispatchDocumentDateRequired` | Sevk Belgesi Tarihi |
| `DispatchDocumentNoRequired` | Sevk Belgesi Numarası |
| `PertemApprovalRequired` | PERTEM Onayı |

Hiçbir ek FK, dropdown tablosu, `Active` kopyası veya yeni entity özelliği
eklenmez. Bu bayraklar *seçenek değerleri* değil, başvuru grubunun hangi
bilgileri **zorunlu** tutacağını gösteren konfigürasyonlardır.

## Backend

Domain: `KLMN.Domain/Entities/Admissions/HcAdmissionTypeDef.cs`.
Persistence: `Configurations/Admissions/HcAdmissionTypeDefConfiguration.cs`,
`KLMNDbContext.HcAdmissionTypeDefs` ve `IKLMNDbContext`.
Kod tekilliği filtresi silinmiş kayıtları kapsamaz.

Application `AdmissionTypes/` altında her eylem için ayrı
`CreateHcAdmissionTypeDef`, `UpdateHcAdmissionTypeDef`,
`DeleteHcAdmissionTypeDef`, `ChangeStatusHcAdmissionTypeDef`
Command/Handler/Validator; liste `GetHcAdmissionTypeDefs` ve detay
`GetHcAdmissionTypeDefById` sorgu/handler/validator dosyaları.
Tüm zorunluluk alanları Create/Update/DTO/Audit'te karşılıklı eşlidir.

HTTP API: `/api/admission-types`
- GET: `search`, `includeInactive`, `isActive`, `pageNumber`, `pageSize`
- GET `/{id}`: güncel `Version` (xmin) ile detay
- POST: oluştur
- PUT `/{id}`: düzenle (`id` ve `version`)
- PATCH `/{id}/status`: aktif/pasif; `BaseEntity.IsActive` üzerinden
- DELETE `/{id}?version=...`: soft-delete
- GET `/export`: Excel; en fazla 5000 satır, her bool Zorunlu/Değil

`AdmissionTypes.View/Query/Create/Update/Delete` izinleri frontend/backend
ve `IdentitySeedData` içinde tanımlı. Yalnız Query Excel indirebilir.
Kayıt değişikliği optimistic concurrency ile 409'a dönüşür. Audit whitelist
yalnız güvenli bu 14 bool, Code, Name, ValidFrom, ValidTo, IsActive alanlarını içerir.
Genel audit türü `HcAdmissionTypeDef` olarak listelenir.

## Angular ekranı

`/app/admission-types`, Tanımlamalar → Başvuru Grubu Tanımları.
Onaylı diğer tanım ekranlarının şu işleyişi korunur: arama,
"Pasifleri dahil et", kolon durum filtresi (Tümü/Aktif/Pasif),
Excel, liste, PrimeNG p-table **yalnız frontend sayfası sıralaması**,
backend sayfalaması, işlem geçmişi, oluştur/düzenle/sil/durum popup ve
yetkiye göre butonlar.

Formda Kod (sayısal), Ad, tarihler ve üç görsel başlık altında 14 onay
kutusu vardır: Askerî Bilgiler (8), Başvuru ve Sağlık (3),
Belgeler ve Onay (3). Kutuların *hem kendileri hem etiketi* tıklanabilir.
Seçili = zorunlu, boş = zorunlu değil. Ana kayıt IsActive bu 14 kutudan
ayrıdır ve yalnız liste durum eyleminde değişir. Başka kolon,
backend filter-options-query veya alt seçenek entity'si oluşturulmaz.
Listeye zorunlu ayarların sayısı `n / 14` salt türetilmiş olarak gösterilir;
DB'ye ayrıca yazılmaz.

## EF migration — gerçek snapshot gerekir

Mevcut GitHub deposunda bütün migration geçmişine ve çalışan veritabanına
erişim olmadığından migration dosyası **uydurulmadı** ve uygulanmadı.
Lokal geliştirme ortamında mevcut `KLMNDbContextModelSnapshot` zincirini
koruyup yeni değişikliği üretin:

```powershell
cd KLMN/Backend
dotnet ef migrations add AddAdmissionTypeDefinitions --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
# Önce script'i gözden geçirin; yalnız test DB'sinde uygulayın
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
```

Beklenen: `HcAdmissionTypeDefs` tablosu, `Code int`, `Name varchar(250)`,
14 `bool NOT NULL`, `ValidFrom/ValidTo timestamp with time zone`,
soft-delete unique Code indeksi, ortak BaseEntity alanları ve xmin token.

## Doğrulama

```powershell
dotnet build KLMN/Backend/KLMN.slnx
dotnet run --project KLMN/Tests/AdmissionTypes.Regression/AdmissionTypes.Regression.csproj
npm --prefix KLMN/Frontend run build
```

Manuel kabul: 14 kutudan her birini aç/kapat, kaydet/yükle, boş kayıt,
pozitif sayısal kod, tarih doğrulaması, 403/404/409, soft-delete,
aktif/pasif, audit, Excel, mobil ve masaüstü form tıklanabilirliği.
Burada gerçek derleme, DB migration, API veya tarayıcı testi koşturulmadı.
