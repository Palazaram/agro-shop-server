using AgroShop.Application.Abstractions;
using AgroShop.Application.Options;
using AgroShop.Domain.Abstractions;
using AgroShop.Domain.Errors;
using AgroShop.Domain.RefreshTokens;
using AgroShop.Domain.Users;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace AgroShop.Application.Users.LoginUser;

public sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider,
    IRefreshTokenGenerator refreshTokenGenerator,
    IUnitOfWork unitOfWork,
    IJwtProvider jwtProvider,
    IOptions<RefreshTokenOptions> refreshTokenOptions)
    : ICommandHandler<LoginUserCommand, LoginUserResponse>
{
    public async Task<Result<LoginUserResponse, Error>> HandleAsync(LoginUserCommand command, CancellationToken ct)
    {
        var phone = Phone.Create(command.Phone).Value;

        var maybeUser = await userRepository.GetByPhoneAsync(phone, ct);
        if (maybeUser.HasNoValue)
            return DomainErrors.Users.PhoneNotRegistered();

        var user = maybeUser.Value;

        if (!passwordHasher.Verify(command.Password!, user.PasswordHash))
            return DomainErrors.Users.PasswordInvalid();

        var now = timeProvider.GetUtcNow();

        var generatedRefreshToken = refreshTokenGenerator.Generate();

        var refreshToken = RefreshToken.Create(
            user.Id,
            generatedRefreshToken.Hash,
            now,
            TimeSpan.FromDays(refreshTokenOptions.Value.LifetimeDays));

        refreshTokenRepository.Add(refreshToken);
        await unitOfWork.SaveChangesAsync(ct);

        var accessToken = jwtProvider.GenerateAccessToken(user);
        return new LoginUserResponse(accessToken, generatedRefreshToken.Value);
    }
}