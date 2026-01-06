import React from "react";
import type { DateRangeValue, NumberRangeValue, SelectValue } from "./types";

interface BaseInputProps {
    className?: string;
    size?: "sm" | "md" | "lg";
}

const Input: React.FC<React.InputHTMLAttributes<HTMLInputElement> & BaseInputProps> = ({
                                                                                           className,
                                                                                           size = "md",
                                                                                           ...props
                                                                                       }) => {
    const sizeClasses = {
        sm: "px-2 py-1.5 text-xs",
        md: "px-3 py-2 text-sm",
        lg: "px-4 py-3 text-base"
    };

    return (
        <input
            className={`
        w-full rounded-lg border border-slate-300 bg-white
        focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20
        transition-all duration-200 outline-none
        ${sizeClasses[size]}
        ${className || ""}
      `}
            {...props}
        />
    );
};

/** Metin filtresi */
export const TextFilter: React.FC<{
    value: string;
    onChange: (v: string) => void;
    placeholder?: string;
}> = ({ value, onChange, placeholder }) => (
    <Input
        size="sm"
        type="text"
        value={value ?? ""}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder ?? "Ara..."}
    />
);

/** Sayı aralığı filtresi */
export const NumberRangeFilter: React.FC<{
    value: NumberRangeValue;
    onChange: (v: NumberRangeValue) => void;
}> = ({ value, onChange }) => (
    <div className="flex items-center gap-2">
        <Input
            size="sm"
            type="number"
            className="flex-1"
            value={value?.min ?? ""}
            onChange={(e) =>
                onChange({
                    ...value,
                    min: e.target.value === "" ? undefined : Number(e.target.value),
                })
            }
            placeholder="Min"
        />
        <span className="text-slate-400 text-xs">-</span>
        <Input
            size="sm"
            type="number"
            className="flex-1"
            value={value?.max ?? ""}
            onChange={(e) =>
                onChange({
                    ...value,
                    max: e.target.value === "" ? undefined : Number(e.target.value),
                })
            }
            placeholder="Max"
        />
    </div>
);

/** Tarih aralığı filtresi */
export const DateRangeFilter: React.FC<{
    value: DateRangeValue;
    onChange: (v: DateRangeValue) => void;
}> = ({ value, onChange }) => (
    <div className="flex items-center gap-2">
        <Input
            size="sm"
            type="date"
            className="flex-1"
            value={value?.from ?? ""}
            onChange={(e) => onChange({ ...value, from: e.target.value || undefined })}
        />
        <span className="text-slate-400 text-xs">-</span>
        <Input
            size="sm"
            type="date"
            className="flex-1"
            value={value?.to ?? ""}
            onChange={(e) => onChange({ ...value, to: e.target.value || undefined })}
        />
    </div>
);

/** Select filtresi */
export const SelectFilter: React.FC<{
    value: SelectValue;
    onChange: (v: SelectValue) => void;
    options: { label: string; value: string }[];
    multiple?: boolean;
    placeholder?: string;
}> = ({ value, onChange, options, multiple, placeholder = "Seçiniz" }) => {
    const v = multiple ? ((value as string[]) ?? []) : ((value as string) ?? "");

    return (
        <select
            multiple={!!multiple}
            value={v as any}
            onChange={(e) => {
                if (multiple) {
                    const vals = Array.from(e.currentTarget.selectedOptions).map((o) => o.value);
                    onChange(vals);
                } else {
                    onChange(e.currentTarget.value);
                }
            }}
            className="
        w-full rounded-lg border border-slate-300 bg-white px-2 py-1.5 text-sm
        focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20
        transition-all duration-200 outline-none
      "
        >
            {!multiple && <option value="">{placeholder}</option>}
            {options.map((o) => (
                <option key={o.value} value={o.value}>
                    {o.label}
                </option>
            ))}
        </select>
    );
};

/** Builtin filtre renderer */
export const RenderBuiltinFilter: React.FC<
    { kind: "text" | "numberRange" | "dateRange" | "select" } & Record<string, any>
> = ({ kind, ...props }) => {
    switch (kind) {
        case "text":
            return <TextFilter {...(props as any)} />;
        case "numberRange":
            return <NumberRangeFilter {...(props as any)} />;
        case "dateRange":
            return <DateRangeFilter {...(props as any)} />;
        case "select":
            return <SelectFilter {...(props as any)} />;
        default:
            return null;
    }
};

export default RenderBuiltinFilter;