import { apiFetch } from "@/shared/api/client";
import type { LoginRequest, RegisterRequest, UserProfile } from "./types";

export function getCurrentUser(signal?: AbortSignal) {
  return apiFetch<UserProfile>("/users/me", { signal });
}

export function register(request: RegisterRequest) {
  return apiFetch<void>("/auth/register", { method: "POST", body: request });
}

export function login(request: LoginRequest) {
  return apiFetch<void>("/auth/login", { method: "POST", body: request });
}

export function logout() {
  return apiFetch<void>("/auth/logout", { method: "POST" });
}
