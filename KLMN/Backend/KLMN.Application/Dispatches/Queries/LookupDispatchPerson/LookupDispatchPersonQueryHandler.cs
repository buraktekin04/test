using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using MediatR;

namespace KLMN.Application.Dispatches.Queries.LookupDispatchPerson;

/// <summary>
/// Kurumun nüfus kimlik sorgulama entegrasyonu için ayrılmış handler.
/// HİÇBİR ŞEKİLDE Persons tablosundan veya sahte kişilerden sonuç üretmez.
/// </summary>
public sealed class LookupDispatchPersonQueryHandler
    : IRequestHandler<LookupDispatchPersonQuery,DispatchPersonResponse?>
{
    public Task<DispatchPersonResponse?> Handle(LookupDispatchPersonQuery request,CancellationToken ct)
    {
        // T.C. veya yabancı kimlik numarası ve doğum tarihi birlikte gönderilmelidir.
        // Harici servis bağlı olmasa da hatalı girişler servise iletilmez.
        var tc=request.TcKimlikNo?.Trim();
        var foreign=request.YabanciKimlikNo?.Trim();
        if(string.IsNullOrEmpty(tc)==string.IsNullOrEmpty(foreign))
            throw new ArgumentException("Yalnız bir kimlik numarası gönderiniz.");
        if(request.BirthDate==default || request.BirthDate>DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Geçerli doğum tarihi giriniz.");
        if(tc is not null && (tc.Length!=11 || tc[0]=='0' || tc.Any(x=>!char.IsAsciiDigit(x))))
            throw new ArgumentException("Geçerli 11 haneli T.C. kimlik numarası giriniz.");
        if(foreign is not null && (foreign.Length<11 || foreign.Length>20 || foreign.Any(x=>!char.IsAsciiDigit(x))))
            throw new ArgumentException("Yabancı kimlik numarası 11-20 haneli sayısal olmalıdır.");

        // TODO: KURUMSAL NÜFUS / MERNİS ENTEGRASYONUNU BURADA BAĞLAYIN.
        // 1) ct ile harici kişi servisini çağırın; tc/foreign ve BirthDate alanlarını aktarın.
        // 2) Yanıtın kişi kimliğini/doğum tarihini doğrulayın. Birini bulamadıysa null dönebilirsiniz.
        // 3) Yanıtı DispatchPersonInput içine map edip new DispatchPersonResponse(person) döndürün.
        // 4) Kişinin ulusal kodlarını KLMN nationality/town GUID alanlarına eşleştirin;
        //    farklı kodları GUID gibi kullanmayın. Gerekirse BirthPlaceCode/Text döndürün.
        // 5) Kişisel verileri loglamayın, cachelemeyin veya bu handler'da veritabanına yazmayın.
        // Bu noktayı siz tamamlayana kadar 'bulunamadı' / sahte başarı yanıtı üretmiyoruz.
        throw new NationalRegistryNotConfiguredException();
    }
}
