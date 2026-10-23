import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { ApiError } from "@/shared/api/apiError";
import { getCurrentUser, login, logout, register } from "./api";
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

export function useLogin() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: login,
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: currentUserQueryKey }),
  });
}

export function useLogout() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: logout,
    onSuccess: () => {
      queryClient.setQueryData(currentUserQueryKey, null);
    },
  });
}

export function useRegister() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: register,
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: currentUserQueryKey }),
  });
}
