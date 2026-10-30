using FluentValidation;

namespace KLMN.Application.Dispatches.Queries.LookupDispatchPerson;

/// <summary>Nüfus sistemi çağrısına geçmeden önce kimlik ve doğum tarihi doğrulaması.</summary>
public sealed class LookupDispatchPersonQueryValidator : AbstractValidator<LookupDispatchPersonQuery>
{
    public LookupDispatchPersonQueryValidator()
    {
        RuleFor(x=>x).Must(x=>
            !string.IsNullOrWhiteSpace(x.TcKimlikNo) != !string.IsNullOrWhiteSpace(x.YabanciKimlikNo))
            .WithMessage("T.C. veya yabancı kimlik numarasından yalnızca biri girilmelidir.");
        RuleFor(x=>x.TcKimlikNo).Must(x=>x is null||
            x.Length==11&&x[0]!='0'&&x.All(char.IsAsciiDigit))
            .WithMessage("T.C. kimlik numarası 11 haneli ve sayısal olmalıdır.");
        RuleFor(x=>x.YabanciKimlikNo).Must(x=>x is null||
            x.Length>=11&&x.Length<=20&&x.All(char.IsAsciiDigit))
            .WithMessage("Yabancı kimlik numarası 11-20 haneli ve sayısal olmalıdır.");
        RuleFor(x=>x.BirthDate).Must(x=>x!=default&&x<=DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Geçerli doğum tarihi giriniz.");
    }
}
