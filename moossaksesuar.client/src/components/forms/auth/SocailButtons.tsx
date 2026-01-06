import { Button } from "@/components/ui";
import { cn } from "@/utils/cn";
import { FcGoogle } from "react-icons/fc";
import { FaApple } from "react-icons/fa";

type Provider = "google" | "apple";

type SocialButtonsProps = {
    onGoogle?: () => void | Promise<void>;
    onApple?: () => void | Promise<void>;
    size?: "xs" | "sm" | "md" | "lg" | "xl";
    loading?: Partial<Record<Provider, boolean>>;
    disabled?: boolean;
    className?: string;
    showDivider?: boolean;
    dividerText?: string;
};

export default function SocialButtons({
                                          onGoogle,
                                          onApple,
                                          size = "lg",
                                          loading,
                                          disabled,
                                          className,
                                          showDivider = true,
                                          dividerText = "veya",
                                      }: SocialButtonsProps) {
    // ikon boyutu: Button.isIcon default'unla uyumlu
    const iconSizeClass =
        {
            xs: "h-3.5 w-3.5",
            sm: "h-4 w-4",
            md: "h-5 w-5",
            lg: "h-6 w-6",
            xl: "h-7 w-7",
        }[size] || "h-4 w-4";

    return (
        <div className={cn("space-y-4", className)}>
            {showDivider && <Divider text={dividerText} />}

            {/* ortada hizala */}
            <div className="flex items-center justify-center gap-3">
                {/* Google (renkli ikon, metin yok) */}
                <Button
                    variant="ghost"
                    isIcon
                    size={size}
                    aria-label="Google ile devam et"
                    onClick={onGoogle}
                    disabled={disabled}
                    loading={Boolean(loading?.google)}
                    className="rounded-full"
                >
                    <FcGoogle className={cn(iconSizeClass)} />
                </Button>

                {/* Apple (tek renk ikon, metin yok) */}
                <Button
                    variant="ghost"
                    isIcon
                    size={size}
                    aria-label="Apple ile devam et"
                    onClick={onApple}
                    disabled={disabled}
                    loading={Boolean(loading?.apple)}
                    className="rounded-full"
                >
                    <FaApple className={cn(iconSizeClass)} />
                </Button>
            </div>
        </div>
    );
}

function Divider({ text }: { text: string }) {
    return (
        <div className="relative">
            <div className="absolute inset-0 flex items-center">
                <span className="w-full border-t" />
            </div>
            <div className="relative flex justify-center">
        <span className="bg-white px-2 text-xs uppercase text-gray-500">
          {text}
        </span>
            </div>
        </div>
    );
}
