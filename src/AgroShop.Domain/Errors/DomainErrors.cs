namespace AgroShop.Domain.Errors;

public static class DomainErrors
{
    public static class Users 
    {
        public static Error NameIsRequired()
            => Error.Validation("user.name.required", "Name is required");

        public static Error NameTooShort(int minLength)
            => Error.Validation("user.name.min_length",
                $"Name must be at least {minLength} characters long");

        public static Error NameTooLong(int maxLength)
            => Error.Validation("user.name.max_length",
                $"Name must not exceed {maxLength} characters");

        public static Error NameInvalidFormat()
            => Error.Validation("user.name.invalid_format",
                "Name must contain only Ukrainian letters, apostrophes and hyphens");

        public static Error EmailIsRequired()
            => Error.Validation("user.email.required", 
                "Email is required");

        public static Error EmailTooLong(int maxLength)
            => Error.Validation("user.email.max_length",
                $"Email must not exceed {maxLength} characters");

        public static Error EmailInvalidFormat()
            => Error.Validation("user.email.invalid_format", 
                "Email has invalid format");

        public static Error PhoneIsRequired()
            => Error.Validation("user.phone.required", 
                "Phone number is required");

        public static Error PhoneInvalidLength(int length)
            => Error.Validation("user.phone.invalid_length",
                $"Phone number must contain {length} digits after the country code");

        public static Error PhoneInvalidFormat()
            => Error.Validation("user.phone.invalid_format", 
                "Phone number has invalid format");

        public static Error PhoneUnknownOperatorCode()
            => Error.Validation("user.phone.unknown_operator_code",
                "Phone number does not start with a known Ukrainian mobile code");

        public static Error PasswordIsRequired()
            => Error.Validation("user.password.required", "Password is required");

        public static Error PasswordTooShort(int minLength)
            => Error.Validation("user.password.min_length",
                $"Password must be at least {minLength} characters long");

        public static Error PasswordTooLong(int maxLength)
            => Error.Validation("user.password.max_length",
                $"Password must not exceed {maxLength} characters");

        public static Error PasswordHasUnsupportedCharacters()
            => Error.Validation("user.password.unsupported_characters",
                "Password may contain only Latin letters, digits and punctuation, without spaces");

        public static Error EmailAlreadyExists()
            => Error.Conflict("user.email.already_exists",
                "User with this email address already exists");

        public static Error PhoneAlreadyExists()
            => Error.Conflict("user.phone.already_exists",
                "User with this phone number already exists");

        public static Error PhoneNotRegistered()
            => Error.Unauthorized("user.phone.not_registered",
                "User with this phone number is not registered");

        public static Error PasswordInvalid()
            => Error.Unauthorized("user.password.invalid", "Invalid password");
    }

    public static class RefreshTokens 
    {
        public static Error AlreadyReplaced()
            => Error.Unauthorized("refresh_token.already_replaced",
                "Refresh token has already been replaced; possible token reuse");

        public static Error Invalid()
            => Error.Unauthorized("refresh_token.invalid",
                "Refresh token is revoked or expired");
    }
}