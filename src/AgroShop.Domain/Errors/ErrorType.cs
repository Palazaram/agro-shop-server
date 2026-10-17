namespace AgroShop.Domain.Errors;

public enum ErrorType 
{
    Validation,     // 422
    Unauthorized,   // 401
    Forbidden,      // 403
    NotFound,       // 404
    Conflict        // 409
}