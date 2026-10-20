using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.ProcedureTypes.Responses;
using KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeDefs;
using KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeDefById;
using KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeOptions;
using KLMN.Application.ProcedureTypes.Commands.CreateProcedureTypeDef;
using KLMN.Application.ProcedureTypes.Commands.UpdateProcedureTypeDef;
using KLMN.Application.ProcedureTypes.Commands.ChangeStatusProcedureTypeDef;
using KLMN.Application.ProcedureTypes.Commands.DeleteProcedureTypeDef;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Tetkik Türü Tanımları; bağımsız izinler, dinamik seçenekler, CRUD ve Excel.</summary>
[ApiController]
[Route("api/procedure-types")]
public sealed class ProcedureTypesController(ISender sender,IExcelExportService excel):ControllerBase
{
    [HttpGet]
    [HasPermission(PermissionCodes.ProcedureTypes.View,PermissionCodes.ProcedureTypes.Query,
      PermissionCodes.ProcedureTypes.Create,PermissionCodes.ProcedureTypes.Update,PermissionCodes.ProcedureTypes.Delete)]
    public async Task<IActionResult> List([FromQuery] GetProcedureTypeDefsQuery r,CancellationToken ct)=>Ok(await sender.Send(r,ct));

    /// <summary>Tetkik formu, başka modülün yetkisini gerektirmeden tür seçebilir.</summary>
    [HttpGet("options")]
    [HasPermission(PermissionCodes.ProcedureTypes.View,PermissionCodes.ProcedureTypes.Query,
      PermissionCodes.ProcedureTypes.Create,PermissionCodes.ProcedureTypes.Update,
      PermissionCodes.Procedures.View,PermissionCodes.Procedures.Query,
      PermissionCodes.Procedures.Create,PermissionCodes.Procedures.Update)]
    public async Task<IActionResult> Options([FromQuery]int? selectedCode,CancellationToken ct)=>
       Ok(await sender.Send(new GetProcedureTypeOptionsQuery(selectedCode),ct));

    [HttpGet("export")]
    [HasPermission(PermissionCodes.ProcedureTypes.Query)]
    public async Task<IActionResult> Export([FromQuery] GetProcedureTypeDefsQuery query,CancellationToken ct)
    {
        var first=await sender.Send(query with{PageNumber=1,PageSize=100},ct);
        if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="En fazla 5.000 kayıt indirilebilir."});
        var rows=new List<ProcedureTypeDefResponse>(first.Items);
        for(int page=2;page<=(int)Math.Ceiling(first.TotalCount/100d);page++)
           rows.AddRange((await sender.Send(query with{PageNumber=page,PageSize=100},ct)).Items);
        ExcelColumn<ProcedureTypeDefResponse>[] cols=[
           new("Kod",x=>x.Code,Width:12),new("Tetkik Türü",x=>x.Name,Width:35),
           new("Durum",x=>x.IsActive?"Aktif":"Pasif",Width:18)];
        return File(excel.Export(rows,cols,"Tetkik Türü Tanımları"),
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Tetkik_Turleri.xlsx");
    }
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.ProcedureTypes.View,PermissionCodes.ProcedureTypes.Query,
       PermissionCodes.ProcedureTypes.Create,PermissionCodes.ProcedureTypes.Update,PermissionCodes.ProcedureTypes.Delete)]
    public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetProcedureTypeDefByIdQuery(id),ct));

    [HttpPost]
    [HasPermission(PermissionCodes.ProcedureTypes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateProcedureTypeDefCommand r,CancellationToken ct)
    {
       var item=await sender.Send(r,ct);
       return CreatedAtAction(nameof(ById),new{id=item.Id},item);
    }
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.ProcedureTypes.Update)]
    public async Task<IActionResult> Update(Guid id,[FromBody] UpdateProcedureTypeDefCommand r,CancellationToken ct)
    {
       if(id!=r.Id)return BadRequest("Kayıt kimliği uyuşmuyor.");
       return Ok(await sender.Send(r,ct));
    }
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.ProcedureTypes.Update)]
    public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusProcedureTypeDefCommand r,CancellationToken ct)
    {
       if(id!=r.Id)return BadRequest("Kayıt kimliği uyuşmuyor.");
       return Ok(await sender.Send(r,ct));
    }
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.ProcedureTypes.Delete)]
    public async Task<IActionResult> Delete(Guid id,[FromQuery]uint version,CancellationToken ct)
    {
       await sender.Send(new DeleteProcedureTypeDefCommand(id,version),ct);
       return NoContent();
    }
}
