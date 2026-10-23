"use client";

import { useRef, useState } from "react";
import { Menu } from "@base-ui/react/menu";
import { LogIn, LogOut, User, UserPlus } from "lucide-react";
import { Dialog } from "@/shared/ui/Dialog";
import { useCurrentUser, useLogout } from "../queries";
import { LoginForm } from "./LoginForm";
import { RegisterForm } from "./RegisterForm";

type AuthDialog = "login" | "register" | null;

const triggerClasses =
  "flex h-10 cursor-pointer items-center gap-2 rounded-full px-2.5 text-ink transition-colors " +
  "hover:bg-olive-tint hover:text-primary-ink data-popup-open:bg-olive-tint [&_svg]:size-5.5";

const popupClasses =
  "flex min-w-48 origin-[var(--transform-origin)] flex-col gap-1 rounded-xl border border-border " +
  "bg-surface p-2 text-ink shadow-lg outline-none transition-[scale,opacity] duration-100 ease-out " +
  "data-starting-style:scale-[0.98] data-starting-style:opacity-0 " +
  "data-ending-style:scale-[0.98] data-ending-style:opacity-0";

const itemClasses =
  "flex w-full cursor-pointer items-center gap-2 rounded-lg px-3 py-2 text-sm text-ink outline-none " +
  "transition-colors data-highlighted:bg-olive-tint data-highlighted:text-forest " +
  "data-disabled:cursor-not-allowed data-disabled:opacity-60 [&_svg]:size-5";

export function UserMenu() {
  const { data: user, isPending } = useCurrentUser();
  const logout = useLogout();
  const [dialog, setDialog] = useState<AuthDialog>(null);
  const triggerRef = useRef<HTMLButtonElement>(null);

  if (isPending) {
    return <div className="size-10" aria-hidden />;
  }

  function handleDialogOpenChange(open: boolean) {
    if (!open) {
      setDialog(null);
    }
  }

  return (
    <>
      <Menu.Root>
        <Menu.Trigger
          ref={triggerRef}
          aria-label={
            user ? `${user.firstName}, обліковий запис` : "Обліковий запис"
          }
          className={triggerClasses}
        >
          <User />
          {user && (
            <span className="hidden text-sm font-medium sm:inline">
              {user.firstName}
            </span>
          )}
        </Menu.Trigger>

        <Menu.Portal>
          <Menu.Positioner sideOffset={8} align="end" className="z-40">
            <Menu.Popup className={popupClasses}>
              {user ? (
                <>
                  <div className="px-3 py-2">
                    <p className="text-sm font-medium">
                      {user.firstName} {user.lastName}
                    </p>
                    <p className="text-xs text-muted">{user.phone}</p>
                  </div>
                  <Menu.Separator className="my-1 h-px bg-border" />
                  <Menu.Item
                    className={itemClasses}
                    disabled={logout.isPending}
                    onClick={() => logout.mutate()}
                  >
                    <LogOut />
                    Вийти
                  </Menu.Item>
                </>
              ) : (
                <>
                  <Menu.Item
                    className={itemClasses}
                    onClick={() => setDialog("login")}
                  >
                    <LogIn />
                    Увійти
                  </Menu.Item>
                  <Menu.Item
                    className={itemClasses}
                    onClick={() => setDialog("register")}
                  >
                    <UserPlus />
                    Зареєструватися
                  </Menu.Item>
                </>
              )}
            </Menu.Popup>
          </Menu.Positioner>
        </Menu.Portal>
      </Menu.Root>

      <Dialog
        open={dialog === "login"}
        onOpenChange={handleDialogOpenChange}
        title="Ласкаво просимо"
        description="Увійдіть, щоб продовжити"
        finalFocus={dialog === null ? triggerRef : false}
      >
        <LoginForm
          onSuccess={() => setDialog(null)}
          onSwitchToRegister={() => setDialog("register")}
        />
      </Dialog>

      <Dialog
        open={dialog === "register"}
        onOpenChange={handleDialogOpenChange}
        title="Реєстрація"
        description="Створіть обліковий запис"
        size="lg"
        finalFocus={dialog === null ? triggerRef : false}
      >
        <RegisterForm
          onSuccess={() => setDialog(null)}
          onSwitchToLogin={() => setDialog("login")}
        />
      </Dialog>
    </>
  );
}
