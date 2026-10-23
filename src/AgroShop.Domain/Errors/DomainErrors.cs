namespace AgroShop.Domain.Errors;

public static class DomainErrors
{
    public static class Users 
    {
        public static Error NameIsRequired()
            => Error.Validation("user.name.required", "Це поле є обов'язковим");

        public static Error NameTooShort(int minLength)
            => Error.Validation("user.name.min_length",
                $"Має містити від {minLength} символів");

        public static Error NameTooLong(int maxLength)
            => Error.Validation("user.name.max_length",
                $"Може містити до {maxLength} символів");

        public static Error NameInvalidFormat()
            => Error.Validation("user.name.invalid_format",
                "Використовуйте лише українські літери, апостроф, дефіс або пробіл між словами");

        public static Error EmailIsRequired()
            => Error.Validation("user.email.required", 
                "Введіть пошту");

        public static Error EmailTooLong(int maxLength)
            => Error.Validation("user.email.max_length",
                $"Адреса може містити до {maxLength} символів");

        public static Error EmailInvalidFormat()
            => Error.Validation("user.email.invalid_format", 
                "Введіть пошту у форматі name@example.com");

        public static Error PhoneIsRequired()
            => Error.Validation("user.phone.required", 
                "Введіть номер телефону");

        public static Error PhoneInvalidLength(int length)
            => Error.Validation("user.phone.invalid_length",
                $"Номер має містити {length} цифр після коду країни");

        public static Error PhoneInvalidFormat()
            => Error.Validation("user.phone.invalid_format", 
                "Перевірте номер телефону, наприклад +380 67 123 45 67");

        public static Error PhoneUnknownOperatorCode()
            => Error.Validation("user.phone.unknown_operator_code",
                "Такого коду мобільного оператора немає. Перевірте номер");

        public static Error PasswordIsRequired()
            => Error.Validation("user.password.required", "Введіть пароль");

        public static Error PasswordTooShort(int minLength)
            => Error.Validation("user.password.min_length",
                $"Пароль має містити від {minLength} символів");

        public static Error PasswordTooLong(int maxLength)
            => Error.Validation("user.password.max_length",
                $"Пароль може містити до {maxLength} символів");

        public static Error PasswordHasUnsupportedCharacters()
            => Error.Validation("user.password.unsupported_characters",
                "Пароль може містити лише латинські літери, цифри та символи, без пробілів");

        public static Error EmailAlreadyExists()
            => Error.Conflict("user.email.already_exists",
                "Обліковий запис з цією поштою вже існує");

        public static Error PhoneAlreadyExists()
            => Error.Conflict("user.phone.already_exists",
                "Обліковий запис з цим номером уже існує. Увійдіть");

        public static Error PhoneNotRegistered()
            => Error.Unauthorized("user.phone.not_registered",
                "Обліковий запис з цим номером не знайдено");

        public static Error PasswordInvalid()
            => Error.Unauthorized("user.password.invalid", "Неправильний пароль");

        public static Error NotFound()
            => Error.NotFound("user.not_found", "Користувача не знайдено");

        public static Error TermsNotAccepted()
            => Error.Validation("user.terms.not_accepted", "Потрібна згода на обробку персональних даних");
    }

    public static class RefreshTokens 
    {
        public static Error AlreadyReplaced()
            => Error.Unauthorized("refresh_token.already_replaced",
                "Сесію завершено з міркувань безпеки. Увійдіть знову");

        public static Error Invalid()
            => Error.Unauthorized("refresh_token.invalid",
                "Сесію не знайдено або вона закінчилася. Увійдіть знову");

        public static Error ConcurrentRotation()
            => Error.Conflict("refresh_token.concurrent_rotation",
                "Сесію вже оновлено в іншій вкладці");
    }
}