// src/components/ui/Button/types.ts
import { type ElementType, type ComponentPropsWithoutRef } from "react";

export type ButtonColor =
    | "neutral"
    | "primary"
    | "secondary"
    | "info"
    | "success"
    | "warning"
    | "danger"
    | "error";

export type ButtonVariant = "filled"
    | "outline"
    | "soft"
    | "ghost";

export type ButtonSize = "xs" | "sm" | "md" | "lg" | "xl";

export type CommonProps = {
    component?: ElementType;
    className?: string;
    children?: React.ReactNode;
    color?: ButtonColor;
    isIcon?: boolean;
    variant?: ButtonVariant;
    unstyled?: boolean;
    type?: "button" | "submit" | "reset";
    isGlow?: boolean;
    loading?: boolean;
    disabled?: boolean;
};

export type PolymorphicProps<C extends ElementType> =
    Omit<ComponentPropsWithoutRef<C>, keyof CommonProps> &
    Omit<CommonProps, "component"> & {
    component?: C;
    size?: ButtonSize;
};
