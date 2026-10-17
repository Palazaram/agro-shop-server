using AgroShop.Application.Abstractions;
using AgroShop.Application.Options;
using AgroShop.Domain.Abstractions;
using AgroShop.Domain.Errors;
using AgroShop.Domain.RefreshTokens;
using AgroShop.Domain.Users;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace AgroShop.Application.Users.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    IRefreshTokenGenerator refreshTokenGenerator,
    IJwtProvider jwtProvider,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IOptions<RefreshTokenOptions> refreshTokenOptions)
    : ICommandHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<Result<RegisterUserResponse, Error>> HandleAsync(
        RegisterUserCommand command, 
        CancellationToken ct)
    {
        var phone = Phone.Create(command.Phone).Value;

        var existsByPhone = await userRepository.ExistsByPhoneAsync(phone, ct);
        if (existsByPhone)
            return DomainErrors.Users.PhoneAlreadyExists();

        var email = Email.Create(command.Email).Value;

        var existsByEmail = await userRepository.ExistsByEmailAsync(email, ct);
        if (existsByEmail)
            return DomainErrors.Users.EmailAlreadyExists();

        var fullName = FullName.Create(command.FirstName, command.LastName, command.Patronymic).Value;
        var password = Password.Create(command.Password).Value;

        var passwordHash = passwordHasher.Hash(password);

        var now = timeProvider.GetUtcNow();

        var user = User.Create(phone, email, passwordHash, fullName, now);

        var generatedRefreshToken = refreshTokenGenerator.Generate();

        var refreshToken = RefreshToken.Create(
            user.Id,
            generatedRefreshToken.Hash,
            now,
            TimeSpan.FromDays(refreshTokenOptions.Value.LifetimeDays));

        userRepository.Add(user);
        refreshTokenRepository.Add(refreshToken);
        await unitOfWork.SaveChangesAsync(ct);

        var accessToken = jwtProvider.GenerateAccessToken(user);
        return new RegisterUserResponse(accessToken, generatedRefreshToken.Value);
    }
}