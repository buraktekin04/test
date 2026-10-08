using FluentValidation;
using MediatR;

namespace KLMN.Application.Common.Behaviors;

/// <summary>MediatR request'lerini FluentValidation ile merkezi olarak doğrular.</summary>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Bu istek tipine uygulanacak FluentValidation doğrulayıcılarının koleksiyonudur.
    /// </summary>
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// validation behavior işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// İlgili MediatR isteğini doğrulama ve iş kurallarından geçirerek yanıtını oluşturur.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        // Doğrulanacak MediatR isteğini FluentValidation'a taşıyan bağlamdır.
        var context = new ValidationContext<TRequest>(request);

        // results değerini ilgili iş kuralını uygulamak için hesaplar.
        var results = await Task.WhenAll(
            _validators.Select(x => x.ValidateAsync(context, cancellationToken)));

        // İsteğin reddedilmesine yol açan alan bazlı doğrulama hatalarıdır.
        var failures = results
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
