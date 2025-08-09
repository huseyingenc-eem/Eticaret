import { forwardRef, type InputHTMLAttributes } from 'react';
import { cn } from '@/utils/cn';

interface InputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'size'> {
    label?: string;
    error?: string;
    leftIcon?: React.ReactNode;
    rightIcon?: React.ReactNode;
    variant?: 'default' | 'outlined' | 'filled';
    size?: 'sm' | 'md' | 'lg';
    fullWidth?: boolean;
}

const Input = forwardRef<HTMLInputElement, InputProps>(({
                                                            className,
                                                            type = 'text',
                                                            label,
                                                            error,
                                                            leftIcon,
                                                            rightIcon,
                                                            variant = 'default',
                                                            size = 'md',
                                                            fullWidth = true,
                                                            ...props
                                                        }, ref) => {
    const baseStyles = "border rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500";

    const variantStyles = {
        default: "border-gray-300 bg-white",
        outlined: "border-2 border-gray-300 bg-transparent",
        filled: "border-gray-200 bg-gray-50"
    };

    const sizeStyles = {
        sm: "px-3 py-1.5 text-sm",
        md: "px-3 py-2 text-base",
        lg: "px-4 py-3 text-lg"
    };

    const errorStyles = error
        ? "border-red-300 bg-red-50 focus:ring-red-500 focus:border-red-500"
        : "";

    const inputClasses = cn(
        baseStyles,
        variantStyles[variant],
        sizeStyles[size],
        errorStyles,
        leftIcon && "pl-10",
        rightIcon && "pr-10",
        fullWidth && "w-full",
        className
    );

    return (
        <div className={cn("relative", fullWidth && "w-full")}>
            {label && (
                <label className="block text-sm font-medium text-gray-700 mb-1">
                    {label}
                </label>
            )}

            <div className="relative">
                {leftIcon && (
                    <div className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400">
                        {leftIcon}
                    </div>
                )}

                <input
                    type={type}
                    ref={ref}
                    className={inputClasses}
                    {...props}
                />

                {rightIcon && (
                    <div className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400">
                        {rightIcon}
                    </div>
                )}
            </div>

            {error && (
                <p className="mt-1 text-sm text-red-600">{error}</p>
            )}
        </div>
    );
});

Input.displayName = 'Input';

export default Input;