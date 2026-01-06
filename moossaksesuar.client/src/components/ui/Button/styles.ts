// src/components/ui/Button/styles.ts
import { cn } from "@/utils/cn";
import type { ButtonColor, ButtonVariant, ButtonSize } from "./types";

export const base =
    "btn-base inline-flex items-center justify-center gap-2 select-none whitespace-nowrap rounded-lg text-sm font-medium transition-colors active:translate-y-px focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed";

export const palette: Record<ButtonColor, Record<ButtonVariant, string>> = {
    // Not: Burada "h-10 px-4" var; birazdan size map'ini SONDA ekleyerek override edeceğiz.
    neutral: {
        filled:
            "h-10 px-4 bg-gray-800 text-white hover:bg-gray-900 focus-visible:ring-gray-400",
        outline:
            "h-10 px-4 border border-gray-300 text-gray-800 hover:bg-gray-100 focus-visible:ring-gray-400",
        soft:
            "h-10 px-4 bg-gray-100 text-gray-800 hover:bg-gray-200 focus-visible:ring-gray-300",
        ghost: "h-10 px-4 text-gray-800 hover:bg-gray-100 focus-visible:ring-gray-300",
    },
    primary: {
        filled:
            "h-10 px-4 bg-blue-600 text-white hover:bg-blue-700 focus-visible:ring-blue-400",
        outline:
            "h-10 px-4 border border-blue-300 text-blue-600 hover:bg-blue-50 focus-visible:ring-blue-400",
        soft:
            "h-10 px-4 bg-blue-50 text-blue-700 hover:bg-blue-100 focus-visible:ring-blue-300",
        ghost: "h-10 px-4 text-blue-600 hover:bg-blue-50 focus-visible:ring-blue-300",
    },
    secondary: {
        filled:
            "h-10 px-4 bg-violet-600 text-white hover:bg-violet-700 focus-visible:ring-violet-400",
        outline:
            "h-10 px-4 border border-violet-300 text-violet-700 hover:bg-violet-50 focus-visible:ring-violet-400",
        soft:
            "h-10 px-4 bg-violet-50 text-violet-700 hover:bg-violet-100 focus-visible:ring-violet-300",
        ghost:
            "h-10 px-4 text-violet-700 hover:bg-violet-50 focus-visible:ring-violet-300",
    },
    info: {
        filled:
            "h-10 px-4 bg-sky-600 text-white hover:bg-sky-700 focus-visible:ring-sky-400",
        outline:
            "h-10 px-4 border border-sky-300 text-sky-700 hover:bg-sky-50 focus-visible:ring-sky-400",
        soft:
            "h-10 px-4 bg-sky-50 text-sky-700 hover:bg-sky-100 focus-visible:ring-sky-300",
        ghost: "h-10 px-4 text-sky-700 hover:bg-sky-50 focus-visible:ring-sky-300",
    },
    success: {
        filled:
            "h-10 px-4 bg-emerald-600 text-white hover:bg-emerald-700 focus-visible:ring-emerald-400",
        outline:
            "h-10 px-4 border border-emerald-300 text-emerald-700 hover:bg-emerald-50 focus-visible:ring-emerald-400",
        soft:
            "h-10 px-4 bg-emerald-50 text-emerald-700 hover:bg-emerald-100 focus-visible:ring-emerald-300",
        ghost:
            "h-10 px-4 text-emerald-700 hover:bg-emerald-50 focus-visible:ring-emerald-300",
    },
    warning: {
        filled:
            "h-10 px-4 bg-amber-500 text-white hover:bg-amber-600 focus-visible:ring-amber-400",
        outline:
            "h-10 px-4 border border-amber-300 text-amber-700 hover:bg-amber-50 focus-visible:ring-amber-400",
        soft:
            "h-10 px-4 bg-amber-50 text-amber-700 hover:bg-amber-100 focus-visible:ring-amber-300",
        ghost:
            "h-10 px-4 text-amber-700 hover:bg-amber-50 focus-visible:ring-amber-300",
    },
    danger: {
        filled:
            "h-10 px-4 bg-rose-600 text-white hover:bg-rose-700 focus-visible:ring-rose-400",
        outline:
            "h-10 px-4 border border-rose-300 text-rose-700 hover:bg-rose-50 focus-visible:ring-rose-400",
        soft:
            "h-10 px-4 bg-rose-50 text-rose-700 hover:bg-rose-100 focus-visible:ring-rose-300",
        ghost:
            "h-10 px-4 text-rose-700 hover:bg-rose-50 focus-visible:ring-rose-300",
    },
    error: {
        filled:
            "h-10 px-4 bg-rose-600 text-white hover:bg-rose-700 focus-visible:ring-rose-400",
        outline:
            "h-10 px-4 border border-rose-300 text-rose-700 hover:bg-rose-50 focus-visible:ring-rose-400",
        soft:
            "h-10 px-4 bg-rose-50 text-rose-700 hover:bg-rose-100 focus-visible:ring-rose-300",
        ghost:
            "h-10 px-4 text-rose-700 hover:bg-rose-50 focus-visible:ring-rose-300",
    },
};

export const glowMap: Record<ButtonColor, string> = {
    neutral:
        "shadow-[0_0_0_.25rem_rgba(0,0,0,.10)] hover:shadow-[0_0_1.25rem_.25rem_rgba(0,0,0,.15)]",
    primary:
        "shadow-[0_0_0_.25rem_rgba(59,130,246,.35)] hover:shadow-[0_0_1.25rem_.25rem_rgba(59,130,246,.5)]",
    secondary:
        "shadow-[0_0_0_.25rem_rgba(139,92,246,.35)] hover:shadow-[0_0_1.25rem_.25rem_rgba(139,92,246,.5)]",
    info:
        "shadow-[0_0_0_.25rem_rgba(56,189,248,.35)] hover:shadow-[0_0_1.25rem_.25rem_rgba(56,189,248,.5)]",
    success:
        "shadow-[0_0_0_.25rem_rgba(16,185,129,.35)] hover:shadow-[0_0_1.25rem_.25rem_rgba(16,185,129,.5)]",
    warning:
        "shadow-[0_0_0_.25rem_rgba(245,158,11,.35)] hover:shadow-[0_0_1.25rem_.25rem_rgba(245,158,11,.5)]",
    danger:
        "shadow-[0_0_0_.25rem_rgba(244,63,94,.35)] hover:shadow-[0_0_1.25rem_.25rem_rgba(244,63,94,.5)]",
    error:
        "shadow-[0_0_0_.25rem_rgba(244,63,94,.35)] hover:shadow-[0_0_1.25rem_.25rem_rgba(244,63,94,.5)]",
};

// 🔧 boyut sınıfları
const SIZE_MAP: Record<ButtonSize, string> = {
    xs: "h-7 px-2 text-xs",
    sm: "h-8 px-3 text-sm",
    md: "h-10 px-4 text-sm",
    lg: "h-11 px-5 text-base",
    xl: "h-12 px-6 text-base",
};

const ICON_SIZE_MAP: Record<ButtonSize, string> = {
    xs: "size-7",
    sm: "size-8",
    md: "size-10",
    lg: "size-11",
    xl: "size-12",
};

export function getButtonClasses(opts: {
    color: ButtonColor;
    variant: ButtonVariant;
    isGlow?: boolean;
    isIcon?: boolean;
    className?: string;
    size?: ButtonSize;
}) {
    const { color, variant, isGlow, isIcon, className, size = "md" } = opts;

    const sizeClasses = isIcon ? cn("p-0", ICON_SIZE_MAP[size]) : SIZE_MAP[size];

    return cn(
        base,
        palette[color]?.[variant] ?? palette.neutral.filled,
        isGlow ? glowMap[color] : "",
        sizeClasses,
        className
    );
}
