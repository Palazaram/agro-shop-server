namespace AgroShop.Application.Users.RefreshSession;

public sealed record RefreshSessionResponse(string AccessToken, string RefreshToken);