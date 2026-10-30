## Sevk Listeleme / Açılır Seçim / Cinsiyet / Belge Revizyonu — 30.10.2026

### 1. Sevk Listesi ilk açıldığında sorgulama YAPILMAZ
- `DispatchListComponent.ngOnInit()` artık `GET /dispatches`, durum veya makam/şehir/hastane seçeneklerini otomatik çağırmaz. Filtreler başlangıçta boştur. İlk açılışta “Sevk sorgusu henüz çalıştırılmadı” bilgisi görülür, tablo boş sonuç diye gösterilmez.
- Kullanıcı filtreleri belirleyip **Sorgula** butonuna bastığında sunucuya gider. Seçili kriterler `appliedForm` içine kopyalanır. Sayfa değiştirme/boyut güncelleme, bu **son uygulanmış** kriterlerle devam eder; kullanıcının ara form değişiklikleri otomatik sorgu başlatmaz.
- **Temizle** butonu bütün filtreleri, eski sonuçları ve `hasSearched` durumunu sıfırlar; otomatik tekrar arama yapmaz. Uçuşta eski sorgu sonuçları istek sıra numarasıyla göz ardı edilir.
- Durum sözlüğü ilk kez dropdown açılınca veya Sorgula'ya basılınca; makam/şehir/hastane dropdown kayıtları açılınca yüklenir. Yalnız tanım isteği asıl liste araması değildir.

### 2. Tüm PrimeNG dropdown filtrelerinde büyük/küçük harf duyarsız arama
- `Frontend/src/app/core/filters/case-insensitive-filter.ts` içinde Türkçe `İ/i/I/ı` ve Unicode harf normalizasyonu tanımlı. `app.config.ts` üzerinden global PrimeNG `FilterService.register` ile `contains`, `startsWith` ve `endsWith` modlarına uygulanır. Böylece ortak PrimeNG `p-select` ve `p-multiselect` dahil UI istemci filtreleri harf büyüklüğünden bağımsızdır.
- Backend'de `GetDispatchOptionsQueryHandler` artık `ToUpper().Contains` yerine PostgreSQL `EF.Functions.ILike` kullanır. Kullanıcının aradığı `%`, `_` ve ters eğik çizgi kaçırılarak literal arama sağlanır. Seçili ID'leri koruma ve max 100 seçenek kuralı aynen devam eder.
- Diğer modüllerde *kendine ait sunucu filtre endpoint'i* varsa onların SQL sorgusu ayrıca incelenmelidir; global PrimeNG değişikliği backend sorgularının kendisini değiştirmez.

### 3. Sevk Girişi belge ekleme kaldırıldı, fotoğraf KORUNDU
- `DispatchEntryComponent` HTML'inde **Belge Ekle**, yükleme alanı ve mevcut ekleri indirme alanı artık YOK. TS dosyasından `selectFile`, bekleyen dosya sinyali, yükleme/indirme adımları kaldırıldı.
- Fotoğraf çekme, seçme, önizleme, kaydetme ve silme akışı **aynen korunur**.
- Backend `DispatchFile` tablosu, mevcut dosya command/query/API endpoint'leri ve migration geçmişi **SİLİNMEDİ**. Veritabanı verisi korunur.

### 4. Cinsiyet artık DB'den alınır
- Yeni `GenderDef : BaseEntity`, `GenderDefs` tablosu, `GenderDefConfiguration` ve ayrı `GetDispatchGendersQuery` / Handler / Response eklendi. EF HasData başlangıç değerleri eski MSKBS enum ile aynıdır: **1 Erkek, 2 Kadın, 3 Belirtilmemiş**; UUID'ler sabittir.
- `GET /api/dispatches/genders` yalnız etkin ve silinmemiş tanımları getirir. Frontend `DispatchService.genders()` ile okur, `genderItems()` artık sabit dizi değildir. Sevk kaydına cinsiyet kodu gönderilirse `DispatchRules` aktif GenderDefs üzerinden doğrular.
- **MIGRATION GEREKİR**: `GenderDefs` yeni tablodur. Migration oluşturma ve uygulama işlemi kullanıcıya aittir; bu çalışma DB migration dosyası üretmedi, mevcut migration'ları silmedi. Önceden eklenmiş `DispatchStatusDefs` migration gereksinimi de devam edebilir.
- Bu sözlük için şimdilik ayrıca tanım ekranı yoktur.

### Kontrol
```bash
cd KLMN/Frontend
npm install
npm run test:filter-search
npm run test:dispatch-pages
npm run test:dispatch-qr
npm run build
```
`KLMN/Backend` dizininden (var olan migration durumuna göre):
```bash
dotnet ef migrations add AddGenderDefs --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
```
**Not:** .NET/Angular derlemesi, veritabanı migration'ı ve gerçek tarayıcı uçtan uca testi bu çalışma ortamında çalıştırılmadı.

## Sevk Girişi — tek bakışta kompakt çalışma ekranı (30.10.2026)

Eski MSKBS Sevk Giriş ekranının kişi tarafından kolay kullanımı, KLMN Angular 21 + PrimeNG yapısına yeniden uygulandı. Önceki büyük, üst üste yığılmış kartlar ve açılır askerî bilgi panelleri kaldırıldı.

**Yeni yerleşim**
- Üstte ince **Kimlik Sorgulama bandı**: T.C./Yabancı kimlik türü, kimlik numarası, doğum tarihi, **Nüfustan Sorgula** ve QR Kod. Edit modunda kimlik alanları salt okunurdur. Nüfus handler'ı kurum servisine bağlanıncaya kadar önceki gibi 503 döner.
- Ortadaki **iki sütun**: sol tarafta **Kişi Bilgileri** (ad/soyad, anne-baba, uyruk, doğum yeri, cinsiyet, telefon, adres) ve köşesinde **kamera/fotoğraf**; altına kompakt **Sevk Bilgileri** (kaçıncı işlem, evrak tarihi ve sayısı, kapsam, kapsam şehir/hastane, alım ID). Sağ tarafta **Askerî ve Başvuru Bilgileri**: grup ve sebep en üstte, devamında kuvvet/rütbe/sınıf/birlik/okul/şube, sicil/nasbı, gönderen makam, uçuş ve iki açıklama kutusu. **Hepsi aynı anda görünür**; gereksiz `<details>` katmanları yoktur.
- En altta **ince işlem bandı**: belge seçme, varsa mevcut sevk dosyaları, vazgeç ve **Sevk Kaydet**. Dar ekranlarda kolonlar kademeli olarak ikiye/bire iner. Normal masaüstünde dikey uzunluk mümkün olduğunca küçüktür.
- Zorunlu alanların **kırmızı `*` göstergeleri**, başvuru grubu ile sebebin sıraya bağlı seçimi, dinamik alan zorunlulukları, seçili tanımın API'den yüklenmesi, QR, kamera, fotoğraf silme, dosya yükleme, güncelleme ve mevcut validasyonlar korunmuştur.
- Formun stilleri artık yalnız `dispatch-entry.component.scss` içindedir. Liste ekranının CSS dosyası forma bağlı değildir. Listeleme ve sorgulama bileşeninde herhangi bir tasarım veya filtre değişikliği yoktur.

**Kontrol:** `cd KLMN/Frontend && npm run test:dispatch-pages && npm run build`. Gerçek masaüstü görüntüsü, fotoğraf/QR, ülke/şehir vb. seçenekler ve gönderim uçtan uca ayrıca test edilmelidir.

**Veritabanı/migration:** Bu yalnızca frontend görünüm değişikliğidir. Yeni tablo/kolon ya da migration eklenmedi. Önceki `DispatchStatusDefs` migration gereksinimi devam eder.

## Nüfus kimlik sorgusu ve açıklama alanları — 30.10.2026

### Kimlik sorgusu: eski MSKBS davranışına dönüş
- Eski MSKBS Sevk Girişi `queryPerson()` uygulaması `personService.getPerson(...)` çağırıyordu. Kimlik numarası ve doğum tarihi kurumun nüfus entegrasyonu üzerinden sorgulanıyor. KLMN içindeki önceki `Persons` yerel sorgusu bu ekranın beklentisini karşılamıyordu.
- Artık frontend **Nüfustan Sorgula** butonu ve `DispatchService.queryPopulation()` ile `POST /api/dispatches/persons/national-registry` çağırır. Formdan T.C. veya yabancı kimlik numarası ve doğum tarihi gönderilir.
- API `Dispatches.Create` veya `Dispatches.Query` yetkisi ile çağrılır. Yanıt için `Cache-Control: no-store` ve `Pragma: no-cache` yazılır; kişisel bilgiler loglanmamalıdır.
- Backend entegrasyonun bağlanacağı yer:
  `KLMN.Application/Dispatches/Queries/LookupDispatchPerson/LookupDispatchPersonQueryHandler.cs`.
  Burada **TODO** var; siz gerçek nüfus sistemine bağlayıp `DispatchPersonResponse` üretmelisiniz.
- DTO: `DispatchPersonResponse(Person, Military?)`. `Person` mevcut `DispatchPersonInput`; isteğe bağlı `Military` ayrı `DispatchRegistryMilitaryResponse` ile kuvvet, sınıf, rütbe, birlik, askerlik şubesi, okul, sicil, nasıp, uçuş ve başvuru grubu alanlarını destekler.
- Kurum servisindeki kodlar (ülke, ilçe, askerî tanımlar) doğrudan `Guid` kabul edilmemeli; KLMN tanımlarının ID'leriyle map edilmeli. Alan yoksa `null` bırakın.
- **Servis entegrasyonu şu anda BAĞLI DEĞİL.** Handler bilerek `NationalRegistryNotConfiguredException` fırlatır; API `503` döner. Başarılıymış veya kişi bulunamamış gibi sahte sonuç / yerel DB sonucu dönmez. Gerçek servis yanıtında eşleşme yoksa handler `null` döndürdüğünde `404` kullanılır. Input validator ayrı dosyadadır.
- QR kimlik ve doğum tarihiyle aynı backend sorgu noktası kullanılabilir. Elle kişi girişini kapatmadık.

### Eski MSKBS formu açıklama alanları
- Eski Sevk Girişi HTML kontrol edildi: `Birlik Açıklama` ve `Muayeneye Gönderen Makam Açıklama` **dx-text-area**, yani çok satırlıydı. Yeni formda her ikisi 3 satırlı, tam grid genişliğinde ve yeniden boyutlandırılabilir `textarea` oldu.
- Eski Sevk Girişi formunda bağımsız bir `Açıklama` input'u yoktu; yeni ekrandaki `description` alanı görünümden kaldırıldı, Uçuş grubunun ismi `Uçuş Bilgileri` oldu.
- Geriye dönük uyumluluk ve mevcut sevk verisini silmemek için `Dispatch.Description` backend/DB alanı ile formun görünmeyen kontrolü **korunur**, ancak kullanıcıya genel Açıklama input'u gösterilmez. Mevcut düzenleme kaydının değeri böylece yanlışlıkla silinmez.
- Bu revizyon **veritabanı migration'ı gerektirmez**; önceki `DispatchStatusDefs` migration gereksinimi aynen devam eder.

### Yerel test
```bash
cd KLMN/Frontend
npm install
npm run test:dispatch-pages
npm run build
```
```bash
cd KLMN/Tests/Dispatches.Regression
dotnet run
```
Nüfus servisini gerçek kurum ortamında bağladıktan sonra yetkiler, kimlik eşleştirme, hata/servis kesintisi senaryoları ve hassas veri günlükleme kontrolünü ayrıca yapın.

## Sevk Sorgulama ve Sevk Girişi kullanılabilirlik yenilemesi — 30.10.2026

### 1. Sevk durumları DB kaynağı
- `DispatchStatusDef : BaseEntity` entity, `DispatchStatusDefs` tablosu, unique `Code`, `Name`, `SortOrder`, `IsActive/IsDeleted/xmin`. Kullanıcı yönetim ekranı **yoktur**.
- EF Core `HasData` ile değişmeyen GUID'lerle 1=Yeni, 2=Provizyon, 3=Rapor, 4=İptal, 5=Bildirim başlangıç değerleri **migration oluşturulduğunda** eklenir.
- `GET /api/dispatches/statuses`: `GetDispatchStatusesQuery/Handler` ile doğrudan tablodan `code/name` listesi. Listede `statusItems` ve durum etiketi artık Angular kodunda sabit yazılmaz. Yeni seferde sunucudan yüklenir.
- **MIGRATION GEREKİR**: Yeni `DispatchStatusDefs` tablosu ve seed kayıtları mevcut DB'de henüz yoksa frontend durum seçenekleri yüklenemez. Migration'ı kullanıcının uygulaması gerekir. Projede migration veya DB reset yapılmadı.

### 2. Sevk Listeleme ve Sorgulama filtreleri
- `GET /api/dispatches`: `DateFrom/DateTo` (sevkin `DispatchDate` kayıt tarihi, UTC tarih aralığı), `CurrentState` (DB'deki durum), `DispatchNo`, `PurchaseId`, `TcKimlikNo`, `YabanciKimlikNo`, `Name`, `Surname`, `SenderMilitaryUnitDefId`, `SenderMilitaryOfficeDefId`, `IncludeRelatedOrganizations`, `DispatchScope`, `DispatchScopeCityId`, `DispatchScopeHospitalId`.
- `IncludeCancelled` ve eski `DocumentDateFrom/To` API parametreleri geriye dönük uyumluluk için korunur. Ekranda Başlangıç/Bitiş Tarihi **evrak değil sevkin sisteme giriş tarihi** bazındadır.
- Gönderen Makam seçiminde birlik/askerlik şubesi dropdown'dan seçilir. "Bağlı organizasyonlarda listele" checkbox'ı **yalnız birlik seçiliyken** kullanılabilir; `ConnectedTopUnity` hiyerarşisindeki tüm alt birlikler SQL filtrelemesine dahil edilir. Makam seçilmeden işaretlenemez. Başka kullanıcıya ait birliğin kapsamını filtrelemez; seçilen gönderen birliği genişletir.
- Kapsam Şehir yalnız `DispatchScope=2`, Sağlık Tesisi yalnız `DispatchScope=3` iken görünür. Seçim temizlenince ilgili alt filtreler de sıfırlanır. Sorgu tüm koşulları sayfalamadan önce SQL'de uygular; T.C. ve yabancı kimlik sonucu maskeli gösterilir.
- Görüntü: üstte tarih, durum, sıra no, alım ID ve hızlı arama; açılır "Ayrıntılı Filtreler" altında kimlik/ad-soyad/makam/kapsam/organizasyon.

### 3. Sevk Girişi formu
- `dispatch-entry-page` artık `max-width:1300px` ile ortalanmaz; diğer ekranlar gibi çalışma alanını doldurur.
- Başvuru Grubu ve Muayene Sebebi her zaman önce gösterilir; grup seçilmeden/kurallar yüklenmeden sebep dropdown'u kilitli ve açıklamalıdır. Başvuru grubu zorunluluk bayrakları alınmadan kayıt gönderilmez.
- Askerî kimlik/birlik, gönderen makam ve uçuş/ek açıklamalar ayrı katlanabilir gruplardır; grup zorunlu alan içeriyorsa otomatik açık gösterilir. Gönderen birlik ve gönderen askerlik şubesi aynı anda seçilmez.
- T.C./Yabancı kimlik, doğum tarihi, ad, soyad, başvuru grubu, muayene sebebi ve seçili gruba göre zorunlu diğer alanların etiket başında kırmızı `*` kullanılır. Bu ekranın veritabanı alanları değişmedi.
- Sol menü `Tetkik Tanımları` ikonu `pi pi-sliders-h` olarak güncellendi.

### Yerel kontrol
```bash
cd KLMN/Frontend
npm install
npm run test:dispatch-pages
npm run test:dispatch-qr
npm run build
```
`KLMN/Backend` içerisinden mevcut Migration kurulumunuza uygun örnek:
```bash
dotnet ef migrations add AddDispatchStatusDefs --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
```
**Uyarı:** Var olan sevk/fotoğraf migration'ı henüz yapılmadıysa bunları da kapsayacak şekilde mevcut ModelSnapshot'a göre migration hazırlayın. Migration'ı bu çalışma sırasında otomatik oluşturmadım/uygulamadım. PostgreSQL, Angular ve kamera/QR entegrasyon testleri yerel ortamda ayrıca yapılmalıdır.

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
