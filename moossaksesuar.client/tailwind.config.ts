// tailwind.config.ts
import type { Config } from "tailwindcss";

export default {
    // Bu satır, HTML'de "dark" sınıfı olduğunda
    // karanlık tema stillerinin çalışmasını sağlar. Bu kalmalı.
    darkMode: "class",

    content: ["./index.html", "./src/**/*.{ts,tsx}"],
    theme: {
        extend: {
            // Özel renkleriniz veya temalarınız yoksa bu alanı
            // boş bırakabilirsiniz. Projeniz zaten Tailwind'in
            // varsayılan renklerini (örn: bg-black, text-white) kullanıyor.
        },
    },
    plugins: [],
} satisfies Config;
