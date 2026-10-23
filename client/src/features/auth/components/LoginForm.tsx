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

const FIELD_BY_CODE = {
  "user.phone.not_registered": "phone",
  "user.password.invalid": "password",
};

export function LoginForm({ onSuccess, onSwitchToRegister }: LoginFormProps) {
  const login = useLogin();
  const [errors, setErrors] = useState<FormErrors>({});
  const [formError, setFormError] = useState<string | null>(null);

  async function handleSubmit(values: LoginFormValues) {
    setErrors({});
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
        validate={fieldValidator(loginSchema.shape.phone)}
      />
      <PasswordField
        name="password"
        label="Пароль"
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
      >
        {login.isPending ? "Входимо…" : "Увійти"}
      </Button>

      <p className="text-center text-sm text-muted">
        Немає акаунту?{" "}
        <button
          type="button"
          onClick={onSwitchToRegister}
          className="cursor-pointer font-medium text-primary-ink transition-colors hover:text-primary-ink-dark hover:underline"
        >
          Зареєструватися
        </button>
      </p>
    </Form>
  );
}
