import { z } from "zod";
import type { RegisterRequest } from "./types";

const PHONE_PATTERN = /^\+380 \(\d{2}\) \d{3}-\d{2}-\d{2}$/;
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/;
const PASSWORD_PATTERN = /^[!-~]+$/;

const phoneSchema = z
  .string()
  .min(1, { error: "Введіть номер телефону", abort: true })
  .regex(PHONE_PATTERN, "Введіть номер повністю")
  .transform((value) => `+${value.replace(/\D/g, "")}`);

function requiredText(message: string) {
  return z.string().trim().min(1, message);
}

export const loginSchema = z.object({
  phone: phoneSchema,
  password: z.string().min(1, "Введіть пароль"),
});

export type LoginFormValues = z.input<typeof loginSchema>;

export const registerFieldsSchema = z.object({
  lastName: requiredText("Введіть прізвище"),
  firstName: requiredText("Введіть ім'я"),
  patronymic: requiredText("Введіть по батькові"),
  phone: phoneSchema,
  email: z
    .string()
    .trim()
    .min(1, { error: "Введіть пошту", abort: true })
    .regex(EMAIL_PATTERN, "Введіть пошту у форматі name@example.com"),
  password: z
    .string()
    .min(1, { error: "Введіть пароль", abort: true })
    .regex(PASSWORD_PATTERN, {
      error:
        "Пароль може містити лише латинські літери, цифри та символи, без пробілів",
      abort: true,
    })
    .min(8, { error: "Пароль має містити щонайменше 8 символів", abort: true })
    .max(64, "Пароль має містити не більше 64 символів"),
  confirmPassword: requiredText("Введіть пароль ще раз"),
  acceptTerms: z.literal(true, {
    error: "Потрібна згода на обробку персональних даних",
  }),
});

export const registerSchema = registerFieldsSchema
  .refine((data) => data.password === data.confirmPassword, {
    path: ["confirmPassword"],
    error: "Паролі не збігаються",
  })
  .transform(
    (data): RegisterRequest => ({
      firstName: data.firstName,
      lastName: data.lastName,
      patronymic: data.patronymic,
      email: data.email,
      phone: data.phone,
      password: data.password,
      acceptTerms: data.acceptTerms,
    }),
  );

export type RegisterFormValues = z.input<typeof registerSchema>;
