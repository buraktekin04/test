using System.IO.Compression;
using System.Text;
using System.Xml;
using KLMN.Application.Users.Responses;

namespace KLMN.Api.Exports;

/// <summary>
/// Kullanıcı Tanımları ekranı için başka paket gerektirmeden, Excel'in
/// açabileceği standart Open XML XLSX arşivini üreten ekran-özel yardımcıdır.
/// </summary>
internal static class UserExcelExporter
{
    /// <summary>SpreadsheetML şema adresidir.</summary>
    private const string SpreadsheetNs = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    /// <summary>Kullanıcı listeleme ekranındaki sütunların Excel başlıklarıdır.</summary>
    private static readonly string[] Headers =
    [
        "Ad Soyad", "Kullanıcı Adı", "E-posta", "Telefon",
        "Görev Yeri", "Roller", "Durum", "Kilit Durumu",
        "Mesai Dışı Erişim", "Geçerlilik Başlangıcı",
        "Geçerlilik Bitişi", "Oluşturulma"
    ];

    /// <summary>
    /// Kullanıcı verilerinden gerçek .xlsx formatında, filtrelenebilir ve başlığı
    /// sabitlenmiş Excel sayfasını oluşturur. Formül enjeksiyonunu önlemek için
    /// bütün kullanıcı girdilerini formül değil inline string hücresine yazar.
    /// </summary>
    public static byte[] Create(IReadOnlyList<UserListItemResponse> users)
    {
        using var stream = new MemoryStream();

        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            AddXml(zip, "[Content_Types].xml",
                """<?xml version="1.0" encoding="utf-8"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/><Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/></Types>""");

            AddXml(zip, "_rels/.rels",
                """<?xml version="1.0" encoding="utf-8"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/></Relationships>""");

            AddXml(zip, "xl/workbook.xml",
                """<?xml version="1.0" encoding="utf-8"?><workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets><sheet name="Kullanıcılar" sheetId="1" r:id="rId1"/></sheets></workbook>""");

            AddXml(zip, "xl/_rels/workbook.xml.rels",
                """<?xml version="1.0" encoding="utf-8"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/><Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/></Relationships>""");

            AddXml(zip, "xl/styles.xml",
                """<?xml version="1.0" encoding="utf-8"?><styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><fonts count="2"><font><sz val="11"/><name val="Calibri"/></font><font><b/><sz val="11"/><color rgb="FFFFFFFF"/><name val="Calibri"/></font></fonts><fills count="3"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill><fill><patternFill patternType="solid"><fgColor rgb="FF243B53"/><bgColor indexed="64"/></patternFill></fill></fills><borders count="1"><border><left/><right/><top/><bottom/><diagonal/></border></borders><cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs><cellXfs count="2"><xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/><xf numFmtId="0" fontId="1" fillId="2" borderId="0" xfId="0" applyFont="1" applyFill="1"/></cellXfs></styleSheet>""");

            // Satırlar XML writer üzerinden escape edilir; gizli access tokenlar,
            // parola hash'leri veya belge içerikleri Excel'e aktarılmaz.
            var sheet = zip.CreateEntry("xl/worksheets/sheet1.xml", CompressionLevel.Optimal);
            using var entryStream = sheet.Open();
            using var writer = XmlWriter.Create(entryStream, new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                CloseOutput = false
            });

            writer.WriteStartDocument();
            writer.WriteStartElement("worksheet", SpreadsheetNs);

            writer.WriteStartElement("sheetViews", SpreadsheetNs);
            writer.WriteStartElement("sheetView", SpreadsheetNs);
            writer.WriteAttributeString("workbookViewId", "0");
            writer.WriteStartElement("pane", SpreadsheetNs);
            writer.WriteAttributeString("ySplit", "1");
            writer.WriteAttributeString("topLeftCell", "A2");
            writer.WriteAttributeString("activePane", "bottomLeft");
            writer.WriteAttributeString("state", "frozen");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("cols", SpreadsheetNs);
            var widths = new[] { 26d, 19d, 32d, 19d, 32d, 25d, 12d, 16d, 20d, 23d, 23d, 23d };
            for (var i = 0; i < widths.Length; i++)
            {
                writer.WriteStartElement("col", SpreadsheetNs);
                writer.WriteAttributeString("min", (i + 1).ToString());
                writer.WriteAttributeString("max", (i + 1).ToString());
                writer.WriteAttributeString("width", widths[i].ToString(System.Globalization.CultureInfo.InvariantCulture));
                writer.WriteAttributeString("customWidth", "1");
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("sheetData", SpreadsheetNs);
            WriteRow(writer, Headers, 1, isHeader: true);

            for (var i = 0; i < users.Count; i++)
            {
                var user = users[i];
                var values = new[]
                {
                    user.FullName,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber ?? string.Empty,
                    user.OrganizationUnitName ?? string.Empty,
                    string.Join(", ", user.Roles),
                    user.IsActive ? "Aktif" : "Pasif",
                    user.IsLocked ? "Kilitli" : "Açık",
                    user.ClockLimit ? "İzin Yok" : "İzinli",
                    FormatDate(user.ValidFrom),
                    FormatDate(user.ValidTo),
                    FormatDate(user.CreatedDate)
                };

                WriteRow(writer, values, i + 2, isHeader: false);
            }
            writer.WriteEndElement(); // sheetData

            writer.WriteStartElement("autoFilter", SpreadsheetNs);
            writer.WriteAttributeString("ref", $"A1:L{users.Count + 1}");
            writer.WriteEndElement();
            writer.WriteEndElement(); // worksheet
            writer.WriteEndDocument();
        }

        return stream.ToArray();
    }

    /// <summary>ZIP arşivine XML metadata parçasını ekler.</summary>
    private static void AddXml(ZipArchive archive, string name, string xml)
    {
        var entry = archive.CreateEntry(name, CompressionLevel.Optimal);
        using var stream = entry.Open();
        using var writer = new StreamWriter(stream, new UTF8Encoding(false));
        writer.Write(xml);
    }

    /// <summary>Excel'deki bir satırı inline metin hücreleri olarak yazar.</summary>
    private static void WriteRow(XmlWriter writer, IReadOnlyList<string> values, int rowNumber, bool isHeader)
    {
        writer.WriteStartElement("row", SpreadsheetNs);
        writer.WriteAttributeString("r", rowNumber.ToString());

        for (var i = 0; i < values.Count; i++)
        {
            var letter = ((char)('A' + i)).ToString();

            writer.WriteStartElement("c", SpreadsheetNs);
            writer.WriteAttributeString("r", letter + rowNumber);
            writer.WriteAttributeString("t", "inlineStr");
            if (isHeader) writer.WriteAttributeString("s", "1");

            writer.WriteStartElement("is", SpreadsheetNs);
            writer.WriteStartElement("t", SpreadsheetNs);
            writer.WriteAttributeString("xml", "space", null, "preserve");
            // XML 1.0 denetim karakterleri üçüncü taraf verilerinden temizlenir.
            var value = new string((values[i] ?? string.Empty)
                .Where(XmlConvert.IsXmlChar).ToArray());
            writer.WriteString(value);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    /// <summary>UTC tarihini kullanıcıya okunabilir Excel metnine dönüştürür.</summary>
    private static string FormatDate(DateTime? value) =>
        value.HasValue ? value.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm 'UTC'") : "Sınırsız";
}
