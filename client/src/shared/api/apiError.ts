import type { components } from "./schema";

export type ApiErrorItem =
  components["schemas"]["ProblemDetails"]["errors"][number];

export class ApiError extends Error {
  readonly status: number;
  readonly errors: ApiErrorItem[];

  constructor(status: number, errors: ApiErrorItem[]) {
    super(`Request failed with status ${status}`);
    this.name = "ApiError";
    this.status = status;
    this.errors = errors;
  }
}
