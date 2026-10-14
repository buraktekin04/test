# ADMIN Rolü: İlk Kurulum ve Sonradan Atama Kısıtı

- `ADMIN` sistem rolü ve tam yetki ilişkileri mevcut `IdentitySeeder` tarafından oluşturulur.
- İlk `ADMIN` rol ilişkisi veritabanına yazıldıktan sonra seeder tekrar yönetici hesabı/ataması oluşturmaz. `InitialAdmin` ayarları değiştirilse bile mevcut kurulumda ikinci bir yönetici açılmaz.
- Kullanıcı oluşturma ve güncelleme ekranlarının rol seçenekleri `ADMIN` içermez. Backend `UserRoleAssignment` da `ADMIN` rolünün yeni atamasını **yönetici dahil tüm kullanıcılara** reddeder.
- Mevcut sistemde daha önce başka kullanıcılara verilmiş `ADMIN` üyelikleri **otomatik silinmez**. Bir ADMIN kullanıcısı, **başka** bir kullanıcının ADMIN rolünü kaldırabilir; geriye en az bir aktif ADMIN kalmalıdır.
- Son aktif ADMIN'in rolünü kaldırmak, kullanıcının kendi rolünü değiştirmesi, ADMIN kullanıcısını silmek veya pasifleştirmek yasaktır.
- Mevcut yanlış atamaları düzeltmek için ilgili kişinin **Kullanıcı Düzenle** ekranında `ADMIN` seçimini kaldırıp yerine uygun normal rolü seçin, kaydedin. Sonrasında hesap standart silme/pasifleştirme kurallarıyla yönetilebilir.
- Bu değişiklik veritabanı migration'ı gerektirmez. Seed veya kullanıcı yönetimi üzerinden ikinci ADMIN atamaya çalışmayın.
- `KLMN/Tests/Users.Regression` içindeki handler regresyon kontrolleri bu senaryoları kapsar. PostgreSQL'le uçtan uca test ayrıca önerilir.
