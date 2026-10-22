import { CurrentUserStatus } from "@/features/auth/components/CurrentUserStatus";

export default function HomePage() {
  return (
    <main className="mx-auto max-w-6xl px-4 py-10">
      <h1 className="text-3xl">Садова Країна</h1>
      <CurrentUserStatus />
    </main>
  );
}
