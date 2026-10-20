using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.CreateReasonForExamUnusedHcAdmissionType;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.UpdateReasonForExamUnusedHcAdmissionType;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.ChangeStatusReasonForExamUnusedHcAdmissionType;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.DeleteReasonForExamUnusedHcAdmissionType;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Queries.GetReasonForExamUnusedHcAdmissionTypes;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Queries.GetReasonForExamUnusedHcAdmissionTypeById;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Queries.GetReasonForExamUnusedHcAdmissionTypeOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Kullanılmayacak Hasta Grupları CRUD, Excel, permission ve xmin API uçları.</summary>
[ApiController]
[Route("api/reason-exam-unused-admission-types")]
public sealed class ReasonForExamUnusedHcAdmissionTypesController(ISender sender,IExcelExportService excel):ControllerBase
{
 /// <summary>Backend arama/sayfalama; modülün tek bir izni bile yeterlidir.</summary>
 [HttpGet]
 [HasPermission(PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.View, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Query, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Create, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Update, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Delete)]
 public async Task<IActionResult> List([FromQuery] GetReasonForExamUnusedHcAdmissionTypesQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 
 /// <summary>Yalnızca Query izni ile en fazla 5000 filtrelenmiş kayıt dışa aktarılır.</summary>
 [HttpGet("export")]
 [HasPermission(PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Query)]
 public async Task<IActionResult> Export([FromQuery] GetReasonForExamUnusedHcAdmissionTypesQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with {PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreyi daraltınız (en fazla 5000)."});
   var items=new List<ReasonForExamUnusedHcAdmissionTypeResponse>(first.Items);
   for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
   {
       var part=await sender.Send(q with {PageNumber=i,PageSize=100},ct);
       items.AddRange(part.Items);
   }
   var cols=new ExcelColumn<ReasonForExamUnusedHcAdmissionTypeResponse>[]{new("Muayene Sebebi",x=>x.ReasonDescription,Width:44),new("Hasta Grubu",x=>x.ReferenceName,Width:40),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(items,cols,"Kullanılmayacak Hasta Grupları"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "reason-exam-unused-admission-types_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 /// <summary>Dropdown seçimleri bu ekranın kendi izniyle döner.</summary>
 [HttpGet("options")]
 [HasPermission(PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.View, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Query, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Create, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Update)]
 public async Task<IActionResult> Options([FromQuery] Guid? selectedReasonId,[FromQuery] Guid? selectedReferenceId,CancellationToken ct)=>
 Ok(await sender.Send(new GetReasonForExamUnusedHcAdmissionTypeOptionsQuery(selectedReasonId,selectedReferenceId),ct));
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.View, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Query, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Create, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Update, PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetReasonForExamUnusedHcAdmissionTypeByIdQuery(id),ct));
 [HttpPost]
 [HasPermission(PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Create)]
 public async Task<IActionResult> Create([FromBody] CreateReasonForExamUnusedHcAdmissionTypeCommand q,CancellationToken ct)
 {
   var item=await sender.Send(q,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateReasonForExamUnusedHcAdmissionTypeCommand q,CancellationToken ct)
 {
   if(id!=q.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
   return Ok(await sender.Send(q,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusReasonForExamUnusedHcAdmissionTypeCommand q,CancellationToken ct)
 {
   if(id!=q.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
   return Ok(await sender.Send(q,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamUnusedHcAdmissionTypes.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteReasonForExamUnusedHcAdmissionTypeCommand(id,version),ct);
   return NoContent();
 }
}
