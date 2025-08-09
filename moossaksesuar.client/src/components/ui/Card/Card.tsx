// src/components/ui/Card/Card.tsx
import { forwardRef, type HTMLAttributes } from 'react';
import { cn } from '@/utils/cn';

interface CardProps extends HTMLAttributes<HTMLDivElement> {
    variant?: 'default' | 'outlined' | 'elevated';
    padding?: 'none' | 'sm' | 'md' | 'lg';
}

const Card = forwardRef<HTMLDivElement, CardProps>(({
                                                        className,
                                                        variant = 'default',
                                                        padding = 'md',
                                                        ...props
                                                    }, ref) => {
    const baseStyles = "bg-white rounded-lg";

    const variantStyles = {
        default: "",
        outlined: "border-2 border-gray-300",
        elevated: "shadow-lg",
    };

    const paddingStyles = {
        none: "",
        sm: "p-3",
        md: "p-6",
        lg: "p-8"
    };

    return (
        <div
            ref={ref}
            className={cn(
                baseStyles,
                variantStyles[variant],
                paddingStyles[padding],
                className
            )}
            {...props}
        />
    );
});

Card.displayName = 'Card';

export default Card;

// Card alt componentleri
export const CardHeader = forwardRef<HTMLDivElement, HTMLAttributes<HTMLDivElement>>(({
                                                                                          className,
                                                                                          ...props
                                                                                      }, ref) => (
    <div
        ref={ref}
        className={cn("flex flex-col space-y-1.5 p-6", className)}
        {...props}
    />
));

CardHeader.displayName = 'CardHeader';

export const CardTitle = forwardRef<HTMLParagraphElement, HTMLAttributes<HTMLHeadingElement>>(({
                                                                                                   className,
                                                                                                   ...props
                                                                                               }, ref) => (
    <h3
        ref={ref}
        className={cn("text-lg font-semibold leading-none tracking-tight", className)}
        {...props}
    />
));

CardTitle.displayName = 'CardTitle';

export const CardContent = forwardRef<HTMLDivElement, HTMLAttributes<HTMLDivElement>>(({
                                                                                           className,
                                                                                           ...props
                                                                                       }, ref) => (
    <div
        ref={ref}
        className={cn("p-6 pt-0", className)}
        {...props}
    />
));

CardContent.displayName = 'CardContent';

export const CardFooter = forwardRef<HTMLDivElement, HTMLAttributes<HTMLDivElement>>(({
                                                                                          className,
                                                                                          ...props
                                                                                      }, ref) => (
    <div
        ref={ref}
        className={cn("flex items-center p-6 pt-0", className)}
        {...props}
    />
));

CardFooter.displayName = 'CardFooter';