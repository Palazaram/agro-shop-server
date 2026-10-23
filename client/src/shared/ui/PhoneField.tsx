"use client";

import { Phone } from "lucide-react";
import { useMask, type Track } from "@react-input/mask";
import { TextField, type TextFieldProps } from "./TextField";

type PhoneFieldProps = Omit<TextFieldProps, "type" | "leftIcon" | "ref">;

const PREFIX = "+380 (";

const track: Track = ({ inputType, data, selectionStart }) => {
  if (inputType !== "insert" || selectionStart > PREFIX.length) {
    return data;
  }

  const digits = data.replace(/\D/g, "").replace(/^(?:380|0)/, "");
  return digits || false;
};

export function PhoneField(props: PhoneFieldProps) {
  const inputRef = useMask({
    mask: "+380 (__) ___-__-__",
    replacement: { _: /\d/ },
    track,
  });

  return (
    <TextField
      autoComplete="tel"
      placeholder="+380 (XX) XXX-XX-XX"
      {...props}
      ref={inputRef}
      type="tel"
      leftIcon={<Phone />}
    />
  );
}
