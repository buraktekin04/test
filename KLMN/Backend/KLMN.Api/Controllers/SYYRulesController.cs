using KLMN.Api.Authorization;
using KLMN.Application.SYYRules.Queries.GetSYYRules;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Mevcut SYYDiagnosisDef tablosu üzerinden salt okunur Kural Tanımları ekranı.</summary>
[ApiController]
[Route("api/syy-rules")]
public sealed class SYYRulesController(ISender sender):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.SYYDiagnoses.View,PermissionCodes.SYYDiagnoses.Query)]
 public async Task<IActionResult> List([FromQuery] GetSYYRulesQuery query,CancellationToken ct)
   =>Ok(await sender.Send(query,ct));
}
