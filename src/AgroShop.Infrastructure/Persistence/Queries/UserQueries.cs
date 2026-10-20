using AgroShop.Application.Users;
using AgroShop.Application.Users.GetCurrentUser;
using AgroShop.Domain.Users;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace AgroShop.Infrastructure.Persistence.Queries;

internal sealed class UserQueries(AppDbContext context) : IUserQueries
{
    public async Task<Maybe<UserProfileResponse>> GetProfileAsync(Guid userId, CancellationToken ct)
    {
        User? user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
            return Maybe<UserProfileResponse>.None;

        return new UserProfileResponse(
            user.Id,
            user.FullName.FirstName.Value,
            user.FullName.LastName.Value,
            user.FullName.Patronymic.Value,
            user.Email.Value,
            user.Phone.Value,
            user.Role);
    }
}