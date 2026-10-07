using FluentValidation;
using MediatR;

namespace KLMN.Application.Common.Behaviors;

/// <summary>
/// MediatR request'lerini handler çalışmadan önce FluentValidation ile doğrular.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Request validation pipeline adımını çalıştırır.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context =
            new ValidationContext<TRequest>(request);

        var validationResults =
            await Task.WhenAll(
                validators.Select(
                    validator =>
                        validator.ValidateAsync(
                            context,
                            cancellationToken)));

        var failures =
            validationResults
                .SelectMany(x => x.Errors)
                .Where(x => x is not null)
                .ToArray();

        if (failures.Length > 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
