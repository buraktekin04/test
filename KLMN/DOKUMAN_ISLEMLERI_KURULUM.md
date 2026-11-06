# KLMN — Yönetim Paneli / Doküman İşlemleri

Yönetim Paneli > Doküman İşlemleri ile Sağlık Yeteneği Yönetmeliği
PDF'leri ayrı ayrı yüklenir, görüntülenir veya silinir.
Sol alt **Dokümanlar** açılır alanı yetkili kullanıcının birden fazla
PDF arasından seçim yapıp yeni sekmede görüntülemesini sağlar.

**Yetkiler:** DocumentPdfs.View — menü ve PDF görüntüleme.
DocumentPdfs.Manage — yönetim ekranında yükleme/silme ve görüntüleme.
İzinler rol ve kullanıcı bazında atanır. ADMIN tüm izinleri seed ile alır.

**API:** GET /api/document-pdfs, GET /api/document-pdfs/{id}/file,
POST /api/document-pdfs (multipart file), DELETE /api/document-pdfs/{id}?version=...
PDF içerikleri JWT'li GET ile Blob olarak alınır; statik dosya URL'si verilmez.

**DB:** DocumentPdfs yeni tablo. Id = eski DocumentPdf.ObjectId,
PdfDataObjectId = eski PDFDataObjectId. FileName, SizeBytes ve BaseEntity
audit/soft delete/xmin alanlarıyla. Binary özel dosya kökündeki
`DocumentStorage:RootPath/regulations/{PdfDataObjectId:N}.pdf` konumunda.
Mevcut ayarlardan DocumentStorage:MaxFileSizeMb limiti kullanılır; PDF
uzantısı ve %PDF- imzası denetlenir.

**Migration zorunlu:** Yeni DocumentPdfs tablosu için EF Core migration
oluşturulmalı ve uygulanmalıdır. Migration bu bağlantıda oluşturulmadı
ve uygulanmadı. Oracle'dan GUID'ler kesinlikle değiştirilmeden aktarılmalı.
Eski PDFDataObjectId'nin işaret ettiği gerçek PDF byte'ları da ayrıca
aynı GUID dosya adıyla taşınmalı; yalnız metadata yeterli değildir.
Eski long ConcurrencyToken PostgreSQL xmin yerine kopyalanmaz.
Eski veri blob/servis sözleşmesi henüz verilmediğinden aktarım yapılmadı.

**Test:** `npm --prefix KLMN/Frontend run test:document-pdfs`,
`npm --prefix KLMN/Frontend run build`,
`dotnet build KLMN/Backend/KLMN.slnx`.
Bu ortamda gerçek build, DB veya tarayıcı E2E uygulanmadı.
Global/ortak SCSS değişmedi, ekranın kendi HTML/TS/SCSS dosyaları var.

