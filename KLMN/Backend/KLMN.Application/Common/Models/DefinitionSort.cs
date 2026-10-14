using System.Linq.Expressions;
using KLMN.Domain.Common;

namespace KLMN.Application.Common.Models;

/// <summary>Yalnızca izin verilen kolonları SQL'de sıralar; eşit değerleri Id ile sabitler.</summary>
public sealed class DefinitionSort<T>(IReadOnlyDictionary<string, Expression<Func<T, object?>>> fields)
    where T : BaseEntity
{
    public bool Supports(string? field) => field is not null && fields.ContainsKey(field);

    public IOrderedQueryable<T> Apply(IQueryable<T> query, string field, int order)
    {
        if (!Supports(field) || (order != 1 && order != -1))
            throw new ArgumentException("Geçersiz sıralama alanı veya yönü.");
        return (order == 1 ? query.OrderBy(fields[field]) : query.OrderByDescending(fields[field]))
            .ThenBy(x => x.Id);
    }
}
