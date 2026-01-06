// src/components/ui/feedback/Loading.tsx
import React from "react";

type Props = {
    className?: string;
    rounded?: "none"|"sm"|"md"|"lg"|"xl"|"2xl"|"full";
    style?: React.CSSProperties;
};
export default function Loading({ className="", rounded="md", style }: Props) {
    const r = rounded === "full" ? "rounded-full"
        : rounded === "2xl" ? "rounded-2xl"
            : rounded === "xl" ? "rounded-xl"
                : rounded === "lg" ? "rounded-lg"
                    : rounded === "sm" ? "rounded-sm"
                        : rounded === "none" ? "" : "rounded-md";

    return <div className={`skeleton ${r} ${className}`} style={style} aria-hidden="true" />;
}
