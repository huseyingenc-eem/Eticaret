// src/components/ui/Avatar/Avatar.tsx
import * as React from "react";

type AvatarSize = 6 | 8 | 10 | 12 | 14 | 16 | 20 | 24 | 28 | 32;

const SIZE_TO_CLASS: Record<AvatarSize, string> = {
    6: "w-6 h-6",
    8: "w-8 h-8",
    10: "w-10 h-10",
    12: "w-12 h-12",
    14: "w-14 h-14",
    16: "w-16 h-16",
    20: "w-20 h-20",
    24: "w-24 h-24",
    28: "w-28 h-28",
    32: "w-32 h-32",
};

export type AvatarProps = {
    src?: string;
    alt?: string;
    name?: string;
    size?: AvatarSize;
    className?: string;
    imgClassName?: string;
    classNames?: {
        root?: string;
        img?: string;
        display?: string;
    };
    fallbackIcon?: React.ReactNode;
    withRing?: boolean;
} & React.HTMLAttributes<HTMLDivElement>;

function getInitials(text?: string) {
    if (!text) return "";
    const parts = text.trim().split(/\s+/).slice(0, 2);
    return parts.map(p => p[0]?.toUpperCase()).join("");
}

const Avatar: React.FC<AvatarProps> = ({
                                           src,
                                           alt,
                                           name,
                                           size = 10,
                                           className,
                                           imgClassName,
                                           classNames,
                                           fallbackIcon,
                                           withRing = false,
                                           ...rest
                                       }) => {
    const [error, setError] = React.useState(false);
    const showFallback = !src || error;

    const initials = getInitials(name || alt);
    const sizeCls = SIZE_TO_CLASS[size] ?? SIZE_TO_CLASS[10];

    return (
        <div
            className={[
                "relative inline-flex shrink-0 select-none items-center justify-center overflow-hidden bg-gray-100 text-gray-600",
                "rounded-full",
                sizeCls,
                withRing ? "ring-1 ring-gray-200" : "",
                classNames?.root ?? "",
                className ?? "",
            ].join(" ")}
            {...rest}
        >
            {showFallback ? (
                fallbackIcon ? (
                    <span className="flex items-center justify-center">{fallbackIcon}</span>
                ) : (
                    <span className="grid h-full w-full place-items-center text-xs font-medium">
            {initials || "?"}
          </span>
                )
            ) : (
                <img
                    src={src}
                    alt={alt}
                    onError={() => setError(true)}
                    className={[
                        "h-full w-full object-cover",
                        classNames?.display ?? "",
                        classNames?.img ?? "",
                        imgClassName ?? "",
                    ].join(" ")}
                    loading="lazy"
                    decoding="async"
                    draggable={false}
                />
            )}
        </div>
    );
};

export default Avatar;
