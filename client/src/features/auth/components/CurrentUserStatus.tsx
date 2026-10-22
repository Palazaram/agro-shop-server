"use client";

import { useCurrentUser } from "../queries";

export function CurrentUserStatus() {
  const { data: user, isPending } = useCurrentUser();

  if (isPending) return <p>Перевіряємо…</p>;

  return <p>{user ? `Ви увійшли як ${user.firstName}` : "Ви не увійшли"}</p>;
}
