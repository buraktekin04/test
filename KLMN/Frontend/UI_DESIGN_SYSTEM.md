# KLMN Arayüz Tasarım Sistemi

## Amaç
KLMN'in sağlık, kontrol, takip ve raporlama süreçlerinde kullanılacak ekranlarını
tutarlı, erişilebilir ve kurumsal bir görünümle yönetmek. Bu düzenleme var olan
iş kurallarını, tablo filtrelerini, sıralamayı, CRUD akışlarını veya yetkileri değiştirmez.

## Dosya sorumlulukları
- `src/styles.scss`: **ortak CSS değişkenleri ve paylaşılan ekran kuralları**.
  Genel sayfa başlığı, filtre/tabloların yüzeyleri, form alanları, PrimeNG
  tamamlayıcı ölçüleri ve sabit İşlemler sütunu buradan yönetilir.
- `src/app/core/theme/klmn.preset.ts`: PrimeNG 21 tema token'ları.
  PrimeNG'in buton, input, checkbox, dialog vb. bileşenlerinin ana renkleri buradan gelir.
- `src/app/core/config/app-branding.ts`: uygulama adı, alt başlık, sürüm ve logo yolu.
- `src/assets/brand/klmn-logo.png`: geçici kurum simgesi.
- `src/app/layout/`: responsive sidebar, topbar ve ana içerik yerleşimi.
- `src/app/features/**\/*.component.scss`: **yalnızca ilgili ekrana özgü**
  form yerleşimleri, modal detayları ve özel iş akışları.

## Temel tasarım değişkenleri
| Amaç | CSS token | Varsayılan |
| --- | --- | --- |
| Ana işlem rengi | `--ui-primary` | `#405c74` |
| Sol menü (grafit) | `sidebar.component.scss` | `#29333d` |
| Sayfa zemini | `--ui-canvas` | `#edf1f4` |
| Kart zemini | `--ui-surface` | `#ffffff` |
| Başlık/metin | `--ui-text-strong` | `#1c2833` |
| İkincil metin | `--ui-muted` | `#647281` |
| Kenarlık | `--ui-border` | `#d3dae1` |
| Kart kavisleri | `--ui-radius` | `7px` |

Renk değiştirirken CSS token'ları ile PrimeNG preset'in `primary` paletini
aynı anda güncelleyin; aksi halde yerel öğeler ve PrimeNG öğeleri farklı renge dönebilir.
Farklı ekranlarda tekrar `#hex` tanımlamak yerine mevcut token'ları kullanın.

## Yeni tanım ekranı geliştirme kuralları
1. Mevcut `.page-header`, `.filter-panel`, `.table-scroll`, `.form-field`
   sınıflarını yeniden kullanın. Paylaşılan başlık stilini component dosyasına kopyalamayın.
2. Ekran özelinde farklı tablo yerleşimi, validasyon mesajı veya form gridi
   gerekirse yalnızca o ekranın SCSS'ine ekleyin.
3. Angular/PrimeNG iç yapısına dokunmayı gerektirmedikçe `::ng-deep` kullanmayın.
4. Tablo, modal, permission ve auth davranışlarının CSS değişikliklerinden
   etkilenmediğini kontrol edin; sadece görsel temayı paylaşın.

## Giriş sonrası nizamlı kurumsal UI standardı (16.10.2026)
- Askerî disiplin hissi veren sade, düzenli ve resmî görünüm.
  Gerçek kurum/rütbe amblemi kullanılmaz.
- Sol menü düz grafit (`#29333d`), açık gri metin, mat çelik
  aktif öğe ve ince sol çizgi. Gradient veya parıltı yoktur.
- Üst bar beyaz, ince alt kenarlık; Segoe UI/Tahoma/Arial
  fontları ve doğal harf aralığı kullanılır.
- Ana yüzey nötr gri (`#edf1f4`), kartlar beyaz, 7px
  köşeli ve ince çerçeveli. Ana sayfanın parlak hero gradient'i,
  pi-sparkles ve dönen büyük kalp dekoru kaldırıldı.
- Ortak `--ui-*` değişkenleri ve PrimeNG 21 Aura preset aynı
  mat çelik mavisi değerlerini kullanır.
- Tanım CRUD, filtre, audit, izin ve sıralama davranışları korunur.
- Onaylanmış login ekranı ayrı tutularak orijinal görsel renk
  token'ları `login.component.scss` içinde korunmuştur.

## Zorunlu PrimeNG p-table kolon standardı (19.10.2026)

Bundan sonra tüm listeleme p-table bileşenlerinde
`[resizableColumns]="true"` ve `columnResizeMode="expand"`
kullanılır; sabit `sticky-actions-col` hariç tüm
`th` başlıkları `pResizableColumn` alır.
Dikey ayırıcılar, görünür sürükleme çizgileri ve yatay tablo
kaydırması `src/styles.scss` dosyasından yönetilir.
Bu karar eski tüm listelere de uygulanmıştır. İşlemler kolonu
sticky kalır; sütun sıralaması yalnız frontend'tedir.

Bağlı Kurul Karar Tanımları'nda uzun Karar Adı satır
kaydırılarak eksiksiz gösterilir; ellipsis yapılmaz.

## Sidebar ve topbar
- **Masaüstü (genişlik > 992px):** Topbar menü butonu sidebar'ı daraltır/genişletir.
  Dar menüde ikonlar ve tooltip'ler kalır; grup ikonuna basınca menü açılır.
- **Mobil (genişlik <= 992px):** Menü soldan çekmece olarak açılır.
  Bağlantı, overlay veya Escape ile kapanır.
- Uygulama adı ve PNG logo yalnızca topbar'da büyük gösterilir.
  Sidebar altında sürüm bilgisi yer alır.

## Giriş ekranı
- Masaüstünde iki net bölge bulunur: solda düz, yumuşak antrasit-gri
  (`#434c55`) zemin üzerinde yalnızca ortalanmış büyük (176px)
  PNG logo ve iki yanında ince yatay çizgiler yer alır. Sol alanda
  uygulama adı veya açıklama metni bulunmaz.
- `KLMN` uygulama adı ve kısa `APP_BRANDING.subtitle` metni sağdaki
  beyaz giriş kartının ÜZERİNDE, kartla aynı hizada görünür.
- Gösterişli gradient, yoğun efekt, slogan veya tanıtım
  maddeleri kullanılmaz. Süsleme yalnızca iki zarif çizgidir.
- Sağ giriş kartı beyaz, ince kenarlıklı ve az gölgelidir.
  Segoe UI / Arial fontu ve tek koyu lacivert "Giriş Yap" butonu korunur.
- Dar ekranlarda sol logo alanı, çizgilerle birlikte daha kısa bir
  üst bölüme dönüşür (104px / küçük mobilde 94px logo).
  KLMN adı sağ form alanının üzerinde kalır; yatay taşma olmamalıdır.
- Sürüm numarası `APP_BRANDING.version` üzerinden alınır,
  sağ bölümün alt köşesinde küçük gösterilir.
- Kullanıcı adı/e-posta, parola göster/gizle, parola sıfırlama,
  alan doğrulama, yükleniyor ve sunucu hata/bilgi mesajları korunur.
- Başarılı çıkış, parola değişimi veya parola sıfırlama sonrası girişteki
  `informationMessage` bildirimi 3 saniye sonra otomatik kapanır.
  Yeni bilgilendirme önceki zamanlayıcıyı sıfırlar; bileşen
  kaldırıldığında zamanlayıcı temizlenir. Hata mesajlarının davranışı
  değişmez.

## Sürüm ve kontrol
Uygulama sürümünü güncellerken `package.json` ve
`src/app/core/config/app-branding.ts` değerlerini birlikte değiştirin.

Proje ortamında kontrol komutları:
```sh
npm ci
npm run build:dev
npm run build
```
Ardından mobil/masaüstü layout'u, login, kullanıcı hesabı menüsü,
tanım ekranlarının filtre/sıralama ve dialoglarını tarayıcıda doğrulayın.
