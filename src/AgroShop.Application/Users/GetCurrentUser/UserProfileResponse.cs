using AgroShop.Domain.Roles;

namespace AgroShop.Application.Users.GetCurrentUser;

public sealed record UserProfileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Patronymic,
    string Email,
    string Phone,
    Role Role);