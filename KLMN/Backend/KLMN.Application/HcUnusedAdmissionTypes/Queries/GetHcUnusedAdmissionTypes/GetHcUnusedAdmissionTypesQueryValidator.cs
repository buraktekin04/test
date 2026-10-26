using FluentValidation;
namespace KLMN.Application.HcUnusedAdmissionTypes.Queries.GetHcUnusedAdmissionTypes;
/// <summary>Sayfa sınırlarını doğrular.</summary>
public sealed class GetHcUnusedAdmissionTypesQueryValidator:AbstractValidator<GetHcUnusedAdmissionTypesQuery>
{
 public GetHcUnusedAdmissionTypesQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,100);}
}
