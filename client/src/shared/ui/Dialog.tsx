import type { ReactNode, RefObject } from "react";
import { Dialog as BaseDialog } from "@base-ui/react/dialog";
import { X } from "lucide-react";

type DialogSize = "md" | "lg";

type DialogProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title: string;
  description?: string;
  size?: DialogSize;
  finalFocus?: RefObject<HTMLElement | null> | false;
  children: ReactNode;
};

const backdropClasses =
  "fixed inset-0 z-40 bg-ink/40 transition-opacity duration-150 ease-out " +
  "data-starting-style:opacity-0 data-ending-style:opacity-0";

const popupClasses =
  "fixed top-1/2 left-1/2 z-50 max-h-[90dvh] w-[calc(100vw-2rem)] " +
  "-translate-x-1/2 -translate-y-1/2 overflow-y-auto rounded-2xl border border-border " +
  "bg-surface p-6 shadow-xl transition-[scale,opacity] duration-150 ease-out " +
  "data-starting-style:scale-95 data-starting-style:opacity-0 " +
  "data-ending-style:scale-95 data-ending-style:opacity-0";

const sizeClasses: Record<DialogSize, string> = {
  md: "max-w-md",
  lg: "max-w-2xl",
};

const closeClasses =
  "absolute top-4 right-4 flex size-8 cursor-pointer items-center justify-center rounded-full " +
  "text-muted transition-colors hover:bg-olive-tint hover:text-ink";

export function Dialog({
  open,
  onOpenChange,
  title,
  description,
  size = "md",
  finalFocus,
  children,
}: DialogProps) {
  return (
    <BaseDialog.Root open={open} onOpenChange={onOpenChange}>
      <BaseDialog.Portal>
        <BaseDialog.Backdrop className={backdropClasses} />
        <BaseDialog.Popup
          className={`${popupClasses} ${sizeClasses[size]}`}
          finalFocus={finalFocus}
        >
          <div className="mb-5 flex flex-col gap-1 px-8 text-center">
            <BaseDialog.Title className="text-xl">{title}</BaseDialog.Title>
            {description && (
              <BaseDialog.Description className="text-sm text-muted">
                {description}
              </BaseDialog.Description>
            )}
          </div>

          {children}

          <BaseDialog.Close aria-label="Закрити" className={closeClasses}>
            <X className="size-4.5" />
          </BaseDialog.Close>
        </BaseDialog.Popup>
      </BaseDialog.Portal>
    </BaseDialog.Root>
  );
}
