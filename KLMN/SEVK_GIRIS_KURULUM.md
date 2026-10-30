# KLMN — Sevk Giriş ve Takip (30.10.2026)

## Kaynak ve mimari
Eski MSKBS'deki deponun kökündeki `test` dosyası (30.10.2026), `dispatch.component.html`, `dispatch.component.ts`, `Dispatch`, `Person`, `DispatchScope` ve `DispatchFile` modelleri incelenerek KLMN'ye yeni bir uygulama olarak taşındı. Eski DevExtreme/Nebular, Oracle, `ITCHEntity`, `ObjectId`, `ConcurrencyToken` ve mevcut olmayan servisler **doğrudan kullanılmadı**.

- Backend: `KLMN.Domain.Entities.Admissions.Person`, `Dispatch`, `DispatchFile` (BaseEntity, audit, soft delete, PostgreSQL xmin); EF mapping ve güvenli foreign key'ler; ayrı MediatR CQRS komutları, sorguları, handler/validator, Response/Exception dosyaları
- API: `/api/dispatches` — GET liste/detay, POST create, PUT update, PATCH cancel; `persons/lookup` yalnız **yerel DB** sorgusu; `options`, `admission-rules/{id}`, `/{id}/files` liste/yükleme/indirme
- Frontend: `/app/dispatches` — Angular 21 standalone + PrimeNG; person/sevk/askerî bilgi, form doğrulama, server-side sayfalama, PrimeNG kolon sort/resize, onaylı iptal, popup hata, audit, belge eki
- Yetkiler: `Dispatches.View`, `Dispatches.Query`, `Dispatches.Create`, `Dispatches.Update`. Kişi sorgusu yalnız Query; iptal ve düzenleme Update. **Delete yok**: iptal geçmişi korunur.
- Başvuru grubu `HcAdmissionTypeDef`: ilgili Required bayrakları ve kullanılmayacak muayene sebepleri API üzerinden uygulanır. Seçimler mevcut ülke/ilçe/şehir/hastane/kuvvet/rütbe/sınıf/birlik/okul/şube/uçuş tanımlarına GUID FK ile bağlanır.
- Sevk kapsamı: 1=Kapsam yok, 2=Şehir, 3=Sağlık tesisi. Sevk durumları: 1=Yeni, 2=Provizyon, 3=Rapor, 4=İptal, 5=Bildirim. İptal edilen kayıt fiziksel silinmez.
- Sevk no: PostgreSQL identity bigint (ve unique index). Kimlik numaraları string olarak saklanır; rakamların başı kaybolmaz, liste ekranında maskelenir. Belge PDF/PNG/JPEG, 5 MiB limit, sunucuda dosya imzası denetimi. Dosyalar `bytea` saklanır; boyut/retansiyon ayrıca değerlendirilmelidir.
- Hassas kişisel veriler audit whitelist'ine **alınmadı**. Sevk numarası, durum, kapsam ve tanım bağlantıları auditlenir. Gerçek veriyle testte yetki/işlem kayıtları ayrıca incelenmelidir.

## Migration — geliştirici ortamında
Bu değişiklik **yeni Persons, Dispatches ve DispatchFiles tabloları** ve permission seed değerleri gerektirir. Güncel yerel `InitialCreate` / `ModelSnapshot` depoda **yok**; mevcut migrasyon zincirini silmeden, geliştiricinin kendi makinesinde migration oluşturup uygula. Bu çalışmada migration dosyası **üretilmedi/uygulanmadı** ve DB reset yapılmadı.

```powershell
cd KLMN/Backend
dotnet build
# Çalıştırılan DbContext projesi / startup projesi baz alınarak:
dotnet ef migrations add AddDispatchRegistration --project KLMN.Persistence --startup-project KLMN.Api
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api
cd ../Tests/Dispatches.Regression
dotnet run
cd ../../Frontend
npm ci
npm run build
```

Önce oluşturulan migration diff'ini okuyun: PostgreSQL bigint identity, nullable FK kısıtları, `DispatchScope` ve `CurrentState` check constraint, Person T.C./yabancı kimlik partial unique indeksleri, şema (`mskbs`), `xmin`, `bytea`. Tenant/isolation ve özel veriler için ortam kısıtları doğrulanmadan canlıya geçmeyin.

## Entegrasyon sınırları ve kabul
1. Eski sistemin gerçek MERNİS/kurum dışı kişi sorgu API'si, QR kod okuma/kamera/fotoğraf çekme servisinin endpoint ve yetkilendirme sözleşmeleri kaynak dosyada **yok**; sahte entegrasyon kurulmadı. Kişi, yerel DB'den kimlik + doğum tarihi eşleşirse getirilir; değilse elle girilir. Gerçek QR ve kurum dışı sorgu için ayrı entegrasyon fazı gerekir.
2. `DispatchFile` eski kaynakta `DispatchLaw3713` navigasyonu barındırıyor; yeni sevk ekleri açık `DispatchId` FK ile bağlanır. Belge indirme id + sevk eşleşmesi ve izin denetimi gerektirir. Dosya adları/yükleme türleri filtrelenir. Tek kayıt işleminde bir dosya yüklenir; daha fazla belge için kaydı tekrar düzenleyerek ek yüklenebilir.
3. Eski `Gender` sayısal sözleşmesi ve `PurchaseId` seçim kaynağı ayrıca onaylanmalıdır. `PurchaseId` serbest sayısal alan; eski `PurchaseNumberDef` KLMN'den kaldırılmıştır. Dış sistemden kişi/kimlik doldurulmuş kabul etmeyin.
4. Başvuru grubu Required alanları ve kullanılmayan muayene sebepleri, aktif/pasif referanslar, `409`, optimistic concurrency, nullable alanlar, iptal, belge indirme, doküman büyüklüğü, View-only/Query-only/Create-only/Update rolleri API ve tarayıcıda sınanmalıdır.
5. Kişi güncellemesi yerel kişi kaydını ortak sevkler için güncelleyebilir. Kimlik ve doğum tarihini mevcut sevkten değiştirmek yasaktır. Kurumsal kişi ana verisi/versiyonlama politikası belirlenince ayrı tasarım yapılmalıdır.

## Test durumu
- Kaynak kodu ve DB/Angular bağlantı noktaları GitHub üzerinden incelendi, kaynak-temelli statik kontrol yapıldı.
- `KLMN/Tests/Dispatches.Regression` .NET 10 kaynak regresyonu eklendi.
- Bu ortamda **gerçek dotnet build/test, npm build, PostgreSQL migration/DB testi ve tarayıcı kabul testleri çalıştırılmadı**; bu yüzden entegrasyon/üretim hazır olarak işaretlenmez.

## İş akışı
Yeni geliştirmeler bu modülü `KLMN/beni oku.txt` standartlarıyla sürdürmeli; migrationları mevcut yerel snapshot üzerinden geliştirici yönetmeli. Değişiklikler tek squash commit ile `main` dalına aktarılır.
