# KLMN — Askerî Sınıf Grubu ve Sınıf–Grup Eşleştirme

## Fotoğraftaki eski alanların yeni modele uyarlanması
MilitaryClassGroupDef: Code, Name, ShortName?, ForcesCommand (Guid),
IsActive, StartDate?, EndDate?. Yeni entity **EffectiveDatedEntity**:
Code, Name, ShortName?, ForcesCommandId; ValidFrom/ValidTo kalıtımdır.
ObjectId, audit, IsActive, soft-delete, Version (xmin) BaseEntity'dedir.

MilitaryClassGroupRelation: ForcesCommand, MilitaryClass,
MilitaryClassGroup (eski üçü de Guid), IsActive ve audit.
Yeni entity **BaseEntity**: ForcesCommandId, MilitaryClassId,
MilitaryClassGroupId ve navigasyonları. Bu ilişkide tarih alanı YOK.

## İş kuralları
Gruplar kuvvete özeldir; aynı silinmemiş kuvvette Code tekrar
tanımlanamaz. Eşleştirmede Kuvvet/Askerî Sınıf/Grup üçü de aktif
ve silinmemiş olmalı; grup aynı seçilen kuvvete ait olmalıdır.
Üçlü (ForcesCommandId, MilitaryClassId, MilitaryClassGroupId)
soft-delete olmayan kayıtlarda benzersizdir. Aynı sınıf, farklı
gruplarla eşleştirilebilir (ek kural belirtilene dek many-to-many).

İlişkili silinmemiş kayıt varsa kuvvet/sınıf/grup soft-delete
engellenir; aktif eşleştirme varsa sınıf/grup/kuvvet pasifleştirilemez.
Aktif grubu olan kuvvet de pasifleştirilemez. Grubun kuvveti,
gruba herhangi bir ilişki bağlıyken değiştirilemez. Aktifleştirme,
üst referansların aktifliğini yeniden denetler.
PostgreSQL ilişkileri DeleteBehavior.Restrict ile ikinci korumadır;
mükerrer kayıtları partial unique index engeller.

## Backend ve yetki
Gruplar: /api/military-class-groups
Eşleştirmeler: /api/military-class-group-relations
İkisinde GET (search/includeInactive/isActive/pageNumber/pageSize
ve ForcesCommandId), GET /{id}, GET /options, POST,
PUT /{id}, PATCH /{id}/status, DELETE /{id}?version=...,
GET /export (Query izni, en fazla 5000 kayıt).
Eşleştirme listesi ayrıca MilitaryClassId ve MilitaryClassGroupId
filtrelerini backend'de destekler.

Her modülün Create/Update/ChangeStatus/Delete kendi CQRS
Command/Handler/Validator klasörlerindedir; Query'ler ayrıdır.
Tüm değişiklikler uint Version (Npgsql xmin) ile korunur.
Hata kodları 404, 409 ve FluentValidation 400.
Audit interceptor iki entity için ayrı alan takibi yapar.

MilitaryClassGroups.View/Query/Create/Update/Delete ile
MilitaryClassGroupRelations.View/Query/Create/Update/Delete
izinleri birbirinden bağımsızdır. Modülün herhangi bir yetkisi
menü/route/listeyi açar; mutasyonlar yalnızca kendi izinleriyle,
Excel yalnızca Query izniyle yapılır. Dropdown'lar parent
modüllerin yönetim yetkisini vermez veya talep etmez.

## Angular UX
Tanımlamalar → Askerî Sınıf Grubu Tanımları
  /app/military-class-groups
Tanımlamalar → Sınıf–Grup Eşleştirmeleri
  /app/military-class-group-relations
Grup ekranında kod, ad, kısa ad, kuvvet ve nullable UTC geçerlilik
tarihi formu var. Eşleştirmede önce kuvvet seçilir; grup dropdown'u
yalnızca o kuvvetin gruplarıyla doldurulur, kuvvet değişirse önceki
grup seçimi sıfırlanır. Sınıf listesi ayrıca gösterilir.
Backend filtre ve sayfalama; PrimeNG p-table yalnızca yüklü sayfada
kolon sıralaması. İki ekranda da "Kolon Filtreleri" düğmesiyle
açılan/kapanan panel, "Tümünü Temizle" ve "Filtreleri Uygula"
bulunur. Grup ekranında Kuvvet Komutanlığı + Durum;
Eşleştirme ekranında Kuvvet Komutanlığı + Askerî Sınıf +
seçili kuvvete ait Sınıf Grubu + Durum filtreleri vardır.
Durum seçenekleri Tümü/Aktif/Pasif; ayrı ve gereksiz
"Aktif kayıtlar" seçeneği bulunmaz. Liste filtreleri için
sınıf/grup seçenekleri popup seçeneklerinden ayrı tutulur:
liste kuvveti değişirse grup temizlenir ve yeniden yüklenir.
Filtreler backend sorgu parametrelerine, aynı filtreler
Excel isteğine gönderilir. Sayfa numarası filtre uygulandığında
1'e döner. "Pasifleri dahil et" mevcut davranışını korur.
Inline validasyon/genel hata, Excel, audit, aktif/pasif,
silme ve mobil kaydırma davranışı desteklenir.

## Migration — MEVCUT SNAPSHOT KORUNMALI
GitHub deposunda yerel Migrations/ModelSnapshot tam zinciri YOK.
Önceden uygulanmış migration'ları veya tabloları sıfırlamayın.
Yerel gerçek zincirden iki yeni tablo için migration oluşturun:
MilitaryClassGroupDefs (nullable ValidFrom/ValidTo) ve
MilitaryClassGroupRelations (tarihsiz), üç FK Restrict,
soft-delete filtreli unique indeksler ve diğer FK indeksleri.
Önce ForcesCommandDefs ve MilitaryClassDefs var olmalıdır.
  cd KLMN/Backend
  dotnet ef migrations add AddMilitaryClassGroupsAndRelations --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
  dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
  dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
Normal Version sütunu açılmamalı: xmin concurrency token.
Daha önce hiç uygulanmamış migration yeni oluşturulurken
bu iki tabloyu kapsamalı; mevcut uygulama verilerini kaybetmeyin.

## Doğrulama
dotnet run --project KLMN/Tests/MilitaryClassGroups.Regression/MilitaryClassGroups.Regression.csproj
dotnet build KLMN/Backend/KLMN.slnx
npm --prefix KLMN/Frontend run build
Test kodları yazıldı; bu ortamda gerçek .NET/Angular build ve
PostgreSQL migration çalıştırılmadı. Gerçek DB'de eşzamanlı 409,
tekrar üçlü eşleştirme, farklı kuvvete ait grup, pasif üst kayıt,
soft-delete ve role/permission 403 akışları ayrıca test edilmeli.
