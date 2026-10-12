# Kullanıcı Tanımları – dosya yerleştirme ve test notları

**KLMN Backend:** .NET 10 + EF Core 10 + CQRS / MediatR
**Frontend:** Angular 21 + PrimeNG 21

## Sol menü

`Tanımlamalar > Kullanıcı Tanımları` adresi: `/app/definitions/users`.
Eski `/app/users` adresi yenisine yönlendirilir.
Rol ve Organizasyon Tanımları menüleri şimdilik pasiftir; ekranları ilerleyen adımlarda yapılacaktır.

## Backend klasörleri

```text
KLMN.Application/Users/
  Commands/
    CreateUser/
      CreateUserCommand.cs
      CreateUserCommandValidator.cs
      CreateUserCommandHandler.cs
    UpdateUser/
      UpdateUserCommand.cs
      UpdateUserCommandValidator.cs
      UpdateUserCommandHandler.cs
    ChangeUserStatus/
      ChangeUserStatusCommand.cs
      ChangeUserStatusCommandValidator.cs
      ChangeUserStatusCommandHandler.cs
    DeleteUser/
      DeleteUserCommand.cs
      DeleteUserCommandValidator.cs
      DeleteUserCommandHandler.cs
  Queries/
    GetUsers/         (mevcut Query, Handler, Validator korunur)
    GetUserById/
      GetUserByIdQuery.cs
      GetUserByIdQueryHandler.cs
  Responses/
    UserListItemResponse.cs (mevcut)
    UserMutationResponse.cs
  Exceptions/
    UserNotFoundException.cs
    UserConflictException.cs
```

**Önemli:** `KLMN.Api/Controllers/UsersController.cs` ve
`KLMN.Api/ExceptionHandling/GlobalExceptionHandler.cs` de
güncellenmelidir. CRUD hataları 404/409 olarak dönmelidir.

## API endpoint sözleşmeleri

| Method | Endpoint | Permission | İşlev |
|---|---|---|---|
| GET | `/api/users` | `Users.Query` | Filtreli ve sayfalı kullanıcı listesi |
| GET | `/api/users/{id}` | `Users.Query` | Düzenleme için güncel kullanıcı detayları |
| POST | `/api/users` | `Users.Create` | Yeni kullanıcı ve STANDARD_USER rolü |
| PUT | `/api/users/{id}` | `Users.Update` | Temel profil alanlarını güncelle |
| PATCH | `/api/users/{id}/status` | `Users.Activate` | Aktif/pasif durumunu değiştir |
| DELETE | `/api/users/{id}?version={xmin}` | `Users.Delete` | Soft delete |

**Concurrency:** PUT/PATCH body içinde `id` ve `version`, DELETE
query string içinde `version` gönderir. Güncel olmayan versiyon HTTP
409 olur. Ayrıca PostgreSQL'in gerçek `xmin` concurrency denetimi
`SaveChanges` sırasında uygulanır.

**Güvenlik:** Parola yalnızca oluşturma endpoint'inde alınır ve
`IPasswordHasherService` üzerinden hash'lenir. ADMIN hesapları silinemez
ve pasifleştirilemez; mevcut kullanıcı kendi hesabını silemez veya
pasifleştiremez. Rol/permission değiştirme bu CRUD kapsamına alınmamıştır.

## Angular kopyalama

```text
src/app/
  app.routes.ts
  layout/
    services/navigation.service.ts
    sidebar/sidebar.component.ts
    sidebar/sidebar.component.html
    sidebar/sidebar.component.scss
  features/users/
    models/
      create-user-request.model.ts
      update-user-request.model.ts
      change-user-status-request.model.ts
      user-mutation-response.model.ts
    services/user.service.ts
    user-list/user-list.component.ts
    user-list/user-list.component.html
    user-list/user-list.component.scss
```

Angular'ın daha önce kullanılan `user-list-item.model.ts` ve
`user-query.model.ts` dosyaları korunur. Proxy gerekmez;
`API_BASE_URL` üzerinden mevcut environment dosyasına bağlanır.

## Test sırası

1. `dotnet build` ve `npm run build` komutlarını çalıştırın.
2. Yetkili ADMIN kullanıcıyla giriş yapın.
3. Tanımlamalar > Kullanıcı Tanımları menüsünü açın; liste/filtre/sayfalama yapın.
4. Yeni kullanıcı oluşturun; sunucuda STANDARD_USER rolü atandığını kontrol edin.
5. Düzenlemeden hemen önce getirilen `version` ile profil güncelleyin.
6. Kullanıcıyı pasifleştirip **Pasifleri dahil et** ile bulun; sonra aktifleştirin.
7. Soft delete ve 409 concurrency davranışlarını sınayın.
8. Yetkisi olmayan kullanıcının CRUD butonlarını göremediğini
   ve API'nin ayrıca 403 döndürdüğünü doğrulayın.

**Migration:** Var olan User, Role ve UserRole tabloları kullanılır;
bu özellik için yeni entity/kolon eklenmedi. Yeni migration gerekmez.

**Dikkat:** Bu repo **KLMN** namespace kullanır. Yerelde proje adı
**MSKBS** ise kopyalarken namespace ve klasör adlarını yerel
solution'ınızla eşleştirin; iki projeyi karıştırmayın.
