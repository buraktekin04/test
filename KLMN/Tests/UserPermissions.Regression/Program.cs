using KLMN.Application.Users.Commands.UpdateUserPermissionSettings;
using KLMN.Application.Users.Queries.GetUserPermissionSettings;
using KLMN.Domain.Entities.Identity;
static void Check(bool valid,string name){if(!valid)throw new Exception(name);Console.WriteLine("PASS "+name);}
var permissionId=Guid.NewGuid();
var userId=Guid.NewGuid();
var validator=new UpdateUserPermissionSettingsCommandValidator();
var valid=new UpdateUserPermissionSettingsCommand(userId,1,new[]{new UserPermissionChoice(permissionId,true)});
Check(validator.Validate(valid).IsValid,"Kişisel açık izin");
Check(validator.Validate(valid with{Choices=new[]{new UserPermissionChoice(permissionId,false)}}).IsValid,"Kişisel açık ret");
Check(validator.Validate(valid with{Choices=new[]{new UserPermissionChoice(permissionId,(bool?)null)}}).IsValid,"Rolden devral");
Check(!validator.Validate(valid with{UserId=Guid.Empty}).IsValid,"Kullanıcı kimliği zorunlu");
Check(!validator.Validate(valid with{Choices=new[]{new UserPermissionChoice(Guid.Empty,true)}}).IsValid,"Permission kimliği zorunlu");
Check(!validator.Validate(valid with{Choices=new[]{new UserPermissionChoice(permissionId,true),new UserPermissionChoice(permissionId,false)}}).IsValid,"Tekrarlı permission");
Check(typeof(UserPermission).GetProperty("IsGranted")?.PropertyType==typeof(bool),"Mevcut UserPermission bool override şeması");
Check(new GetUserPermissionSettingsQuery(userId).UserId==userId,"Kullanıcı permission sorgusu");
Console.WriteLine("User permission validator regresyon kontrolleri tamamlandı.");
