"use client";

import { useState } from "react";
import { Form } from "@base-ui/react/form";
import { z } from "zod";
import { Button } from "@/shared/ui/Button";
import { PasswordField } from "@/shared/ui/PasswordField";
import { PhoneField } from "@/shared/ui/PhoneField";
import { fieldValidator } from "@/shared/lib/validation";
import { mapApiErrors, type FormErrors } from "@/shared/api/formErrors";
import { useLogin } from "../queries";
import { loginSchema, type LoginFormValues } from "../schemas";

type LoginFormProps = {
  onSuccess: () => void;
  onSwitchToRegister: () => void;
};

const LOGIN_FIELDS = ["phone", "password"] as const;

type LoginField = (typeof LOGIN_FIELDS)[number];

const FIELD_BY_CODE = {
  "user.phone.not_registered": "phone",
  "user.password.invalid": "password",
} satisfies Record<string, LoginField>;

export function LoginForm({ onSuccess, onSwitchToRegister }: LoginFormProps) {
  const login = useLogin();
  const [errors, setErrors] = useState<FormErrors>({});
  const [formError, setFormError] = useState<string | null>(null);

  async function handleSubmit(values: LoginFormValues) {
    if (login.isPending) {
      return;
    }

    setFormError(null);

    const result = loginSchema.safeParse(values);
    if (!result.success) {
      setErrors(z.flattenError(result.error).fieldErrors);
      return;
    }

    try {
      await login.mutateAsync(result.data);
      onSuccess();
    } catch (error) {
      const mapped = mapApiErrors(error, LOGIN_FIELDS, FIELD_BY_CODE);
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
      <PhoneField
        name="phone"
        label="Номер телефону"
        required
        validate={fieldValidator(loginSchema.shape.phone)}
      />
      <PasswordField
        name="password"
        label="Пароль"
        required
        autoComplete="current-password"
        validate={fieldValidator(loginSchema.shape.password)}
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
        disabled={login.isPending}
        focusableWhenDisabled
      >
        {login.isPending ? "Входимо…" : "Увійти"}
      </Button>

      <p className="text-center text-sm text-muted">
        Немає акаунту?{" "}
        <button
          type="button"
          onClick={onSwitchToRegister}
          disabled={login.isPending}
          className="cursor-pointer font-medium text-primary-ink transition-colors hover:text-primary-ink-dark hover:underline disabled:cursor-not-allowed disabled:opacity-60"
        >
          Зареєструватися
        </button>
      </p>
    </Form>
  );
}
