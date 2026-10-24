"use client";

import { useState } from "react";
import Link from "next/link";
import { Form } from "@base-ui/react/form";
import { Mail } from "lucide-react";
import { z } from "zod";
import { Button } from "@/shared/ui/Button";
import { CheckboxField } from "@/shared/ui/CheckboxField";
import { PasswordField } from "@/shared/ui/PasswordField";
import { PhoneField } from "@/shared/ui/PhoneField";
import { TextField } from "@/shared/ui/TextField";
import { fieldValidator } from "@/shared/lib/validation";
import { mapApiErrors, type FormErrors } from "@/shared/api/formErrors";
import { useRegister } from "../queries";
import {
  registerFieldsSchema,
  registerSchema,
  type RegisterFormValues,
} from "../schemas";

type RegisterFormProps = {
  onSuccess: () => void;
  onSwitchToLogin: () => void;
};

const REGISTER_FIELDS = [
  "lastName",
  "firstName",
  "patronymic",
  "phone",
  "email",
  "password",
  "acceptTerms",
] as const;

type RegisterField = (typeof REGISTER_FIELDS)[number];

const FIELD_BY_CODE = {
  "user.phone.already_exists": "phone",
  "user.email.already_exists": "email",
} satisfies Record<string, RegisterField>;

const fields = registerFieldsSchema.shape;

const validateConfirmPresence = fieldValidator(fields.confirmPassword);

function validateConfirmPassword(
  value: unknown,
  formValues: Record<string, unknown>,
) {
  return (
    validateConfirmPresence(value) ??
    (value === formValues.password ? null : "Паролі не збігаються")
  );
}

export function RegisterForm({
  onSuccess,
  onSwitchToLogin,
}: RegisterFormProps) {
  const register = useRegister();
  const [errors, setErrors] = useState<FormErrors>({});
  const [formError, setFormError] = useState<string | null>(null);

  async function handleSubmit(values: RegisterFormValues) {
    if (register.isPending) {
      return;
    }

    setFormError(null);

    const result = registerSchema.safeParse(values);
    if (!result.success) {
      setErrors(z.flattenError(result.error).fieldErrors);
      return;
    }

    try {
      await register.mutateAsync(result.data);
      onSuccess();
    } catch (error) {
      const mapped = mapApiErrors(error, REGISTER_FIELDS, FIELD_BY_CODE);
      setErrors(mapped.fieldErrors);
      setFormError(mapped.formError);
    }
  }

  return (
    <Form
      errors={errors}
      onFormSubmit={handleSubmit}
      validationMode="onSubmit"
      className="flex flex-col gap-4"
    >
      <div className="grid items-start gap-4 sm:grid-cols-3">
        <TextField
          name="lastName"
          label="Прізвище"
          required
          autoComplete="family-name"
          autoCapitalize="words"
          validate={fieldValidator(fields.lastName)}
        />
        <TextField
          name="firstName"
          label="Ім'я"
          required
          autoComplete="given-name"
          autoCapitalize="words"
          validate={fieldValidator(fields.firstName)}
        />
        <TextField
          name="patronymic"
          label="По батькові"
          required
          autoComplete="additional-name"
          autoCapitalize="words"
          validate={fieldValidator(fields.patronymic)}
        />
      </div>

      <div className="grid items-start gap-4 sm:grid-cols-2">
        <PhoneField
          name="phone"
          label="Номер телефону"
          required
          validate={fieldValidator(fields.phone)}
        />
        <TextField
          name="email"
          label="Пошта"
          required
          inputMode="email"
          autoComplete="email"
          autoCapitalize="none"
          spellCheck={false}
          placeholder="name@example.com"
          leftIcon={<Mail />}
          validate={fieldValidator(fields.email)}
        />
      </div>

      <div className="grid items-start gap-4 sm:grid-cols-2">
        <PasswordField
          name="password"
          label="Пароль"
          required
          autoComplete="new-password"
          validate={fieldValidator(fields.password)}
        />
        <PasswordField
          name="confirmPassword"
          label="Повторіть пароль"
          required
          autoComplete="new-password"
          validate={validateConfirmPassword}
        />
      </div>

      <CheckboxField
        name="acceptTerms"
        label={
          <>
            Я погоджуюсь з{" "}
            <Link
              href="/privacy"
              target="_blank"
              className="font-medium text-primary-ink underline underline-offset-2 transition-colors hover:text-primary-ink-dark"
            >
              умовами обробки персональних даних
              <span className="sr-only"> (відкриється в новій вкладці)</span>
            </Link>
          </>
        }
        required
        validate={fieldValidator(fields.acceptTerms)}
      />

      {formError && (
        <p role="alert" className="text-center text-sm text-danger">
          {formError}
        </p>
      )}

      <Button
        type="submit"
        variant="primary"
        className="mt-2 w-full"
        disabled={register.isPending}
        focusableWhenDisabled
      >
        {register.isPending ? "Реєструємо…" : "Зареєструватися"}
      </Button>

      <p className="text-center text-sm text-muted">
        Вже є акаунт?{" "}
        <button
          type="button"
          onClick={onSwitchToLogin}
          disabled={register.isPending}
          className="cursor-pointer font-medium text-primary-ink transition-colors hover:text-primary-ink-dark hover:underline disabled:cursor-not-allowed disabled:opacity-60"
        >
          Увійти
        </button>
      </p>
    </Form>
  );
}
