// src/components/ui/Loading/Loading.tsx
import { cn } from '@/utils/cn';

interface LoadingProps {
    size?: 'sm' | 'md' | 'lg' | 'xl';
    variant?: 'spinner' | 'dots' | 'pulse';
    text?: string;
    className?: string;
}

const Loading = ({
                     size = 'md',
                     variant = 'spinner',
                     text,
                     className
                 }: LoadingProps) => {
    const sizeStyles = {
        sm: 'w-4 h-4',
        md: 'w-6 h-6',
        lg: 'w-8 h-8',
        xl: 'w-12 h-12'
    };

    const textSizeStyles = {
        sm: 'text-sm',
        md: 'text-base',
        lg: 'text-lg',
        xl: 'text-xl'
    };

    const renderSpinner = () => (
        <div
            className={cn(
                "animate-spin rounded-full border-2 border-gray-300 border-t-blue-600",
                sizeStyles[size]
            )}
        />
    );

    const renderDots = () => (
        <div className="flex space-x-1">
            {[0, 1, 2].map((i) => (
                <div
                    key={i}
                    className={cn(
                        "rounded-full bg-blue-600 animate-pulse",
                        size === 'sm' && "w-1 h-1",
                        size === 'md' && "w-2 h-2",
                        size === 'lg' && "w-3 h-3",
                        size === 'xl' && "w-4 h-4"
                    )}
                    style={{ animationDelay: `${i * 0.2}s` }}
                />
            ))}
        </div>
    );

    const renderPulse = () => (
        <div
            className={cn(
                "rounded-full bg-blue-600 animate-pulse",
                sizeStyles[size]
            )}
        />
    );

    const renderLoader = () => {
        switch (variant) {
            case 'dots':
                return renderDots();
            case 'pulse':
                return renderPulse();
            default:
                return renderSpinner();
        }
    };

    return (
        <div className={cn("flex flex-col items-center justify-center", className)}>
            {renderLoader()}
            {text && (
                <p className={cn("mt-2 text-gray-600", textSizeStyles[size])}>
                    {text}
                </p>
            )}
        </div>
    );
};

export default Loading;