using FluentValidation;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Commands.CreateDispatch;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Models;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Commands.CreateBulkDispatch;

/// <summary>
/// Her kaydı farklı DI/DbContext scope'unda işler. Hatalı satır diğer satırlara
/// dokunmaz; kaydedilmiş satırlar geri alınmaz. Tekli sevkin iş kuralları tekrar kullanılır.
/// </summary>
public sealed class CreateBulkDispatchCommandHandler(
    IServiceScopeFactory scopes,
    ILogger<CreateBulkDispatchCommandHandler> logger)
    : IRequestHandler<CreateBulkDispatchCommand, BulkDispatchResponse>
{
    public async Task<BulkDispatchResponse> Handle(CreateBulkDispatchCommand request, CancellationToken ct)
    {
        var rows = new List<BulkDispatchRowResponse>(request.Items.Count);
        for (var i = 0; i < request.Items.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            var input = request.Items[i];
            var success = false;
            Guid? dispatchId = null;
            long? dispatchNo = null;
            var message = "Sevk kaydedilemedi.";

            // Başarısız SaveChanges ardından kirli change tracker sonraki kayda taşınmasın.
            await using (var scope = scopes.CreateAsyncScope())
            {
                try
                {
                    // Eski birleşik SenderChair GUID'sini aynı GUID ile doğru mevcut FK'ye koy.
                    // Hiçbir eşleşme yoksa veya iki tabloda da varsa tahmin/yeni GUID üretme.
                    if (input.LegacySenderChairId is { } legacyId)
                    {
                        if (input.Dispatch.SenderMilitaryUnitDefId.HasValue ||
                            input.Dispatch.SenderMilitaryOfficeDefId.HasValue)
                            throw new DispatchConflictException(
                                "Gönderen makam hem eski birleşik hem yeni alanlarla gönderilemez.");

                        var db = scope.ServiceProvider.GetRequiredService<IKLMNDbContext>();
                        var unitExists = await db.MilitaryUnitDefs.AsNoTracking().AnyAsync(
                            x => x.Id == legacyId && x.IsActive && !x.IsDeleted,ct);
                        var officeExists = await db.MilitaryOfficeDefs.AsNoTracking().AnyAsync(
                            x => x.Id == legacyId && x.IsActive && !x.IsDeleted,ct);
                        if (unitExists == officeExists)
                            throw new DispatchConflictException(
                                unitExists
                                    ? "Gönderen makam GUID'si birlik ve şube tablolarında birden bulundu; eşleştirme belirsiz."
                                    : "Eski gönderen makam GUID'si aktif birlik veya askerlik şubesi kayıtlarında bulunamadı.");
                        if (unitExists) input.Dispatch.SenderMilitaryUnitDefId = legacyId;
                        else input.Dispatch.SenderMilitaryOfficeDefId = legacyId;
                    }

                    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                    var result = await sender.Send(new CreateDispatchCommand(input.Person,input.Dispatch),ct);
                    success = true;
                    dispatchId = result.Id;
                    dispatchNo = result.DispatchNo;
                    message = "Sevk başarıyla kaydedildi.";
                }
                catch (ValidationException e)
                {
                    message = e.Errors.FirstOrDefault()?.ErrorMessage ?? "Sevk bilgileri geçersiz.";
                }
                catch (DispatchConflictException e)
                {
                    message = e.Message;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception e)
                {
                    // Teknik exception içeriğini ve kişisel verileri API yanıtına/loglara yazmayız.
                    logger.LogWarning("Toplu sevk satırı {RowNumber} başarısız; hata türü: {ErrorType}",
                        i+1,e.GetType().Name);
                    message = "İşlem sırasında teknik bir hata oluştu; sistem yöneticisine başvurunuz.";
                }
            }

            rows.Add(new BulkDispatchRowResponse
            {
                RowNumber = i + 1,
                IdentityMasked = MaskIdentity(input.Person),
                Name = input.Person.Name,
                Surname = input.Person.Surname,
                DispatchId = dispatchId,
                DispatchNo = dispatchNo,
                Success = success,
                ResultCode = success ? 0 : 1,
                Message = message
            });
        }

        return new BulkDispatchResponse
        {
            TotalCount = rows.Count,
            SuccessCount = rows.Count(x => x.Success),
            FailureCount = rows.Count(x => !x.Success),
            Rows = rows
        };
    }

    private static string MaskIdentity(DispatchPersonInput person)
    {
        var value = person.TcKimlikNo?.Trim() ?? person.YabanciKimlikNo?.Trim();
        if (string.IsNullOrEmpty(value)) return "—";
        return value.Length > 4
            ? new string('*',value.Length-4) + value[^4..]
            : "****";
    }
}
