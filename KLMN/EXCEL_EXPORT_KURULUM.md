# Dinamik Excel (.xlsx) Export — ClosedXML

## Hedef

Kullanıcı, Rol, Organizasyon ve gelecekteki diğer Tanımlamalar/Raporlar
ekranlarında ayrı ayrı Excel dosyası üreten kod yazılmaz.
Sadece o ekranın **DTO kolonları** ve **filtreli verileri** belirlenir.
Ortak servis Excel dosyasını oluşturur.

- NuGet: **ClosedXML 0.105.1**
- Lisans: **MIT** (ücretsiz/ticari kullanım imkânı; lisans bildirim koşullarını koruyun)
- NuGet adresi: https://www.nuget.org/packages/ClosedXML/0.105.1
- Kaynak: https://github.com/ClosedXML/ClosedXML

## Yeni ve değiştirilen dosyalar

Yeni:
`KLMN.Api/Exports/ExcelExportService.cs`

Güncellenen:
- `KLMN.Api/KLMN.Api.csproj` — NuGet PackageReference
- `KLMN.Api/Program.cs` — `IExcelExportService` DI kaydı
- `KLMN.Api/Controllers/UsersController.cs` — ekranın kolon listesi

Silinen eski dosya:
`KLMN.Api/Exports/UserExcelExporter.cs`

Angular kullanıcı ekranı, kullanıcı filtreleme komutları ve mevcut
`GET /api/users/export` URL'si değişmedi.

## Nasıl kullanılır?

Controller'a IExcelExportService inject edilir:

```csharp
public sealed class ExampleController(
    IExcelExportService excelExportService) : ControllerBase
{
    // ÖRNEK: Farklı ekranın ayrı DTO alanları olabilir.
    public sealed record ExampleRow(string Code, string Name, int Count);

    [HttpGet("export")]
    public IActionResult Export()
    {
        // Gerçek kullanımda veriler CQRS handler ve ilgili kullanıcının
        // read permission kontrolünden sonra alınmalıdır.
        var data = new List<ExampleRow>
        {
            new("BR001", "Birinci Birim", 7)
        };

        var columns = new ExcelColumn<ExampleRow>[]
        {
            new("Birim Kodu", x => x.Code, Width: 18),
            new("Birim Adı", x => x.Name, Width: 35),
            new("Personel Sayısı", x => x.Count, NumberFormat: "#,##0", Width: 20)
        };

        var bytes = excelExportService.Export(data, columns, "Organizasyonlar");
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Organizasyonlar.xlsx");
    }
}
```

Bu yalnızca kullanım örneğidir; örnek controller projeye eklenmemiştir.
Kendi endpoint'lerinde gerekli `[HasPermission(...)]` kontrolünü unutmayın.
**Kullanıcının gönderdiği rastgele alan adlarını reflection ile Excel'e
kopyalamayın.** Aktarılabilecek sütunlar her ekranın API kodunda belirlenmelidir.

## Desteklenen özellikler

- Değişken sayıda/isimde/sırada kolon, tip güvenli `ExcelColumn<T>`
- Boş/null metin, DateTime/DateOnly/DateTimeOffset, sayı, boolean, metin
- Excel üzerinde gerçek tarih/sayı tipleri, isteğe bağlı `NumberFormat`
- İsteğe bağlı sütun genişliği
- Koyu üst başlık, dondurulmuş ilk satır ve Excel otomatik filtre
- Her export çağrısında ayrı workbook: servis durum tutmaz
- Dosya diske kaydedilmez, yalnızca yetkili API yanıtı olarak gönderilir
- Satır ve kolon başlıkları/selector'ları backend tarafından belirlenir;
  parola hash'i, token veya PDF binary verisi otomatik aktarılmaz
- Formül özelliği kullanılmaz; kullanıcıdan gelen içerik düz metin olarak yazılır

## Kullanıcı ekranındaki mevcut davranış

`GET /api/users/export` halen kullanıcı listesiyle **aynı arama ve kolon
filtrelerini** kullanır. Bir sayfalık veri yerine filtreye uyan tüm satırları
alır. Bellek kullanımını sınırlamak için bu endpoint özelinde en fazla
5000 satır aktarılır; daha fazlaysa HTTP 400 döner. Bu limit ortak servisin
diğer ekranlara zorladığı bir sabit değildir; **endpoint'e özeldir**.

Yeni Excel servisinde kullanıcıya özel kolon isimleri sabit değildir.
Kullanıcı listesi 12 kolon kullanır; farklı ekran örneğin 3 veya 8 kolon
gönderdiğinde aynı servis farklı Excel üretebilir.

## Derleme ve kurulum

Güncellenmiş `KLMN.Api.csproj` içinde zaten
`<PackageReference Include="ClosedXML" Version="0.105.1" />` bulunur.

```powershell
cd KLMN/Backend
dotnet restore
dotnet build
```

Paket kapalı/offline kurumsal ortamda indirilemiyorsa NuGet paketi ve
bağımlılıklarının iç NuGet kaynağından alınması gerekir.

**Veritabanı şeması veya migration değişikliği yoktur.**
GitHub'daki kaynak doğrulanmıştır; gerçek .NET derlemesi ve tarayıcıdan
.xlsx açma testi çalıştırılacak ortamda doğrulanmalıdır.
