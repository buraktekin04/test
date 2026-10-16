# KLMN Arayüz Tasarım Sistemi

## Amaç
KLMN'in sağlık, kontrol, takip ve raporlama süreçlerinde kullanılacak ekranlarını
tutarlı, erişilebilir ve kurumsal bir görünümle yönetmek. Bu düzenleme var olan
iş kurallarını, tablo filtrelerini, sıralamayı, CRUD akışlarını veya yetkileri değiştirmez.

## Dosya sorumlulukları
- \`src/styles.scss\`: **ortak CSS değişkenleri ve paylaşılan ekran kuralları**.
  Genel sayfa başlığı, filtre/tabloların yüzeyleri, form alanları, PrimeNG
  tamamlayıcı ölçüleri ve sabit İşlemler sütunu buradan yönetilir.
- \`src/app/core/theme/klmn.preset.ts\`: PrimeNG 21 tema token'ları.
  PrimeNG'in buton, input, checkbox, dialog vb. bileşenlerinin ana renkleri buradan gelir.
- \`src/app/core/config/app-branding.ts\`: uygulama adı, alt başlık, sürüm ve logo yolu.
- \`src/assets/brand/klmn-logo.png\`: geçici kurum simgesi.
- \`src/app/layout/\`: responsive sidebar, topbar ve ana içerik yerleşimi.
- \`src/app/features/**\/*.component.scss\`: **yalnızca ilgili ekrana özgü**
  form yerleşimleri, modal detayları ve özel iş akışları.

## Temel tasarım değişkenleri
| Amaç | CSS token | Varsayılan |
| --- | --- | --- |
| Ana işlem rengi | \`--ui-primary\` | \`#087f81\` |
| Ana menü | \`--ui-navy-deep\` | \`#0d253a\` |
| Sayfa zemini | \`--ui-canvas\` | \`#f2f6f8\` |
| Kart zemini | \`--ui-surface\` | \`#ffffff\` |
| Başlık/metin | \`--ui-text-strong\` | \`#172d3e\` |
| İkincil metin | \`--ui-muted\` | \`#718394\` |
| Kenarlık | \`--ui-border\` | \`#dce6e9\` |
| Kart kavisleri | \`--ui-radius\` | \`14px\` |

Renk değiştirirken CSS token'ları ile PrimeNG preset'in \`primary\` paletini
aynı anda güncelleyin; aksi halde yerel öğeler ve PrimeNG öğeleri farklı renge dönebilir.
Farklı ekranlarda tekrar \`#hex\` tanımlamak yerine mevcut token'ları kullanın.

## Yeni tanım ekranı geliştirme kuralları
1. Mevcut \`.page-header\`, \`.filter-panel\`, \`.table-scroll\`, \`.form-field\`
   sınıflarını yeniden kullanın. Paylaşılan başlık stilini component dosyasına kopyalamayın.
2. Ekran özelinde farklı tablo yerleşimi, validasyon mesajı veya form gridi
   gerekirse yalnızca o ekranın SCSS'ine ekleyin.
3. Angular/PrimeNG iç yapısına dokunmayı gerektirmedikçe \`::ng-deep\` kullanmayın.
4. Tablo, modal, permission ve auth davranışlarının CSS değişikliklerinden
   etkilenmediğini kontrol edin; sadece görsel temayı paylaşın.

## Sidebar ve topbar
- **Masaüstü (genişlik > 992px):** Topbar menü butonu sidebar'ı daraltır/genişletir.
  Dar menüde ikonlar ve tooltip'ler kalır; grup ikonuna basınca menü açılır.
- **Mobil (genişlik <= 992px):** Menü soldan çekmece olarak açılır.
  Bağlantı, overlay veya Escape ile kapanır.
- Uygulama adı ve PNG logo yalnızca topbar'da büyük gösterilir.
  Sidebar altında sürüm bilgisi yer alır.

## Sürüm ve kontrol
Uygulama sürümünü güncellerken \`package.json\` ve
\`src/app/core/config/app-branding.ts\` değerlerini birlikte değiştirin.

Proje ortamında kontrol komutları:
\`\`\`sh
npm ci
npm run build:dev
npm run build
\`\`\`
Ardından mobil/masaüstü layout'u, login, kullanıcı hesabı menüsü,
tanım ekranlarının filtre/sıralama ve dialoglarını tarayıcıda doğrulayın.
