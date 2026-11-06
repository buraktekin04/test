# KLMN — Azure DevOps CI, Artifacts ve Release Kurulumu

## Durum ve kapsam

KLMN'nin mevcut depodaki kaynakları `KLMN/Backend` (.NET 10) ve
`KLMN/Frontend` (Angular 21 / PrimeNG 21) altındadır.
Bu kurulum **derleme ve IIS yayın tanımlarıdır**. Azure DevOps kuruluşuna
erişim olmadığı için platform üzerinde pipeline/agent/variable group
oluşturulmadı ve hiçbir Test/Production sunucusuna dağıtım yapılmadı.

- `azure-pipelines-ci.yml`: `main` üzerindeki KLMN değişimlerinde
  backend ve frontend ayrı işlerde restore/build/test/publish yapar.
- `azure-pipelines-release.yml`: **manuel** release; Test → isteğe bağlı
  Production. Production için Azure DevOps Environment approval gereklidir.
- `deploy/azure-devops/iis-release-steps.yml`: CI'nin seçilen run'ından
  Build Artifacts indirir ve IIS dağıtım görevini çalıştırır.
- `deploy/azure-devops/deploy-iis.ps1`: Windows IIS üzerinde siteyi
  durdurur, staging klasöründen günceller, eski sürümü yedekler; hata
  durumunda önceki dosyaları geri yüklemeyi dener.
- `deploy/azure-devops/frontend-web.config`: Angular history routing;
  **/api reverse proxy kuralı içermez**; sunucu üzerinde ayrıca gerekir.

### Artifact isimleri

| CI Artifact | İçerik | Yayın |
| --- | --- | --- |
| `KLMN-API` | `KLMN.Api.zip` — .NET 10 publish | Backend / IIS |
| `KLMN-WEB` | `KLMN.Web.zip` — Angular production browser çıktısı + `web.config` | Frontend / IIS |
| `KLMN-DEPLOY` | PowerShell deploy script, template, web.config | Classic Release alternatifi |

**Önemli:** API publish ZIP'i `appsettings*.json` dosyalarını **içermez**.
DB/JWT/SMTP/InitialAdmin ayarları repoya, CI loguna veya artifact ZIP'ine
konulmaz. İlk dağıtımdan önce ilgili IIS API site dizininde gerçek
`appsettings.json` bulunmalıdır; script bu dosyayı yeni sürüme taşır.
Windows dosya ACL'leriyle bu dosyayı ve yedekleri koruyun. Ortam
değişkenleri / Key Vault / Azure DevOps secret variables kullanılabilir;
secret'lar asla Angular environment.ts dosyasına yazılmamalıdır.

## Azure DevOps'ta bir kere yapılacak kurulum

1. Azure DevOps → **Pipelines → New pipeline** → kaynak
   **GitHub** (`buraktekin04/test`) veya Azure Repos'a alınmış aynı repo.
   **Existing Azure Pipelines YAML file** seç:
   `/KLMN/azure-pipelines-ci.yml`. Pipeline adını **KLMN-CI** yap.
   Repository root'tan checkout beklenir; yalnız `KLMN/**`
   değişikliklerinde CI tetiklenir. KLMN iki ayrı repo'ya taşınırsa
   YAML içindeki yolları güncelle.
2. CI için Windows hosted agent `windows-2022` kullanılır. Kurumsal ağda
   hosted agent erişimi kapalıysa CI'daki `pool` alanlarını .NET 10
   SDK ve Node.js 22 kurulu **Windows self-hosted agent pool** ile değiştir.
   NuGet ve npm registry erişimi ya da onaylı kurum içi mirror gereklidir.
3. **İlk CI çalışması** başarılı olunca Run → Artifacts altında yukarıdaki
   üç ayrı artifact'i kontrol et. İkisinin ZIP dosyaları açılabilmelidir.
   Artifact görevi `PublishBuildArtifacts@1` kullanır; böylece Azure
   DevOps **Classic Releases** ile de uyumludur.
4. Azure DevOps → **Pipelines → Environments** bölümünde
   `KLMN-Test` ve `KLMN-Production` oluştur. **Production →
   Approvals and checks → Approvals** ile en az bir onaylayıcı tanımla.
   Production aşamasını onaysız bırakma. Gerekirse exclusive lock ekle.
5. **Agent pools**: `KLMN-TEST-IIS`, `KLMN-PRODUCTION-IIS`.
   Her havuzda hedef Windows/IIS sunucusunda çalışan, ilgili IIS
   site/app pool dizinlerine erişimi olan bir self-hosted agent olmalı.
   ASP.NET Core 10 Hosting Bundle, .NET 10 runtime, IIS URL Rewrite,
   uygun TLS sertifikası ve izinler kurulu olmalıdır.
6. Azure DevOps → **Pipelines → Library → Variable groups**
   altında `KLMN-TEST` ve `KLMN-PRODUCTION` gruplarını oluştur.
   İki grupta da aşağıdaki değişkenler kendi ortama ait değerlerle dolmalı:

   | Değişken | Açıklama / örnek (gerçek sunucuna göre değiştir) |
   | --- | --- |
   | `KLMN_API_SITE` | IIS site ismi, örn. `KLMN-API-Test` |
   | `KLMN_API_APPPOOL` | API'ye özel app pool |
   | `KLMN_API_PATH` | API site physicalPath, örn. `D:\Sites\KLMN-Test\Api` |
   | `KLMN_WEB_SITE` | Angular IIS site ismi |
   | `KLMN_WEB_APPPOOL` | Angular'a özel app pool |
   | `KLMN_WEB_PATH` | Angular site physicalPath |
   
   Bu gruplara erişim iznini yalnız ilgili pipeline'a ver.
   IIS site physicalPath ile PATH değişkeni **birebir eşleşmelidir**.
   Script disk köküne ya da yanlış siteye yayın yapmaz.
7. Yeni pipeline oluştur: YAML `/KLMN/azure-pipelines-release.yml`.
   Kaynak pipeline resource adı `KLMN-CI` olduğu için bu isimde bir
   CI tanımı mevcut olmalı. Release pipeline'ın, variable group'ların,
   environment'ların ve artifact kaynağının izinlerini ver.
8. **Run pipeline** ile manuel başlat. Pipeline resource picker'dan başarılı
   **aynı CI run** sürümünü seç. Varsayılan
   `promoteToProduction=false`: **yalnız Test**. Test doğrulandıktan
   sonra `promoteToProduction=true` ile yeniden başlat; önce Test,
   ardından Production approval ve aynı artifact sürümünün dağıtımı
   uygulanır. Release için otomatik tetikleyici bilinçli olarak kapalıdır.

## Windows/IIS yayın ön koşulları

- API ve Web **ayrı IIS sitesi/app pool** olabilir; ikisini de aynı
  ortama yayınlayan release şablonu hazırdır. Test ve Prod yolları,
  hostname'leri ve kullanıcı izinleri farklı tutulmalıdır.
- Angular production kaynak `apiBaseUrl: '/api'` kullanıyor.
  Bu nedenle tarayıcıdan açılan frontend origin'inin `/api`
  isteğini **gerçek KLMN.Api**'ye yönlendiren aynı-origin reverse proxy
  (IIS ARR + URL Rewrite veya kurum gateway'i) **ayrıca kurulmalıdır**.
  Yalnızca iki IIS sitesi açmak bu yönlendirmeyi sağlamaz.
  Ayrı API origin planlanıyorsa frontend'in public HTTPS
  `apiBaseUrl`, backend CORS ve cookie politikaları birlikte
  güncellenmeli; URL gizli bilgi değildir.
- Angular'ın doğrudan yenilenen rota URL'leri için web.config SPA
  fallback kuralı içerir ve mevcut dosyaları korur; `/api`
  isteklerini index.html'e çevirmemeye dikkat eder.
- API'nin `Program.cs` başlangıçta **Database.MigrateAsync()** çalıştırır.
  Migration'ları ve PostgreSQL backup/geri dönüş planını onaylamadan
  uygulamayı Release ile başlatma. Bu pipeline migration oluşturmaz
  veya veritabanına doğrudan komut göndermez.
- İlk IIS API site dizini ve `appsettings.json` önce hazırlanmalı.
  Script `DocumentStorage:RootPath` gibi yüklenen PDF/belge dizinlerini
  **site physicalPath dışında** tutmayı gerektirir; site klasörü
  sürüm değişiminde yedek konuma taşınır.
- Script eski yayın klasörünü site yanında `*.backup.<timestamp>`
  olarak tutar. Bunların saklama, disk kotası, backup ACL ve temizleme
  politikasını Azure DevOps dışındaki operasyon ekibi belirlemelidir.
  Dağıtım, kesintisiz/blue-green değildir; site kısa süre durur.
  İki ayrı site iki ayrı işlemdir: API başarılı olup Web başarısızsa
  API'nin otomatik geri alındığını varsaymayın.
- Başarılı Release sonrası API auth, Angular `/api`, sayfa F5,
  JWT refresh cookie, CORS, dosya yükleme ve PostgreSQL bağlantısını
  gerçek Test ortamında doğrula. Pipeline'daki testler bu E2E doğrulamayı
  karşılamaz.

## Klasik (Classic) Releases tercih ediliyorsa

Azure DevOps → Pipelines → Releases → **New pipeline**:
1. **Artifacts → Add**: Source type **Build** → Source
   `KLMN-CI`, default version `Latest successful` veya
   elle seçilen run. Build'in **KLMN-API**, **KLMN-WEB**,
   **KLMN-DEPLOY** artifact'leri tek kaynak tanımının altından gelir.
2. **Test** ve **Production** stage oluştur, Production için
   pre-deployment approval koy. Her stage'in agent job'u o ortamdaki
   Windows/IIS sunucusunun self-hosted agent'ında çalışmalıdır.
3. Her stage içine API ve Web için ayrı **PowerShell** task ekle.
   Script, `KLMN-DEPLOY/deploy-iis.ps1` artifact'inden seçilir.
   API için parametreler:
   `-Kind Api -PackagePath "<indirilen KLMN-API/KLMN.Api.zip yolu>"
   -SiteName "<ortam API sitesi>" -AppPoolName "<API havuzu>"
   -TargetPath "<API physicalPath>"`.
   Web için `-Kind Web` ve `KLMN-WEB/KLMN.Web.zip` kullan.
   Gerçek artifact dosyasını görevdeki dosya seçiciyle doğrula.
4. Aynı koşullar geçerli: fiziksel site yolu önceden tanımlı, API
   `appsettings.json` server-side mevcut, Production onaylı.
   Classic Release ile YAML Release'i aynı sürüm için **eş zamanlı**
   çalıştırma. Classic pipeline Azure DevOps UI olmadan repodan
   otomatik oluşmaz.

## Gözlenen eksikler ve kapsam sınırı

- Repoda halen `package-lock.json` yok. CI, lockfile varsa
  `npm ci`, yoksa `npm install` yapar ve uyarı verir. Uzun vadede
  aynı bağımlılık sürümlerini garanti etmek için lockfile commit edilmeli.
- Azure DevOps organizasyon/proje URL'si, agent, IIS site ve gerçek
  hedef yolları bilinmediği için Release **gerçekten çalıştırılmadı**.
- Pipeline YAML'leri ve IIS deployment script'i kaynak kontrolüne
  eklendi; Azure DevOps'ta pipeline definition/Classic Release
  yaratma, var olmayan variable group'ları oluşturma, approval'ları
  tanımlama ve sunucuya kurulum işlemleri ayrıca yapılmalıdır.
