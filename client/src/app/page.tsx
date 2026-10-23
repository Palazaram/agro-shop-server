import { CurrentUserStatus } from "@/features/auth/components/CurrentUserStatus";
import { Button } from "@/shared/ui/Button";
import { Form } from "@base-ui/react/form";
import { PhoneField } from "@/shared/ui/PhoneField";

export default function HomePage() {
  return (
    <main className="mx-auto max-w-6xl px-4 py-10">
      <h1 className="text-3xl">Садова Країна</h1>
      <CurrentUserStatus />

      <div className="flex gap-3 p-6">
        <Button variant="primary">Увійти</Button>
        <Button>Скасувати</Button>
        <Button variant="danger">Видалити</Button>
        <Button variant="primary" disabled>
          Зачекайте…
        </Button>
      </div>

      <Form className="flex max-w-sm flex-col gap-4 p-6">
        <PhoneField name="phone" label="Телефон" />
      </Form>
    </main>
  );
}
