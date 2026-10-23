import { ApiError } from "./apiError";

export type FormErrors = Record<string, string[]>;

type MappedErrors = {
  fieldErrors: FormErrors;
  formError: string | null;
};

const NETWORK_ERROR =
  "Не вдалося з'єднатися з сервером. Перевірте інтернет і спробуйте ще раз.";
const UNKNOWN_ERROR = "Щось пішло не так. Спробуйте ще раз.";
const SERVER_ERROR = "Сервер тимчасово недоступний. Спробуйте пізніше.";

export function mapApiErrors(
  error: unknown,
  fields: readonly string[],
  fieldByCode: Record<string, string> = {},
): MappedErrors {
  if (!(error instanceof ApiError)) {
    return { fieldErrors: {}, formError: NETWORK_ERROR };
  }

  const fieldErrors: FormErrors = {};
  const formErrors: string[] = [];

  for (const item of error.errors) {
    const field = item.field ?? fieldByCode[item.code];

    if (field !== undefined && fields.includes(field)) {
      (fieldErrors[field] ??= []).push(item.message);
    } else {
      formErrors.push(item.message);
    }
  }

  if (Object.keys(fieldErrors).length === 0 && formErrors.length === 0) {
    formErrors.push(error.status >= 500 ? SERVER_ERROR : UNKNOWN_ERROR);
  }

  return {
    fieldErrors,
    formError: formErrors.length > 0 ? formErrors.join(" ") : null,
  };
}
