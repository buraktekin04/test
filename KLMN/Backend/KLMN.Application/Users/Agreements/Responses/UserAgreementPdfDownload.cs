namespace KLMN.Application.Users.Agreements.Responses;

/// <summary>Korunan API yanıtı için PDF stream ve önerilen indirme adıdır.</summary>
/// <param name="Content">Diske ait salt okunur dosya akışıdır, yanıt tamamlandığında kapatılır.</param>
/// <param name="OriginalFileName">Kullanıcının gördüğü özgün dosya adıdır.</param>
public sealed record UserAgreementPdfDownload(Stream Content, string OriginalFileName);
