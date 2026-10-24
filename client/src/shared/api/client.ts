import { ApiError, type ApiErrorItem } from "./apiError";

type RequestOptions = {
  method?: "GET" | "POST" | "PUT" | "DELETE";
  body?: unknown;
  signal?: AbortSignal;
};

const API_PREFIX = "/api";
const REFRESH_LOCK_NAME = "agro-shop:auth-refresh";
const REFRESH_TIMEOUT_MS = 10_000;

let refreshInFlight: Promise<boolean> | null = null;

export async function apiFetch<T>(
  path: string,
  options: RequestOptions = {},
): Promise<T> {
  let response = await send(path, options);

  if (response.status === 401 && !path.startsWith("/auth/")) {
    const refreshed = await refreshSession();

    if (refreshed) {
      response = await send(path, options);
    }
  }

  if (!response.ok) {
    throw await toApiError(response);
  }

  const text = await response.text();
  return (text ? JSON.parse(text) : undefined) as T;
}

function send(
  path: string,
  { method = "GET", body, signal }: RequestOptions,
): Promise<Response> {
  return fetch(`${API_PREFIX}${path}`, {
    method,
    signal,
    headers:
      body === undefined ? undefined : { "Content-Type": "application/json" },
    body: body === undefined ? undefined : JSON.stringify(body),
  });
}

function refreshSession(): Promise<boolean> {
  refreshInFlight ??= refreshAcrossTabs().finally(() => {
    refreshInFlight = null;
  });

  return refreshInFlight;
}

async function refreshAcrossTabs(): Promise<boolean> {
  if (typeof navigator === "undefined" || !navigator.locks) {
    return requestRefresh();
  }

  return await navigator.locks.request(REFRESH_LOCK_NAME, requestRefresh);
}

async function requestRefresh(): Promise<boolean> {
  const response = await fetch(`${API_PREFIX}/auth/refresh`, {
    method: "POST",
    signal: AbortSignal.timeout(REFRESH_TIMEOUT_MS),
  });

  return response.ok || response.status === 409;
}

async function toApiError(response: Response): Promise<ApiError> {
  let errors: ApiErrorItem[] = [];

  try {
    const body = await response.json();
    errors = body.errors ?? [];
  } catch {
    // Пустое тело или не JSON — например, 401 от проверки JWT.
  }

  return new ApiError(response.status, errors);
}
