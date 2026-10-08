using FluentValidation;

namespace KLMN.Application.Users.Queries.GetUsers;

/// <summary>
/// Kullanıcı listeleme sorgusunun doğrulama kurallarını tanımlar.
/// </summary>
public sealed class GetUsersQueryValidator
    : AbstractValidator<GetUsersQuery>
{
    /// <summary>
    /// get users query validator işlemini ilgili güvenlik ve doğrulama kurallarına uygun yürütür.
    /// </summary>
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Search)
            .MaximumLength(200);
    }
}
