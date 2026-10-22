import type { components } from "@/shared/api/schema";

export type RegisterRequest = components["schemas"]["RegisterUserCommand"];
export type LoginRequest = components["schemas"]["LoginUserCommand"];
export type UserProfile = components["schemas"]["UserProfileResponse"];
