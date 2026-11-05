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
- Eski düz MSKBS alanları (`UniqueRefno`, `Name`, `Surname`,
  `HcAdmissionTypeDef`, `ReasonForExamination`, `DispatchScopeProvince`,
  `ForcesCommand`, `Rank`, `MilitaryClass`, `MilitaryUnit`,
  `MilitarySchool`, `MilitaryOffice`, `FlyingMission`,
  `FlyingPlaneType`, `LastNominationDate` ve Türkçe frontend
  karşılıkları) yeniden dosya biçimi oluşturulmadan okunur.
- **İlişkili bütün GUID değerleri dosyadan geldiği gibi kalır.**
  `SenderChair` / `muayeneyeGonderenMakam` tek birleşik GUID alanı
  frontend'de `legacySenderChairId` olarak API'ye taşınır. Backend
  mevcut PostgreSQL `MilitaryUnitDefs` ve `MilitaryOfficeDefs`
  tablolarından **aynı GUID** ile sorgulayarak yalnızca eşleşen
  ilişkiye atar. İkisinde birden bulunursa veya hiçbirinde bulunmazsa
  başka GUID üretmeden satır hatası döndürür.
- İstenirse yeni `senderMilitaryUnitDefId` /
  `senderMilitaryOfficeDefId` alanları da kullanılabilir; eski
  birleşik GUID ve yeni gönderen makam aynı satırda birlikte olamaz.

Aşağıdaki örnek yalnızca **dokümantasyon için uydurulmuş test GUID'leri** içerir.
Gerçek MSKBS dosyasında zaten mevcut olan GUID'ler **aynen kullanılır**;
kullanıcı yeni GUID üretmez veya dosyayı yeniden düzenlemez. PostgreSQL'de
bu kimlikler doğru tablolarda aktarılmış olmalıdır:

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

## GUID koruma ve uyumluluk sınırı

- **Yeni tanım GUID üretilmez.** Eski dosyanın Başvuru Grubu,
  Muayene Sebebi, Kuvvet, Rütbe, Sınıf, Birlik, Okul, Askerlik Şubesi,
  Uçuş/İşlem ve Kapsam Şehir/Hastane GUID'leri aynı KLMN FK alanlarına geçer.
  Yeni *Sevk* ve yeni *Kişi* kayıtlarının kendi ID'leri olağan kayıt
  oluşturma davranışıyla ayrı üretilir.
- Gönderen Makam için eski `SenderChair` GUID'sinin birlik/şube
  ayrımı PostgreSQL üzerinden doğrulanır; eşleşme yoksa satır reddedilir.
- Eski MSKBS kod bazlı alanı (ör. `Nationality: "9980"`) bir GUID
  değildir; mevcut eşleşen GUID bilgisi veya onaylı kod→ID referansı
  olmadan **tahmini eşleştirme yapılmaz**. Kullanıcı sahte GUID üretmez.
- Eski `SevkYeniGiris` / `SevkYeniTopluGiris` TypeScript
  sınıfları ve gerçek, kişisel verileri temizlenmiş dosya `test2`
  içinde bulunmadığından bütün olası eski alanlar için **%100 veri
  dosyası uyumluluğu henüz doğrulanamaz**. Bilinen eski alanların
  eşleştirmesi ve birebir GUID koruma regresyon testleri eklendi. Gerçek
  dosya ve PostgreSQL entegrasyon testleri ayrıca yapılmalıdır.

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
