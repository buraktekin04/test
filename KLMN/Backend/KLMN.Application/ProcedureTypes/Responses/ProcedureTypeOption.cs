using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ProcedureTypes.Responses;
/// <summary>Mevcut enum değerlerini taşımayı sağlayan kod ve ad seçeneği.</summary>
public sealed record ProcedureTypeOption(int Code,string Name,bool IsActive);
