// src/components/ui/Checkbox/Checkbox.tsx
import { forwardRef, type InputHTMLAttributes } from 'react';
import { cn } from '@/utils/cn';

interface CheckboxProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'type'| 'size'> {
    label?: React.ReactNode;
    description?: string;
    error?: string;
    size?: 'sm' | 'md' | 'lg';
}

const Checkbox = forwardRef<HTMLInputElement, CheckboxProps>(({
                                                                  className,
                                                                  label,
                                                                  description,
                                                                  error,
                                                                  size = 'md',
                                                                  ...props
                                                              }, ref) => {
    const sizeStyles = {
        sm: "w-3 h-3",
        md: "w-4 h-4",
        lg: "w-5 h-5"
    };

    const checkboxClasses = cn(
        "text-blue-600 bg-gray-100 border-gray-300 rounded focus:ring-blue-500 focus:ring-2",
        sizeStyles[size],
        error && "border-red-300 bg-red-50",
        className
    );

    return (
        <div className="flex items-start">
            <div className="flex items-center h-5">
                <input
                    type="checkbox"
                    ref={ref}
                    className={checkboxClasses}
                    {...props}
                />
            </div>
            {(label || description) && (
                <div className="ml-3">
                    {label && (
                        <label className={cn(
                            "font-medium text-gray-700",
                            size === 'sm' && "text-sm",
                            size === 'md' && "text-sm",
                            size === 'lg' && "text-base"
                        )}>
                            {label}
                        </label>
                    )}
                    {description && (
                        <p className={cn(
                            "text-gray-500",
                            size === 'sm' && "text-xs",
                            size === 'md' && "text-sm",
                            size === 'lg' && "text-sm"
                        )}>
                            {description}
                        </p>
                    )}
                    {error && (
                        <p className="mt-1 text-sm text-red-600">{error}</p>
                    )}
                </div>
            )}
        </div>
    );
});

Checkbox.displayName = 'Checkbox';

export default Checkbox;