# Sevk ve Provizyon Sorgulama — Eski MSKBS karşılaştırması

Kaynak: depo kökündeki `test` (Sevk) ve `test3` (Provizyon).
Yeni KLMN mevcut mimarisini, GUID'leri ve ayrı ekran SCSS dosyalarını korur.

## Bu aşamada yapılanlar
- Sevk Sorgulama: GET /api/dispatches/{id} ile Sevk Detay modalı; başvuran fotoğrafı için GET /api/dispatches/{id}/photo; Kişi, Askerî ve Sevk Bilgileri.
- Mevcut tanımları (rütbe, kuvvet, başvuru grubu vb.) GUID olarak göstermemek için GET /api/dispatches/options çağrıları.
- Sevk iptali: kullanıcıdan gerekçe (1–500 karakter), PATCH /api/dispatches/{id}/cancel için `Reason` alanı, ayrı `Dispatches.CancellationReason` sütunu. Orijinal `Dispatch.Description` değişmez.
- Provizyon Sorgulama: mevcut GET /api/provisions/{id} içindeki kişi/sevk/provizyon bilgileriyle genişletilmiş Provizyon Detay modalı.
- GET /api/provisions?a=...&includeRelatedOrganizations=true: isteği yapan kullanıcının bağlı olduğu birlik ve alt organizasyonlar için sorgu. Kullanıcıya birlik atanmadıysa HTTP 400.
- Mevcut tablo kolonları için göster/gizle; T.C. kimlik numarası checksum doğrulaması.
- KLMN Audit Log altyapısı yeterli; yeni İz Kaydı API veya ekranı eklenmedi.

## Gereken EF Migration
`Dispatches.CancellationReason` nullable, 500 karakterlik yeni alan için migration gerekir. Bu bağlantıda migration üretilmedi/uygulanmadı. Eski kayıtlardaki GUID'ler ve eski açıklamalar değişmez.

## Harici API / eski backend kodu gerekiyor
Bu aşamada yeni endpoint olarak uygulanmadılar; gerçek implementasyon bekleniyor:
1. `IProvisionService.queryProvisionState`: provizyon durumu, hasta/alım, muayene ve tetkik listeleri.
2. `IProvisionService.queryProvisionSummaryState`: tarih aralığı ve alım ID bazında durum özeti.
3. `IProvisionService.getReportLastMonthList`: itiraz raporu seçimi için önceki raporların listesi.
4. `/mskbs/rapor/giris/:id`: rapor giriş ekranı; rapor oluşturma/kaydetme/inceleme API ve modelleri.
Eski `newProvision`, `newProvisionWithoutDispatchPhoto`, `IProvisionService.query`, `IDispatchService.query` kaynaklarının iş kuralı ve yanıt alanları da doğrulanmalıdır.

## Hâlen eksik / doğrulanacak
- Eski DevExtreme tablonun tüm saklı kolonlarının veritabanı sayfaları boyunca gruplanması ve başlık filtrelerinin eşdeğeri. Mevcut kolonlarda göster/gizle çalışması tasarlandı; bütün eski alanlar sorgu liste DTO'suna taşınmadı.
- Harici bakanlık Provizyon Takibi (muayene, tetkik, rapor imza/onay bilgileri), itiraz raporu sorgusu ve Rapor Giriş işlevleri eski backend sözleşmesi gelmeden tamamlanamaz.
- Sevk iptal açıklaması için migration + üretim veritabanında doğrulama.
- Derleme / API entegrasyon / tarayıcı uçtan uca testi bu bağlantıda çalıştırılmadı.

Kontrol komutları:
```bash
npm --prefix KLMN/Frontend run test:legacy-query
npm --prefix KLMN/Frontend run build
dotnet build KLMN/Backend/KLMN.slnx
```

## 06.11.2026 — Yeni `test4` kaynağı ile kontrol
Depoya bu geliştirme hazırlanırken `test4` dosyası eklendi; korunarak
yeni commit üstüne taşındı. Buradaki eski API kaynakları doğrulandı:
- `POST api/provizyon/sorguladetayli` (`SorgulaDetayli`);
- `POST api/provizyon/provizyondurum` (Angular istemci metodu);
- `POST api/provizyon/provizyonozetdurum` (`ProvizyonOzetDurum`);
- `POST api/provizyon/itirazraporsorgula` (`ItirazRaporSorgula`);
- `POST api/provizyon/yenisevkli`, `yenisevksizfoto`;
- `POST api/sevk/sorguladetayli`, `iptal`.

Bu dosya **bütün eski backend implementasyonlarını içermiyor**:
özellikle `_provisionWS.QueryProvisionState/QueryProvisionSummaryState`,
`_provisionService.GetReportLastMonthList`,
`_provisionService.QueryProvision`,
`MapToProvizyonOzetTo` ve Rapor Giriş modülünün gerçek servis
kodu/DTO'ları hâlâ gerekli. Yeni API entegrasyonları bu eksik sözleşmeler
doğrulanmadan otomatik oluşturulmadı.
