using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.ReasonForExamBranchDefs.Responses;
using KLMN.Application.ReasonForExamBranchDefs.Commands.CreateReasonForExamBranchDef;
using KLMN.Application.ReasonForExamBranchDefs.Commands.UpdateReasonForExamBranchDef;
using KLMN.Application.ReasonForExamBranchDefs.Commands.ChangeStatusReasonForExamBranchDef;
using KLMN.Application.ReasonForExamBranchDefs.Commands.DeleteReasonForExamBranchDef;
using KLMN.Application.ReasonForExamBranchDefs.Queries.GetReasonForExamBranchDefs;
using KLMN.Application.ReasonForExamBranchDefs.Queries.GetReasonForExamBranchDefById;
using KLMN.Application.ReasonForExamBranchDefs.Queries.GetReasonForExamBranchDefOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Muayene Gereken Branşlar CRUD, Excel, permission ve xmin API uçları.</summary>
[ApiController]
[Route("api/reason-exam-branches")]
public sealed class ReasonForExamBranchDefsController(ISender sender,IExcelExportService excel):ControllerBase
{
 /// <summary>Backend arama/sayfalama; modülün tek bir izni bile yeterlidir.</summary>
 [HttpGet]
 [HasPermission(PermissionCodes.ReasonForExamBranchDefs.View, PermissionCodes.ReasonForExamBranchDefs.Query, PermissionCodes.ReasonForExamBranchDefs.Create, PermissionCodes.ReasonForExamBranchDefs.Update, PermissionCodes.ReasonForExamBranchDefs.Delete)]
 public async Task<IActionResult> List([FromQuery] GetReasonForExamBranchDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 
 /// <summary>Yalnızca Query izni ile en fazla 5000 filtrelenmiş kayıt dışa aktarılır.</summary>
 [HttpGet("export")]
 [HasPermission(PermissionCodes.ReasonForExamBranchDefs.Query)]
 public async Task<IActionResult> Export([FromQuery] GetReasonForExamBranchDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with {PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreyi daraltınız (en fazla 5000)."});
   var items=new List<ReasonForExamBranchDefResponse>(first.Items);
   for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
   {
       var part=await sender.Send(q with {PageNumber=i,PageSize=100},ct);
       items.AddRange(part.Items);
   }
   var cols=new ExcelColumn<ReasonForExamBranchDefResponse>[]{new("Muayene Sebebi",x=>x.ReasonDescription,Width:40),new("Branş",x=>x.ReferenceName,Width:40),new("Başlangıç Yaşı",x=>x.StartingAge),new("Bitiş Yaşı",x=>x.EndingAge),new("Cinsiyet Kodu",x=>x.Gender),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(items,cols,"Muayene Gereken Branşlar"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "reason-exam-branches_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 /// <summary>Dropdown seçimleri bu ekranın kendi izniyle döner.</summary>
 [HttpGet("options")]
 [HasPermission(PermissionCodes.ReasonForExamBranchDefs.View, PermissionCodes.ReasonForExamBranchDefs.Query, PermissionCodes.ReasonForExamBranchDefs.Create, PermissionCodes.ReasonForExamBranchDefs.Update)]
 public async Task<IActionResult> Options([FromQuery] Guid? selectedReasonId,[FromQuery] Guid? selectedReferenceId,CancellationToken ct)=>
 Ok(await sender.Send(new GetReasonForExamBranchDefOptionsQuery(selectedReasonId,selectedReferenceId),ct));
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamBranchDefs.View, PermissionCodes.ReasonForExamBranchDefs.Query, PermissionCodes.ReasonForExamBranchDefs.Create, PermissionCodes.ReasonForExamBranchDefs.Update, PermissionCodes.ReasonForExamBranchDefs.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetReasonForExamBranchDefByIdQuery(id),ct));
 [HttpPost]
 [HasPermission(PermissionCodes.ReasonForExamBranchDefs.Create)]
 public async Task<IActionResult> Create([FromBody] CreateReasonForExamBranchDefCommand q,CancellationToken ct)
 {
   var item=await sender.Send(q,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamBranchDefs.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateReasonForExamBranchDefCommand q,CancellationToken ct)
 {
   if(id!=q.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
   return Ok(await sender.Send(q,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.ReasonForExamBranchDefs.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusReasonForExamBranchDefCommand q,CancellationToken ct)
 {
   if(id!=q.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
   return Ok(await sender.Send(q,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamBranchDefs.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteReasonForExamBranchDefCommand(id,version),ct);
   return NoContent();
 }
}
