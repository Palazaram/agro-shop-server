namespace AgroShop.Api.ErrorHandling;

public sealed record ApiError(string? Field, string Code, string Message);