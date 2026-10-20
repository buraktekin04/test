using KLMN.Application.ProcedureTypes.Responses;
using MediatR;
namespace KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeOptions;
/// <summary>Tetkik formunun dinamik tür seçenekleri.</summary>
public sealed record GetProcedureTypeOptionsQuery(int? SelectedCode=null):IRequest<IReadOnlyList<ProcedureTypeOption>>;
