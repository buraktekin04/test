using KLMN.Application.ProcedureTypes.Commands.CreateProcedureTypeDef;
using KLMN.Application.ProcedureTypes.Commands.UpdateProcedureTypeDef;
using KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeDefs;
using KLMN.Application.ProcedureTypes.Responses;
using KLMN.Application.Procedures.Commands.CreateProcedureDef;
using KLMN.Application.Procedures.Commands.UpdateProcedureDef;
using KLMN.Application.Procedures.Responses;
using KLMN.Domain.Entities.Admissions;

static void Check(bool check,string name) {
    if(!check)throw new InvalidOperationException("FAIL: "+name);
    Console.WriteLine("PASS: "+name);
}
var type=new ProcedureTypeDef{Code=0,Name="Örnek Tür"};
var createType=new CreateProcedureTypeDefCommand(0,"Örnek Tür");
Check(new CreateProcedureTypeDefCommandValidator().Validate(createType).IsValid,"Enum code 0 can be stored");
Check(!new CreateProcedureTypeDefCommandValidator().Validate(createType with{Code=-1}).IsValid,"Negative code blocked");
Check(!new CreateProcedureTypeDefCommandValidator().Validate(createType with{Name=""}).IsValid,"Type name required");
Check(!new UpdateProcedureTypeDefCommandValidator().Validate(new(Guid.NewGuid(),0,"Yeni")).IsValid,"xmin required");
var mapped=ProcedureTypeDefResponse.Projection.Compile()(type);
Check(mapped.Code==0&&mapped.Name=="Örnek Tür","Dynamic type mapped");
Check(!new GetProcedureTypeDefsQueryValidator().Validate(new(PageSize:101)).IsValid,"Server paging max");
var from=new DateTime(2026,10,20,0,0,0,DateTimeKind.Utc);
var req=new CreateProcedureDefCommand("TET-1","Tam Kan",true,true,false,from,from.AddDays(20),0);
Check(new CreateProcedureDefCommandValidator().Validate(req).IsValid,"Old ProcedureDef fields accepted");
Check(!new CreateProcedureDefCommandValidator().Validate(req with{ProcedureCode=""}).IsValid,"ProcedureCode required");
Check(!new CreateProcedureDefCommandValidator().Validate(req with{EndDate=from}).IsValid,"StartDate EndDate bounds");
var updated=new UpdateProcedureDefCommand(Guid.NewGuid(),5,"TET-1","Tam Kan",true,true,true,from,from.AddDays(20),0);
Check(new UpdateProcedureDefCommandValidator().Validate(updated).IsValid,"Extended update accepted");
Check(!new UpdateProcedureDefCommandValidator().Validate(updated with{Version=0}).IsValid,"Stale version blocked");
var procedure=new ProcedureDef{ProcedureCode="TET-1",Name="Tam Kan",ProcedureType=0,
  TypeDefinition=type,RequiresNumericResult=true,RequiresReferenceRange=true,RequiresDeviationInfo=false,
  StartDate=from};
var dto=ProcedureDefResponse.Projection.Compile()(procedure);
Check(dto.ProcedureCode=="TET-1"&&dto.RequiresNumericResult&&dto.RequiresReferenceRange&&!dto.RequiresDeviationInfo,
  "Legacy boolean fields mapped");
Check(dto.ProcedureType==0&&dto.ProcedureTypeName=="Örnek Tür","Dynamic FK label mapped");
Check(dto.StartDate==from&&dto.EndDate==null,"Legacy dates mapped");
Console.WriteLine("Tetkik Türü/Tetkik regresyon kaynakları hazır.");
