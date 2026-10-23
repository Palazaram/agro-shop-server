"use client";

import { useState } from "react";
import { Eye, EyeOff, Lock } from "lucide-react";
import { TextField, type TextFieldProps } from "./TextField";

type PasswordFieldProps = Omit<
  TextFieldProps,
  "type" | "leftIcon" | "rightIcon"
>;

const toggleClasses =
  "flex shrink-0 cursor-pointer items-center justify-center rounded-full text-muted " +
  "transition-colors hover:text-primary-ink disabled:cursor-not-allowed";

export function PasswordField({ disabled, ...props }: PasswordFieldProps) {
  const [visible, setVisible] = useState(false);

  return (
    <TextField
      {...props}
      disabled={disabled}
      type={visible ? "text" : "password"}
      leftIcon={<Lock />}
      rightIcon={
        <button
          type="button"
          disabled={disabled}
          onClick={() => setVisible((current) => !current)}
          onMouseDown={(event) => event.preventDefault()}
          aria-label={visible ? "Приховати пароль" : "Показати пароль"}
          className={toggleClasses}
        >
          {visible ? <EyeOff /> : <Eye />}
        </button>
      }
    />
  );
}
