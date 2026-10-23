import type { ReactNode, Ref } from "react";
import { Field } from "@base-ui/react/field";

export type TextFieldProps = Omit<
  Field.Control.Props,
  "className" | "name" | "disabled"
> & {
  name: string;
  label: string;
  validate?: Field.Root.Props["validate"];
  disabled?: boolean;
  leftIcon?: ReactNode;
  rightIcon?: ReactNode;
  ref?: Ref<HTMLInputElement>;
  className?: string;
};

const boxClasses =
  "flex items-center gap-2 rounded-xl border border-border bg-field px-3.5 py-2.5 text-muted " +
  "transition-colors [&_svg]:size-4.5 [&_svg]:shrink-0 focus-within:border-primary-ink " +
  "has-[input:focus-visible]:outline-2 has-[input:focus-visible]:outline-offset-2 " +
  "has-[input:focus-visible]:outline-primary-ink " +
  "group-data-invalid:border-danger group-data-disabled:opacity-60";

const controlClasses =
  "w-full min-w-0 bg-transparent text-base text-ink outline-none placeholder:text-muted " +
  "sm:text-sm disabled:cursor-not-allowed";

export function TextField({
  name,
  label,
  validate,
  disabled,
  leftIcon,
  rightIcon,
  className,
  ...controlProps
}: TextFieldProps) {
  return (
    <Field.Root
      name={name}
      validate={validate}
      disabled={disabled}
      className={`group flex flex-col gap-1.5 ${className ?? ""}`}
    >
      <Field.Label className="text-sm font-medium text-body">
        {label}
      </Field.Label>
      <div className={boxClasses}>
        {leftIcon}
        <Field.Control className={controlClasses} {...controlProps} />
        {rightIcon}
      </div>
      <Field.Error className="text-sm text-danger" />
    </Field.Root>
  );
}
