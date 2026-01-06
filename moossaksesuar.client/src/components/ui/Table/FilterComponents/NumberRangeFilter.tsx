import { useState, useEffect } from "react";
import { Hash, ChevronDown, X } from "lucide-react";
import { Dropdown, Button, Input } from "@/components/ui";

interface NumberRangeFilterProps {
    value: { min?: number; max?: number };
    onChange: (value: { min?: number; max?: number }) => void;
    label?: string;
    disabled?: boolean;
    formatValue?: (value: number) => string;
    placeholder?: { min?: string; max?: string };
}

export default function NumberRangeFilter({
                                              value = {},
                                              onChange,
                                              label = "Sayı Aralığı",
                                              disabled = false,
                                              formatValue,
                                              placeholder = { min: "Min", max: "Max" }
                                          }: NumberRangeFilterProps) {
    const [localValue, setLocalValue] = useState(value);

    useEffect(() => {
        setLocalValue(value);
    }, [value]);

    const hasValue = value.min != null || value.max != null;

    const buttonText = () => {
        if (!hasValue) return label;
        const formatNum = (num: number) => formatValue ? formatValue(num) : num.toLocaleString('tr-TR');
        if (value.min != null && value.max != null) return `${formatNum(value.min)} - ${formatNum(value.max)}`;
        if (value.min != null) return `${formatNum(value.min)} ve üzeri`;
        if (value.max != null) return `${formatNum(value.max)} ve altı`;
        return label;
    };

    const handleApply = () => onChange(localValue);
    const handleClear = () => {
        setLocalValue({});
        onChange({});
    };

    return (
        <Dropdown placement="bottom-start">
            <Dropdown.Trigger>
                <Button
                    variant={hasValue ? "filled" : "outline"}
                    color={hasValue ? "primary" : "neutral"}
                    size="sm"
                    disabled={disabled}
                    className={`group gap-2 ${hasValue ? 'shadow-lg shadow-blue-500/20' : ''}`}
                >
                    <Hash className="h-4 w-4" />
                    <span className="max-w-48 truncate">{buttonText()}</span>
                    <ChevronDown className="h-4 w-4 text-slate-400 group-hover:rotate-180 transition-transform" />
                </Button>
            </Dropdown.Trigger>

            <Dropdown.Content className="min-w-[320px]" closeOnClickInside={false}>
                <div className="p-5">
                    <div className="flex items-center justify-between mb-4">
                        <div className="flex items-center gap-2">
                            <Hash className="h-4 w-4 text-blue-500" />
                            <span className="text-sm font-semibold text-slate-900">{label}</span>
                        </div>
                        {hasValue && (
                            <button onClick={handleClear} className="p-1 hover:bg-slate-100 rounded-full group" title="Temizle">
                                <X className="h-3 w-3 text-slate-400 group-hover:text-red-500" />
                            </button>
                        )}
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-xs font-medium text-slate-700 mb-2">Minimum</label>
                            <Input
                                type="number"
                                value={localValue.min ?? ""}
                                onChange={(e) => setLocalValue(p => ({ ...p, min: e.target.value === '' ? undefined : Number(e.target.value) }))}
                                placeholder={placeholder.min}
                                size="sm" className="bg-slate-50"
                            />
                        </div>
                        <div>
                            <label className="block text-xs font-medium text-slate-700 mb-2">Maksimum</label>
                            <Input
                                type="number"
                                value={localValue.max ?? ""}
                                onChange={(e) => setLocalValue(p => ({ ...p, max: e.target.value === '' ? undefined : Number(e.target.value) }))}
                                placeholder={placeholder.max}
                                size="sm" className="bg-slate-50"
                            />
                        </div>
                    </div>

                    <div className="flex gap-2 mt-6 pt-4 border-t border-slate-200">
                        <Button variant="filled" color="primary" size="sm" onClick={handleApply} className="flex-1">
                            Uygula
                        </Button>
                    </div>
                </div>
            </Dropdown.Content>
        </Dropdown>
    );
}
