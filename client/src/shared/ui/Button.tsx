import { Button as BaseButton } from "@base-ui/react/button";

type ButtonVariant = "primary" | "secondary" | "danger";

type ButtonProps = Omit<BaseButton.Props, "className"> & {
  variant?: ButtonVariant;
  className?: string;
};

const baseClasses =
  "inline-flex items-center justify-center gap-2 rounded-xl px-4 py-2.5 text-sm font-semibold " +
  "cursor-pointer transition-colors disabled:cursor-not-allowed disabled:opacity-60";

const variantClasses: Record<ButtonVariant, string> = {
  primary: "bg-primary-ink text-white hover:bg-primary-ink-dark",
  secondary: "border border-ink bg-transparent text-ink hover:bg-olive-tint",
  danger: "bg-danger text-white hover:bg-danger/90",
};

export function Button({
  variant = "secondary",
  className,
  ...props
}: ButtonProps) {
  return (
    <BaseButton
      className={`${baseClasses} ${variantClasses[variant]} ${className ?? ""}`}
      {...props}
    />
  );
}
