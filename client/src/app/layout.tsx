import type { Metadata } from "next";
import { Inter, PT_Serif } from "next/font/google";
import "./globals.css";

const inter = Inter({
  subsets: ["latin", "cyrillic"],
  variable: "--font-inter",
});

const ptSerif = PT_Serif({
  weight: "700",
  subsets: ["latin", "cyrillic"],
  variable: "--font-pt-serif",
});

export const metadata: Metadata = {
  title: "Садова Країна",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="uk" className={`${inter.variable} ${ptSerif.variable}`}>
      <body>{children}</body>
    </html>
  );
}
