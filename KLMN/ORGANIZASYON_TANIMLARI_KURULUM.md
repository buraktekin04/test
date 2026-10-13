# KLMN — Organizasyon Tanımları

Kullanıcı Tanımları ekranındaki **Görev Yeri / Organizasyon** dropdown'ında seçilen
`OrganizationUnit` kayıtlarını yönetmek için oluşturulan tanım ekranıdır.
Bunlar **rol**, **permission** veya ayrıca bir **kullanıcı tipi enum'u** değildir.

## Ekran ve izinler

- Menü: Tanımlamalar → Organizasyon Tanımları
- Angular route: `/app/organizations`
- Listeleme ve Excel: `Organizations.Query`
- Oluşturma: `Organizations.Create`
- Düzenleme ve aktiflik: `Organizations.Update`
- Silme: `Organizations.Delete`
- İşlem geçmişi: `AuditLogs.View`

## API

| Metot | Endpoint | İşlem |
| --- | --- | --- |
| GET | `/api/organizations` | Sunucu tarafında filtrelenen/sayfalanan birimler |
| GET | `/api/organizations/export` | En çok 5.000 satırlık Excel |
| GET | `/api/organizations/parent-options` | Aktif üst birim seçenekleri |
| GET | `/api/organizations/{id}` | Güncel detay ve xmin |
| POST | `/api/organizations` | Yeni birim |
| PUT | `/api/organizations/{id}` | Birim/üst birim güncelleme |
| PATCH | `/api/organizations/{id}/status` | Aktif/pasif |
| DELETE | `/api/organizations/{id}?version={xmin}` | Soft delete |

## CQRS klasör yapısı

```text
KLMN.Application/Organizations/
├── Commands/
│   ├── Common/OrganizationUnitRules.cs
│   ├── CreateOrganizationUnit/
│   │   ├── CreateOrganizationUnitCommand.cs
│   │   ├── CreateOrganizationUnitCommandHandler.cs
│   │   └── CreateOrganizationUnitCommandValidator.cs
│   ├── UpdateOrganizationUnit/
│   │   ├── UpdateOrganizationUnitCommand.cs
│   │   ├── UpdateOrganizationUnitCommandHandler.cs
│   │   └── UpdateOrganizationUnitCommandValidator.cs
│   ├── ChangeOrganizationUnitStatus/
│   │   ├── ChangeOrganizationUnitStatusCommand.cs
│   │   ├── ChangeOrganizationUnitStatusCommandHandler.cs
│   │   └── ChangeOrganizationUnitStatusCommandValidator.cs
│   └── DeleteOrganizationUnit/
│       ├── DeleteOrganizationUnitCommand.cs
│       ├── DeleteOrganizationUnitCommandHandler.cs
│       └── DeleteOrganizationUnitCommandValidator.cs
├── Queries/
│   ├── GetOrganizationUnits/
│   │   ├── GetOrganizationUnitsQuery.cs
│   │   ├── GetOrganizationUnitsQueryHandler.cs
│   │   └── GetOrganizationUnitsQueryValidator.cs
│   ├── GetOrganizationUnitById/
│   │   ├── GetOrganizationUnitByIdQuery.cs
│   │   └── GetOrganizationUnitByIdQueryHandler.cs
│   └── GetOrganizationParentOptions/
│       ├── GetOrganizationParentOptionsQuery.cs
│       └── GetOrganizationParentOptionsQueryHandler.cs
├── Responses/ (3 DTO dosyası)
└── Exceptions/ (2 hata sınıfı)
```

## Kurallar

1. Organizasyon kodu, silinmemiş kayıtlar içinde **benzersizdir** ve
   `OrganizationUnitConfiguration` indeksini kullanır.
2. Yeni ve değişen üst birim atamalarında seçilen üst birim **aktif** olmak zorundadır.
   Çocuk-kendi / ata-altı şeklinde **hiyerarşi döngüsü** sunucuda yasaktır.
3. Bağlı **aktif** kullanıcı veya aktif alt birim varken birim pasifleştirilmez.
4. Bağlı (pasif dâhil, silinmemiş) kullanıcı veya alt birim varken birim **silinmez**.
5. Kayıt güncelleme, durum değiştirme ve silmede `Version` olarak PostgreSQL `xmin`
   istenir; başka bir güncellemeden sonra istek **409 Conflict** alır.
6. Organizasyon değişiklikleri mevcut `AuditableEntitySaveChangesInterceptor` üzerinden
   işlem geçmişine dâhil olur. Kullanıcıya organizasyon bağlama işlemi mevcut
   Kullanıcı Düzenleme ekranından devam eder.

## Manuel test sırası

1. ADMIN ile `/app/organizations` adresini açın.
2. Kök birim: `GENEL_MUDURLUK` / Genel Müdürlük ekleyin.
3. Alt birim: `IK_DAIRE` / İnsan Kaynakları Dairesi ekleyerek üst birim seçin.
4. Organizasyon listesinde adı, kodu, üst birimi, bağlı kullanıcı ve alt birim sayısını doğrulayın.
5. Kullanıcı Düzenleme içindeki görev yeri dropdown'ında yeni aktif birimi görebildiğinizi doğrulayın.
6. `GENEL_MUDURLUK` üstünü `IK_DAIRE` yapmayı deneyin: 409 beklenir.
7. Aynı kodu tekrar oluşturmayı deneyin: 409 beklenir.
8. Alt birimi olan kök birimi silmeyi ve pasifleştirmeyi deneyin: 409 beklenir.
9. Kullanıcısı olmayan yaprak birimi pasifleştirip yeniden aktifleştirin.
10. Liste filtrelerini, Excel'i, başka oturumdan xmin çatışmasını ve işlem geçmişini test edin.

## Kurulum

Yeni entity/kolon/tablo yoktur; **migration gerekmiyor**.
Backend ve frontend kaynaklarını klasörlerine kopyalayıp uygulamaları yeniden başlatın.
Angular ve .NET derlemesi ile gerçek tarayıcı testleri ayrıca yapılmalıdır.
