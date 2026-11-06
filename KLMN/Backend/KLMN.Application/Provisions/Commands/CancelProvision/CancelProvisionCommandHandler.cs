using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Provisions.Exceptions;
using KLMN.Application.Provisions.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Provisions.Commands.CancelProvision;

/// <summary>İptal geçmişini ayrı ProvisionState satırıyla saklar; silme yapmaz.</summary>
public sealed class CancelProvisionCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CancelProvisionCommand,ProvisionMutationResponse>
{
    public async Task<ProvisionMutationResponse> Handle(CancelProvisionCommand request,CancellationToken ct)
    {
        var provision=await db.Provisions.SingleOrDefaultAsync(
            x=>x.Id==request.Id&&!x.IsDeleted,ct)??throw new ProvisionNotFoundException();
        if(provision.Version!=request.Version)
            throw new DbUpdateConcurrencyException("Provizyon başka kullanıcı tarafından değiştirildi.");
        if(provision.CurrentState!=ProvisionStatusCodes.New)
            throw new ProvisionConflictException("Yalnız Yeni durumundaki provizyon iptal edilebilir.");
        provision.CurrentState=ProvisionStatusCodes.Cancelled;
        provision.Description=request.Reason.Trim();
        await db.ProvisionStates.AddAsync(new ProvisionState
        {
            ProvisionId=provision.Id,State=ProvisionStatusCodes.Cancelled
        },ct);
        await db.SaveChangesAsync(ct);
        return new ProvisionMutationResponse(provision.Id,provision.ProvisionNo,provision.DispatchId,provision.Version);
    }
}
