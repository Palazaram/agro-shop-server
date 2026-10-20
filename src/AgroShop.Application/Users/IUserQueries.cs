using AgroShop.Application.Users.GetCurrentUser;
using CSharpFunctionalExtensions;

namespace AgroShop.Application.Users;

public interface IUserQueries
{
    Task<Maybe<UserProfileResponse>> GetProfileAsync(Guid userId, CancellationToken ct);
}