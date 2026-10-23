import type { ZodType } from "zod";

export function fieldValidator(schema: ZodType) {
  return (value: unknown): string | null => {
    const result = schema.safeParse(value);
    return result.success ? null : result.error.issues[0].message;
  };
}
