import React from "react";
import { cn } from "@/utils/cn";

type SpinnerProps = React.HTMLAttributes<HTMLSpanElement>;

export function Spinner({ className, ...rest }: SpinnerProps) {
    return (
        <span
            aria-hidden
            className={cn(
                "inline-block align-middle h-4 w-4 animate-spin rounded-full border-2 border-current border-r-transparent",
                className
            )}
            {...rest}
        />
    );
}

type GhostSpinnerProps = SpinnerProps & {
    variant?: "default" | "soft" | "innerDot";
};

export function GhostSpinner({ className, variant = "default", ...rest }: GhostSpinnerProps) {
    if (variant === "innerDot") {
        return (
            <span className={cn("relative inline-grid place-items-center", className)} {...rest}>
        <span className="h-full w-full animate-spin rounded-full border border-current/40 border-t-transparent" />
        <span className="absolute h-[40%] w-[40%] rounded-full bg-current/70" />
      </span>
        );
    }

    return (
        <span
            aria-hidden
            className={cn(
                "inline-block align-middle h-4 w-4 animate-spin rounded-full",
                variant === "soft"
                    ? "border border-current/30 border-t-transparent"
                    : "border-2 border-current/80 border-r-transparent",
                className
            )}
            {...rest}
        />
    );
}
