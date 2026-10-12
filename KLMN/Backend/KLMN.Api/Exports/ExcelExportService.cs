using System.Globalization;
using ClosedXML.Excel;

namespace KLMN.Api.Exports;

/// <summary>
/// Her ekranın kendi veri modeli ve kolon tanımlarını kullanarak
/// aynı XLSX oluşturma mekanizmasından yararlanmasını sağlar.
/// </summary>
public interface IExcelExportService
{
    /// <summary>
    /// Kolon başlığı/sırası ve veri seçicileri endpoint'ten verilir.
    /// Workbook bytes üretir, diske dosya kaydetmez.
    /// </summary>
    byte[] Export<T>(
        IReadOnlyList<T> rows,
        IReadOnlyList<ExcelColumn<T>> columns,
        string worksheetName);
}

/// <summary>
/// Dinamik Excel sütununu temsil eder. Sabit DTO property isimlerine
/// veya reflection'a bağlı kalmadan her ekran istediği alanları seçer.
/// </summary>
/// <typeparam name="T">Ekranın export response/DTO tipidir.</typeparam>
/// <param name="Header">Excel'de görünen kolon başlığıdır.</param>
/// <param name="ValueSelector">Bir satırdan aktarılacak güvenli alanı okur.</param>
/// <param name="NumberFormat">İsteğe bağlı tarih/sayı biçimi; örn. dd.MM.yyyy.</param>
/// <param name="Width">İsteğe bağlı Excel kolon genişliğidir.</param>
public sealed record ExcelColumn<T>(
    string Header,
    Func<T, object?> ValueSelector,
    string? NumberFormat = null,
    double? Width = null);

/// <summary>
/// ClosedXML (MIT) kullanan uygulama genelindeki tek Excel üreticisidir.
/// Veri sorgulamaz ve ekranların hangi kolonları kullanacağına karar vermez.
/// Her istek için yeni workbook üreterek thread'ler arasında paylaşım yapmaz.
/// </summary>
public sealed class ExcelExportService : IExcelExportService
{
    /// <summary>Excel'in teknik olarak izin verdiği en çok sütun sayısıdır.</summary>
    private const int MaxColumns = 16_384;
    /// <summary>Başlık dahil Excel sayfasındaki en fazla satır sayısıdır.</summary>
    private const int MaxExcelRows = 1_048_576;

    /// <summary>
    /// Kolonları verilen sırada yazar, tarih/sayıları gerçek Excel veri tipinde
    /// tutar; tablo üstbilgisi, filtre ve dondurulmuş ilk satırı ayarlar.
    /// Kullanıcı verileri formül olarak değerlendirilmez.
    /// </summary>
    public byte[] Export<T>(
        IReadOnlyList<T> rows,
        IReadOnlyList<ExcelColumn<T>> columns,
        string worksheetName)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(columns);

        if (columns.Count == 0 || columns.Count > MaxColumns)
            throw new ArgumentException("Excel için 1-16384 arasında kolon tanımlanmalıdır.", nameof(columns));

        if (rows.Count + 1L > MaxExcelRows)
            throw new ArgumentException("Excel'in maksimum satır sayısı aşıldı.", nameof(rows));

        if (columns.Any(column =>
                string.IsNullOrWhiteSpace(column.Header) || column.ValueSelector is null))
        {
            throw new ArgumentException("Her Excel kolonunun başlığı ve veri seçicisi olmalıdır.", nameof(columns));
        }

        // Dışarıya sunulan Excel belge adları endpoint'ten gelen kontrollü isimlerdir.
        var safeSheetName = NormalizeWorksheetName(worksheetName);
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(safeSheetName);

        for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
        {
            var column = columns[columnIndex];
            var excelIndex = columnIndex + 1;
            var headerCell = sheet.Cell(1, excelIndex);
            headerCell.Value = column.Header;
            sheet.Column(excelIndex).Width = Math.Clamp(
                column.Width ?? (column.Header.Length + 5.0), 12.0, 60.0);
        }

        var header = sheet.Range(1, 1, 1, columns.Count);
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#243B53");
        header.Style.Font.FontColor = XLColor.White;
        header.Style.Font.Bold = true;
        header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        sheet.Row(1).Height = 25;

        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            var row = rows[rowIndex];

            for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                var definition = columns[columnIndex];
                var cell = sheet.Cell(rowIndex + 2, columnIndex + 1);

                // Seçiciler yalnızca ilgili ekranın belirlediği export DTO
                // üzerinden değer üretir; parolalar veya tokenlar yansıtılmaz.
                var value = definition.ValueSelector(row);
                WriteCell(cell, value, definition.NumberFormat);
            }
        }

        // Excel için veri aralığı birinci satırdaki başlıktan son satıra kadardır.
        // Boş tabloda sadece başlık üretilir; hata vermek yerine kullanılabilir dosya döner.
        if (rows.Count > 0)
            sheet.Range(1, 1, rows.Count + 1, columns.Count).SetAutoFilter();

        sheet.SheetView.FreezeRows(1);

        using var output = new MemoryStream();
        workbook.SaveAs(output);
        return output.ToArray();
    }

    /// <summary>
    /// Excel'in formül alanlarını istemci girdisinden türetmeden;
    /// metin, boolean, sayısal ve tarih hücrelerini yerel veri tiplerinde yazar.
    /// </summary>
    private static void WriteCell(IXLCell cell, object? value, string? numberFormat)
    {
        switch (value)
        {
            case null:
                cell.Value = string.Empty;
                break;

            case DateTimeOffset instant:
                cell.Value = instant.UtcDateTime;
                cell.Style.DateFormat.Format = numberFormat ?? "dd.MM.yyyy HH:mm";
                break;

            case DateTime date:
                cell.Value = date.Kind == DateTimeKind.Local
                    ? date.ToUniversalTime()
                    : date;
                cell.Style.DateFormat.Format = numberFormat ?? "dd.MM.yyyy HH:mm";
                break;

            case DateOnly dateOnly:
                cell.Value = dateOnly.ToDateTime(TimeOnly.MinValue);
                cell.Style.DateFormat.Format = numberFormat ?? "dd.MM.yyyy";
                break;

            case TimeOnly timeOnly:
                cell.Value = timeOnly.ToTimeSpan();
                cell.Style.DateFormat.Format = numberFormat ?? "HH:mm";
                break;

            case bool flag:
                cell.Value = flag;
                break;

            case byte or sbyte or short or ushort or int or uint or long or ulong
                or float or double or decimal:
                cell.Value = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                if (!string.IsNullOrWhiteSpace(numberFormat))
                    cell.Style.NumberFormat.Format = numberFormat;
                break;

            default:
                // Formüller asla set edilmez. '=' ile başlayan istemci verisi bile
                // hücre metni olarak kalır; formula injection oluşturulmaz.
                cell.Value = value.ToString() ?? string.Empty;
                break;
        }
    }

    /// <summary>
    /// Excel worksheet isminin teknik karakter ve 31 karakter sınırını uygular.
    /// </summary>
    private static string NormalizeWorksheetName(string? name)
    {
        var nameToUse = string.IsNullOrWhiteSpace(name) ? "Veriler" : name.Trim();
        var invalidCharacters = new[] { '[', ']', ':', '*', '?', '/', '\\' };
        foreach (var ch in invalidCharacters)
            nameToUse = nameToUse.Replace(ch, '_');

        nameToUse = nameToUse.Trim('\'', ' ');

        if (string.IsNullOrWhiteSpace(nameToUse))
            nameToUse = "Veriler";

        return nameToUse.Length > 31 ? nameToUse[..31] : nameToUse;
    }
}
