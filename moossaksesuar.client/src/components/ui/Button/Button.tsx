import React, { forwardRef, type ElementType } from "react";
import { cn } from "@/utils/cn";
import type { PolymorphicProps } from "./types";
import { getButtonClasses } from "./styles";

const ButtonInner = <C extends ElementType = "button">(
    {
        component,
        className,
        children,
        color = "neutral",
        isIcon = false,
        variant = "filled",
        unstyled = false,
        type,
        isGlow = false,
        loading = false,
        disabled,
        size = "md",
        ...rest
    }: PolymorphicProps<C>,
    ref: React.Ref<unknown>
) => {
    const Comp = (component || "button") as ElementType;

    const computedType =
        (Comp === "button" ? (type ?? "button") : type) as
            | "button"
            | "submit"
            | "reset"
            | undefined;

    if (unstyled) {
        return (
            <Comp
                ref={ref}
                type={computedType as unknown}
                className={className}
                disabled={disabled || loading}
                aria-disabled={disabled || loading || undefined}
                aria-busy={loading || undefined}
                {...rest}
            >
                {children}
            </Comp>
        );
    }

    const classes = getButtonClasses({
        color,
        variant,
        isGlow,
        isIcon,
        className,
        size, // 🔧 eklendi
    });

    const spinnerSize =
        {
            xs: "h-3 w-3",
            sm: "h-3.5 w-3.5",
            md: "h-4 w-4",
            lg: "h-5 w-5",
            xl: "h-5 w-5",
        }[size] || "h-4 w-4";

    return (
        <Comp
            ref={ref}
            type={computedType as unknown}
            className={classes}
            disabled={disabled || loading}
            aria-disabled={disabled || loading || undefined}
            aria-busy={loading || undefined}
            data-variant={variant}
            data-color={color}
            data-size={size}
            {...rest}
        >
            {loading && (
                <span
                    className={cn(
                        "mr-1 inline-flex animate-spin rounded-full border-2 border-current border-r-transparent",
                        spinnerSize
                    )}
                />
            )}
            <span className={cn("inline-flex items-center gap-2 whitespace-nowrap overflow-hidden", "[&>svg]:shrink-0 [&>svg]:inline-block", loading && "opacity-80")}>
                {children}
            </span>
        </Comp>
    );
};

const Button = forwardRef(ButtonInner) as <C extends ElementType = "button">(
    props: PolymorphicProps<C>
) => React.ReactElement | null;


export default Button;
