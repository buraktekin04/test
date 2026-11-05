# KLMN – Toplu Sevk Girişi

## Kaynak ve ekran

Eski MSKBS'nin `test2` içindeki `newdispatchmultiple.component.html` /
`newdispatchmultiple.component.ts` dosyaları referans alındı. Eski ekran:
dosya seçimi, dosya içeriği JSON önizlemesi, "Sevkleri Kaydet" ve satır bazlı
kayıt/sonuç kodu tablosundan oluşuyordu.

KLMN karşılığı: **Sevk İşlemleri → Toplu Sevk Girişi**,
`/app/dispatches/bulk`. Yalnız `Dispatches.Create` izniyle görünür,
backend `POST /api/dispatches/bulk` üzerinde aynı izinle korunur.

## Dosya biçimi

Eski `.xml` input'u gerçek XML okuyup ayrıştırmıyordu: metni okuyor,
`JSON.parse` ve virgülde parçalama kullanıyordu. KLMN gerçek XML dosyasını
JSON diye kabul etmez. Doğrulanan biçimler:

- JSON nesne dizisi: `[{ "person": {...}, "dispatch": {...} }]`
- `{"items":[...]}` veya eski `{"sevkYeniGiris":[...]}`
- Satır bazlı JSON (JSONL) veya virgülle ayrılmış JSON nesneleri.
- Bilinen eski düz alan adları (`adi`, `soyadi`, `skBasvuruGrubu`,
  `muayeneSebebi`, `sevkKapsami`, `sevkKapsamiSehir/Hastane`) da
  karşılanır; eski `muayeneyeGonderenMakam` alanı türü belirsiz tek GUID
  ise otomatik birlik/şube tahmini **yapılmaz**. Bunun yerine
  `senderMilitaryUnitDefId`, `senderMilitaryOfficeDefId` veya
  `senderChairChoice: "unit:<GUID>" / "office:<GUID>"` kullanılır.

Aşağıdaki örnekte GUID'ler **yer tutucudur**; istekten önce KLMN'nin
gerçek ve aktif tanım kimlikleriyle değiştirilmelidir:

```json
{
  "items": [
    {
      "person": {
        "tcKimlikNo": "12345678901",
        "name": "Örnek",
        "surname": "Kişi",
        "birthDate": "1990-01-01"
      },
      "dispatch": {
        "hcAdmissionTypeDefId": "11111111-1111-4111-8111-111111111111",
        "reasonForExaminationDefId": "22222222-2222-4222-8222-222222222222",
        "dispatchScope": 2,
        "dispatchScopeCityId": "33333333-3333-4333-8333-333333333333",
        "senderMilitaryUnitDefId": "44444444-4444-4444-8444-444444444444"
      }
    }
  ]
}
```

`person` ve `dispatch` nesneleri mevcut `CreateDispatchCommand`
alanlarını kullanır. Tarihler `YYYY-MM-DD`, kimlik alanları string,
ilişkili tanım kimlikleri GUID olmalıdır. Sevk kapsamı yalnız
**2=Şehirdeki sağlık tesisleri** veya **3=Belirli sağlık tesisi**;
ilgili şehir/hastane alanı ve gönderen birlik veya şube kimliği zorunludur.

## Teknik tasarım

- İstemcide en fazla **1 MiB dosya** ve **100 satır**; HTTP API'de 2 MiB
  gövde sınırı. Dosyanın herhangi bir satırı yapısal olarak geçersizse,
  eksik/geçersiz satırlar sessizce atılmaz ve toplu kayıt engellenir.
- Önizleme ilk 5 geçerli kaydı gösterir, kimlik numaralarını maskeler.
- Satırların iş kuralları veritabanındaki başvuru grubu, muayene sebebi,
  gönderen makam ve tarihsel tanımlara göre **sunucuda** tekrar doğrulanır.
- `CreateBulkDispatchCommandHandler`, **her kayıt için ayrı DI scope**
  oluşturup mevcut `CreateDispatchCommand` üzerinden kaydeder.
  Böylece bir hata diğer satırların değişikliklerini etkilemez.
- Başarılı satırlar kalıcı, hatalı satırlar açıklamalı döner.
  İstemci aynı dosyayı otomatik yeniden göndermez. Ağ kesintisinde
  Sevk Listesi kontrol edilmeden yeniden denemek **çift sevk kaydı**
  yaratabilir; kalıcı import idempotency anahtarı bu fazda yoktur.
- Kimlik numaraları API sonuçlarında maskeli; teknik exception gövdesi
  ve kişisel bilgiler loglanmaz. Dış servis/MERNİS çağrısı uydurulmaz.
- **Veritabanı şeması değişmez, migration gerekmez.**

## Uyumluluk sınırı

Eski `SevkYeniGiris` ve `SevkYeniTopluGiris` TypeScript sınıfları
veya gerçek, kişisel bilgiler temizlenmiş örnek dosya `test2` içinde yok.
Bu nedenle eksiksiz eski veri şeması uyumluluğu **henüz doğrulanamaz**.
KLMN'nin mevcut `person`/`dispatch` sözleşmesi üzerinden toplu
giriş tamamlanmıştır. Bilinmeyen eski property'ler otomatik doğru alan
gibi varsayılmaz.

## Kontroller

```bash
cd KLMN/Frontend
npm run test:dispatch-bulk
npm run test:dispatch-bulk-flow
npm run test:dispatch-pages
npm run build

cd ../Backend
dotnet run --project ../Tests/Dispatches.Regression
dotnet build KLMN.slnx
```

Gerçek Angular/.NET derlemesi, PostgreSQL bütünleşik testi ve uçtan uca
dosya yükleme burada yapılmadı. İlk canlı test, kişisel veri içermeyen
ve gerçek KLMN tanım GUID'leriyle kurulmuş birkaç satırla yapılmalıdır.
