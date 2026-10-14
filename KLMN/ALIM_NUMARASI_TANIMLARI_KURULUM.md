# Alım Numarası Tanımları — KLMN

## Alanlar ve BaseEntity

Eski `PurchaseNumberDef.ObjectId` → `BaseEntity.Id`. `CreationDate/CreatedBy` →
`CreatedDate/CreatedBy`, `LastUpdate/UpdatedBy` → `UpdatedDate/UpdatedBy`,
`ConcurrencyToken` → PostgreSQL `xmin` / `BaseEntity.Version (uint)`,
`IsActive` → `BaseEntity.IsActive`. Tüm BaseEntity soft-delete/audit alanları ortaktır.
`PurchaseNumberDef : EffectiveDatedEntity : BaseEntity` yapısı kullanılır. Alım numarası tanımı `ValidFrom` ve `ValidTo` alanlarını yeniden oluşturmaz; bu nullable alanlar ortak `EffectiveDatedEntity` üzerinden gelir. Tarihler boşsa ilgili başlangıç/bitiş sınırı yoktur; ikisi de girilirse bitiş başlangıçtan sonra olmalıdır (başlangıç dahil, bitiş hariç). Entity kendi üzerinde yalnızca `Code`, `Name`, opsiyonel `HealthReportEndDate` ve zorunlu `SupplyDescriptionName` alanlarını tanımlar. Kod maksimum 100, ad 250, tedarik açıklaması 500 karakter.
Silinmemiş kayıtların kodu benzersizdir.

## Backend

- `GET /api/purchase-numbers`: arama, aktiflik ve tedarik açıklaması filtresi, sayfalama
- `GET /api/purchase-numbers/{id}`: güncel xmin dahil detay
- `POST /api/purchase-numbers`, `PUT /api/purchase-numbers/{id}`: Create/Update
- `PATCH /api/purchase-numbers/{id}/status`: aktif/pasif
- `DELETE /api/purchase-numbers/{id}?version=...`: soft delete
- `GET /api/purchase-numbers/export`: gerçek XLSX, en fazla 5000 kayıt

CQRS command/query/validator/handler dosyaları ayrıdır; EF ve xmin kuralları ortak
`PurchaseNumberDefRules` içindedir. `PurchaseNumbers.View/Query/Create/Update/Delete`
izinlerinden herhangi biri menüye ve liste/detaya erişim sağlar. Oluşturma, güncelleme,
silme izinleri ayrıca sunucuda doğrulanır. Excel yalnızca `Query` izniyle açıktır.

Audit interceptor'ında `PurchaseNumberDef` izinli değişiklik alanları tanımlandı.
`app-audit-history` aynı entityType ile çalışır.

## Frontend

`/app/purchase-numbers` ve `Tanımlamalar → Alım Numarası Tanımları`.
PrimeNG `p-table` kolonlarında **yalnızca ön yüzde sıralama** aktiftir
(mevcut sayfadaki kayıtlar üzerinde); filtreler ve sayfalama backend üzerindedir.
Ekleme/düzenleme formunda alan altı hata, tarih dönüşümü ve görünür kayıt hatası vardır.

## PostgreSQL migration — gerekli

Bu repoda EF Core `Migrations` snapshot'ı tutulmuyor; mevcut migration geçmişini
korumak adına elle sahte migration eklenmedi. **Uygulama ilk kez açılmadan önce**
mevcut yerel migration zincirine yeni entity'yi ekleyiniz:

```powershell
cd KLMN/Backend
dotnet ef migrations add AddPurchaseNumberDefs --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext --output-dir Migrations
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
```

Uygulama başlangıcındaki `Database.MigrateAsync()` migration'ı sonraki
ortamlarda otomatik uygular; ancak migration dosyasının kendisi oluşturulmuş
ve yayınlanmış olmalıdır. Ardından permission seeder yeni kodları yükler;
yeni izinler kullanıcılara/rollere ayrıca atanmalıdır (ADMIN tüm izinleri alır).

Dikkat: PostgreSQL `xmin` için ayrı `Version` kolonu oluşturulmamalıdır.
Var olan EF migrations snapshot'ını silmeyin veya sıfırdan başlatmayın.

## Test

```powershell
dotnet run --project KLMN/Tests/PurchaseNumbers.Regression/PurchaseNumbers.Regression.csproj
```

Gerçek PostgreSQL/Angular entegrasyon ve migration testi ayrıca yapılmalıdır.
