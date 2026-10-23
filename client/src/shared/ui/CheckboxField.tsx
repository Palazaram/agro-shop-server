import type { ReactNode } from "react";
import { Checkbox } from "@base-ui/react/checkbox";
import { Field } from "@base-ui/react/field";
import { Check } from "lucide-react";

type CheckboxFieldProps = Omit<
  Checkbox.Root.Props,
  "className" | "name" | "disabled"
> & {
  name: string;
  label: ReactNode;
  validate?: Field.Root.Props["validate"];
  disabled?: boolean;
  className?: string;
};

const labelClasses =
  "flex cursor-pointer items-start gap-2.5 text-sm text-body " +
  "group-data-disabled:cursor-not-allowed group-data-disabled:opacity-60";

const boxClasses =
  "mt-px flex size-5 shrink-0 items-center justify-center rounded-md border border-muted bg-surface " +
  "transition-colors data-checked:border-primary-ink data-checked:bg-primary-ink " +
  "data-invalid:border-danger";

export function CheckboxField({
  name,
  label,
  validate,
  disabled,
  className,
  ...checkboxProps
}: CheckboxFieldProps) {
  return (
    <Field.Root
      name={name}
      validate={validate}
      disabled={disabled}
      className={`group flex flex-col gap-1.5 ${className ?? ""}`}
    >
      <Field.Label className={labelClasses}>
        <Checkbox.Root className={boxClasses} {...checkboxProps}>
          <Checkbox.Indicator className="flex text-white">
            <Check className="size-3.5" strokeWidth={3} />
          </Checkbox.Indicator>
        </Checkbox.Root>
        <span>{label}</span>
      </Field.Label>
      <Field.Error className="pl-7.5 text-sm text-danger" />
    </Field.Root>
  );
}
