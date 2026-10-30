## Ayrı Sevk Girişi ve Sevk Listeleme sayfaları — 30.10.2026

Mevcut Sevk Giriş ve Takip tek sayfa/modal mimarisi ayrıştırıldı. İşlemler artık 2 bağımsız Angular standalone bileşeninde bulunur:

- **`/app/dispatches/new` — Sevk Girişi:** `DispatchEntryComponent`. Başvuran kişi, QR okuyucu (kamera, görsel, USB/metin), kişi fotoğrafı (kamera, resim seçme, önizleme), başvuru ve askerî bilgiler, sevk dosyaları; bağımsız tam sayfa form. `Dispatches.Create` yetkisi gerekir.
- **`/app/dispatches` — Sevk Listesi ve Sorgulama:** `DispatchListComponent`. Sevk numarası/ad/soyad arama, durum filtresi, sevk **evrak tarihi** başlangıç/bitiş aralığı, iptalleri dahil etme, PostgreSQL üzerinden sayfalama, tablo kolon sıralaması/boyutlandırma, kayıt iptali ve audit. `Dispatches.View` **veya** `Dispatches.Query` yetkisi gerekir.
- **`/app/dispatches/:id/edit` — Sevk Düzenleme:** Aynı bağımsız kayıt formu, seçilen sevk bilgileriyle açılır. Yalnız `Dispatches.Update` ve `CurrentState=1 (Yeni)` ile düzenleme yapılabilir. Formdan çıkışta listeye dönülür (liste yetkisi yoksa ana sayfaya). Eski büyük düzenleme dialog'u kaldırılmıştır.
- Menüde `Sevk İşlemleri > Sevk Girişi` ile `Sevk İşlemleri > Sevk Listesi ve Sorgulama` iki ayrı item'dır. Her item kendi permission'ına göre gösterilir; oluşturma rolü liste yetkisi olmadan da kayıt açabilir.
- Kayıt başarılıysa ve liste izni varsa listeye yönlenip başarı mesajı gösterir; dosya/fotoğraf yükleme hataları ayrıca kullanıcıya bildirilir. Liste izni olmayan kayıt yetkilisi kayıt sayfasında kalır ve başarı mesajını görür.
- GET `/api/dispatches` sorgusu opsiyonel `CurrentState`, `DocumentDateFrom` ve `DocumentDateTo` alanlarıyla genişletildi. Tarih filtreleri **DispatchDocumentDate** alanına uygulanır; tarih aralığı validator ile doğrulanır. Durum=İptal seçildiğinde `IncludeCancelled=false` olsa bile istenen iptal kayıtları görünür; diğer durumlarda mevcut iptal gizleme davranışı korunur.
- `Frontend/tests/dispatch-pages-regression.mjs` kaynak sözleşmesi testi: `npm run test:dispatch-pages`. QR testini de `npm run test:dispatch-qr` ile çalıştırabilirsiniz.
- Bu ayrıştırma için **yeni tablo/kolon veya veritabanı migration'ı yoktur**. Önceki Sevk Giriş/Fotoğraf migration gereksinimleri aynen geçerlidir. Frontend/Backend derlemeleri ve gerçek tarayıcı uçtan uca testleri bu ortamda çalıştırılamadı.

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
Bu değişiklik **yeni Persons, Dispatches ve DispatchFiles tabloları**, `Dispatches.Photo` ve `Dispatches.PhotoContentType` kolonları ve permission seed değerleri gerektirir. Daha önce Sevk Giriş tablolarının migration'ı oluşturulduysa yalnız fotoğraf kolonlarını ekleyen migration üretin; oluşturulmadıysa yeni ilk sevk migration'ında bu iki kolonu da dahil edin. Güncel yerel `InitialCreate` / `ModelSnapshot` depoda **yok**; mevcut migrasyon zincirini silmeden, geliştiricinin kendi makinesinde migration oluşturup uygula. Bu çalışmada migration dosyası **üretilmedi/uygulanmadı** ve DB reset yapılmadı.

```powershell
cd KLMN/Backend
dotnet build
# Çalıştırılan DbContext projesi / startup projesi baz alınarak:
dotnet ef migrations add AddDispatchRegistration --project KLMN.Persistence --startup-project KLMN.Api
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api
cd ../Tests/Dispatches.Regression
dotnet run
cd ../../Frontend
npm install
npm run test:dispatch-qr
npm run build
```

Önce oluşturulan migration diff'ini okuyun: PostgreSQL bigint identity, nullable FK kısıtları, `DispatchScope` ve `CurrentState` check constraint, Person T.C./yabancı kimlik partial unique indeksleri, şema (`mskbs`), `xmin`, `bytea`, `PhotoContentType` ve fotoğraf 2 MiB uygulama limiti. Tenant/isolation ve özel veriler için ortam kısıtları doğrulanmadan canlıya geçmeyin.

## Entegrasyon sınırları ve kabul
1. QR kod okuma ve kamera fotoğrafı **tarayıcı üzerinden çalışacak şekilde** geliştirildi. Eski kaynakta QR için ayrı kurum API'si yok, metin/USB okuyucu kullanılıyordu. Yeni QR bileşeni arka kamera (ZXing), QR görsel dosyası ve USB okuyucu/metin girişini destekler. Eski `key:value,key:value` formatı ve JSON alanları ayrıştırılır. Doğum ve sevk tarihleri DD.MM.YYYY / YYYY-MM-DD doğrulanır; telefon no, kimlik numarası, GUID, sayısal alanlar reddedilmeden forma yazılmaz. Eski sistemin sabit başvuru grubu/GUID atamaları **yapılmaz**; mevcut KLMN tanımları doğrulanır. Başvuru Grubu seçilmemişse QR muayene sebebi bekletilir, seçilince doğrulanır.
2. Kamera fotoğrafı ayrı `Dispatch.Photo` (`bytea`) ve `PhotoContentType` alanlarında tutulur. Kişi fotoğrafı için arayüzde ön kamera, fotoğraf çekme, dosya yükleme, önizleme, seçim temizleme ve **kayıtlı fotoğrafı silme** vardır. GET/POST/DELETE `/api/dispatches/{id}/photo` uçları yetkilidir, 2 MiB JPEG/PNG ve magic bytes kontrolü içerir. Fotoğraf alanları audit whitelist'inde yoktur. Fotoğraf ve ek belge, sevk başarılı kaydedildikten sonra sırayla yüklenir; eklerden biri hata verirse sevk kaydı korunur ve hata bildirilir. Kamera kapanışta/diyalog kapanınca track'ler durdurulur.
3. Eski sistemin gerçek MERNİS/kurum dışı kişi sorgu API'sinin sözleşmesi ve erişim bilgileri kaynak `test` dosyasında **yok**. KLMN yalnız yerel DB'de kimlik numarası + doğum tarihi eşleşen kişiyi getirir; eşleşme yoksa kişi elle doldurulur. Harici MERNİS entegrasyonunun sağlandığı iddia edilmez; kurum altyapısı sağlanınca ayrıca bağlanacaktır.
4. `DispatchFile` eski kaynakta `DispatchLaw3713` navigasyonu barındırıyor; yeni sevk ekleri açık `DispatchId` FK ile bağlanır. Belge indirme id + sevk eşleşmesi ve izin denetimi gerektirir. Dosya adları/yükleme türleri filtrelenir. Tek kayıt işleminde bir dosya yüklenir; daha fazla belge için kaydı tekrar düzenleyerek ek yüklenebilir.
5. Eski `Gender` sayısal sözleşmesi ve `PurchaseId` seçim kaynağı ayrıca onaylanmalıdır. `PurchaseId` serbest sayısal alan; eski `PurchaseNumberDef` KLMN'den kaldırılmıştır. Dış sistemden kişi/kimlik doldurulmuş kabul etmeyin.
6. Başvuru grubu Required alanları ve kullanılmayan muayene sebepleri, aktif/pasif referanslar, `409`, optimistic concurrency, nullable alanlar, iptal, belge indirme, doküman büyüklüğü, View-only/Query-only/Create-only/Update rolleri API ve tarayıcıda sınanmalıdır.
7. Kişi güncellemesi yerel kişi kaydını ortak sevkler için güncelleyebilir. Kimlik ve doğum tarihini mevcut sevkten değiştirmek yasaktır. Kurumsal kişi ana verisi/versiyonlama politikası belirlenince ayrı tasarım yapılmalıdır.

## Test durumu
- Kaynak kodu ve DB/Angular bağlantı noktaları GitHub üzerinden incelendi, kaynak-temelli statik kontrol yapıldı.
- `KLMN/Tests/Dispatches.Regression` .NET 10 kaynak regresyonuna fotoğraf MIME, boyut ve concurrency kontrolleri eklendi.
- `Frontend/tests/dispatch-qr-regression.mjs` QR legacy/JSON biçimi, tarih, telefon, kimlik, GUID ve izinli alan testlerini Node 22 üzerinde çalıştırabilir.
- Bu ortamda **gerçek dotnet build/test, npm install/build, QR regresyonu, PostgreSQL migration/DB testi ve kamera/tarayıcı kabul testleri çalıştırılmadı**; bu yüzden entegrasyon/üretim hazır olarak işaretlenmez.

## İş akışı
Yeni geliştirmeler bu modülü `KLMN/beni oku.txt` standartlarıyla sürdürmeli; migrationları mevcut yerel snapshot üzerinden geliştirici yönetmeli. Değişiklikler tek squash commit ile `main` dalına aktarılır.

## QR / kamera kurulumu ve kabul senaryoları (30.10.2026)
- `Frontend/package.json` içine `@zxing/browser@^0.2.1` eklendi. Depoda önceki `package-lock.json` bulunmadığından bu değişiklikte lockfile oluşturulamadı; `npm install` sonrasında üretilen `package-lock.json` ayrıca commitlenmelidir.
- Kamera için **HTTPS veya localhost** ve tarayıcıdan izin gereklidir; iOS/Safari, Chrome ve masaüstünde gerçek cihaz kabul testi şarttır. Kameradan okuma tarayıcı içindedir, fotoğraf yalnız sevk kaydı kaydedildiğinde sunucuya gider.
- Kabul: (1) yeni sevk -> QR Kod Okut -> USB okuyucu, metin, kamera ve görsel ile alanları doldurma; (2) eski tarih formatı ve 10 haneli telefon; (3) geçersiz alanlar popup; (4) eski GUID yoksa manuel seçim uyarısı; (5) Başvuru Grubu sonradan seçilince bekleyen QR muayene sebebi; (6) kişi fotoğrafı çek/yükle/önizle/kaydet/yeniden aç/sil; (7) modal kapandığında kamera ışığı kapanmalı; (8) fotoğraf 2 MiB, belge 5 MiB, desteklenmeyen MIME ve imza reddi; (9) yalnız yetkili rol fotoğraf okuyabilir veya değiştirebilir; (10) uygulamanın güncel yerel PostgreSQL migration'ı olmadan uçlar çalışmaz.
