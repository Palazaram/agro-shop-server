import Link from "next/link";
import { Clock, MapPin, Phone, Sprout } from "lucide-react";
import { UserMenu } from "@/features/auth/components/UserMenu";

export function Header() {
  return (
    <>
      <div className="bg-forest text-sm text-white">
        <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-4 py-2 [&_svg]:size-4.5 [&_svg]:shrink-0">
          <div className="flex items-center gap-6">
            <p className="flex items-center gap-2">
              <Clock />
              Пн–Сб: 10:00–18:00
            </p>
            <p className="hidden items-center gap-2 sm:flex">
              <MapPin />
              м. Київ, вул. Велика Кільцева, 4г
            </p>
          </div>
          <a
            href="tel:+380996286644"
            className="flex items-center gap-2 text-white transition-colors hover:text-primary-on-dark"
          >
            <Phone />
            +380 (99) 628-66-44
          </a>
        </div>
      </div>

      <header className="sticky top-0 z-30 border-b border-border bg-surface">
        <div className="mx-auto flex max-w-6xl items-center justify-between gap-6 px-4 py-4">
          <Link href="/" className="flex shrink-0 items-center gap-2">
            <Sprout className="size-7 text-olive" />
            <span className="font-serif text-2xl font-bold text-forest">
              Садова Країна
            </span>
          </Link>
          <UserMenu />
        </div>
      </header>
    </>
  );
}
