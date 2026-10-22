import { useQuery } from "@tanstack/react-query";
import { ApiError } from "@/shared/api/apiError";
import { getCurrentUser } from "./api";
import type { UserProfile } from "./types";

export const currentUserQueryKey = ["auth", "current-user"] as const;

export function useCurrentUser() {
  return useQuery({
    queryKey: currentUserQueryKey,
    queryFn: async ({ signal }): Promise<UserProfile | null> => {
      try {
        return await getCurrentUser(signal);
      } catch (error) {
        if (
          error instanceof ApiError &&
          (error.status === 401 || error.status === 404)
        ) {
          return null;
        }
        throw error;
      }
    },
  });
}
