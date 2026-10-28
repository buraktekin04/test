# KLMN tanım göçü — kaynak kolon ve FK envanteri

`KLMN` projesinde **45 tanım ve eşleştirme hedef tablosu** belirlendi. `01_ORACLE_KAYNAK_KATALOG.sql` eski Oracle şemasına ait tablo/kolon/FK metadata'sını gösterir. Yalnız metadata okur; veri aktarmayı veya migration uygulamayı denemez.

Hedefin `mskbs` şemasında olduğu önceki DBViewer görüntülerinden biliniyor. Bağımsız Code/Name tabloları için `../04_Bagimsiz_Tanimlar` çalışma dosyaları hazır. Diğer gruplarda ilişkili eski sütunların gerçek adları kontrol edilmeden SQL üretmek veri bütünlüğü riski taşır. `02_TABLO_DURUMU.md` bütün 45 tabloyu ve gerekli ilişkileri listeler.

Oracle'da kolon envanterini XLS olarak alıp yalnız başlık/kısıt bilgisini paylaşırsanız, özel alanları ve ilişkileri de birebir doğrulayabiliriz. Kaynak tablo kodunu veya PK'yi tahmin ederek aktarım yapmayın.
