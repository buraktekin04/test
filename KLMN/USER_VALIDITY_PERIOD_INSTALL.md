# Kullanıcı geçerlilik tarihleri ve ortak kalıtım

Bu çalışma halihazırda bulunan Users.ValidFrom ve Users.ValidTo kolonlarını yeniden
oluşturmaz veya yeniden adlandırmaz. Entity üzerinde daha önce tanımlı olan iki alan,
EffectiveDatedEntity temel sınıfına taşınmıştır. User artık bu sınıftan türemektedir.

## Ortak Domain ve Persistence yapısı

- KLMN.Domain/Common/EffectiveDatedEntity.cs: BaseEntity'den kalıtım alır,
  ValidFrom ve ValidTo (DateTime? UTC) alanlarını ve IsValidAt kontrolünü taşır.
- KLMN.Domain/Common/ValidityPeriod.cs: başlangıç dahil, bitiş hariç
  [ValidFrom, ValidTo) dönemini ve sıralama kuralını merkezi denetler.
- KLMN.Persistence/Configurations/Common/EffectiveDatedEntityConfiguration.cs:
  iki alanı PostgreSQL timestamp with time zone olarak eşler.
- User : EffectiveDatedEntity
- UserConfiguration : EffectiveDatedEntityConfiguration<User>

Gelecekte tarih sınırları kullanılacak başka bir tanım için entity
EffectiveDatedEntity'den, konfigürasyonu da
EffectiveDatedEntityConfiguration<TEntity>'den kalıtım alabilir.
Ancak tarihe göre sorgu filtreleri ve iş kuralları her tanım için ayrıca
uygulanmalıdır. Ortak kalıtım tek başına tüm tanımları pasifleştirmez.

## Kullanıcı erişimi

LoginCommandHandler, RefreshTokenCommandHandler ve JWT OnTokenValidated,
ortak dönem kuralıyla hesap geçerliliğini denetler. Geçersiz dönemde yeni token
üretilmez; mevcut tokenlı yeni istek de açıklayıcı HTTP 403 ile reddedilir.
Angular login ekranında backend'in dönem hatası gösterilir.

- Başlangıç: dahil (ValidFrom <= şu an)
- Bitiş: hariç (şu an < ValidTo)
- Tarihler UTC tutulur. Angular datetime-local form değeri UTC ISO-8601'e çevrilir.
- İki alan null: sınırsız kullanım.
- Tek sınır null: yalnızca tanımlı sınır geçerlidir.
- İki sınır dolu: ValidTo > ValidFrom zorunludur.
- ClockLimit mesai kısıtı bu tarih kontrolünden bağımsız ve ek olarak uygulanır.

## Kopyalama ve migration

Var olan Users tablosunda ValidFrom ve ValidTo gerçekten mevcutsa bu çalışma
için yeni migration YOKTUR. UserConfiguration aynı mevcut kolonları eşler.
Veritabanında eksik alan varsa öncelikle mevcut şemayı ve migration geçmişini
kontrol edin; birbiriyle çakışan initial migration uygulamayın.

Yeni başka bir tanım tablosunda EffectiveDatedEntity kullanılmaya
başlanırsa o tablonun ValidFrom / ValidTo kolonlarının eklenmesi için
ilgili migration gerekir.

Angular kullanıcı formu, UserListItem ve Create/Update sözleşmeleri
ValidFrom ve ValidTo değerlerini taşır. Tarihler boş bırakılabilir;
bitiş başlangıçtan önce/eşit olamaz.

Geliştirme sırasında çalıştırılması gereken kontroller:
- dotnet build
- npm run build
- Başlangıç öncesi / aralık içi / bitiş sonrası login, refresh ve JWT testleri
- İstemcide iki boş, tek sınır ve ters tarih aralığı testleri

GitHub'a kaynak kod gönderildi; kullanıcının PostgreSQL veritabanına
doğrudan müdahale edilmedi.
