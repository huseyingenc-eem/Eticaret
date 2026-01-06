import { useState, useMemo } from "react";
import { CheckCircle, ChevronDown, Search, X } from "lucide-react";
import { Dropdown, Button, Input, Checkbox } from "@/components/ui";

interface StatusFilterProps {
    value: string[];
    onChange: (value: string[]) => void;
    options: { value: string; label: string; color?: string }[];
    label?: string;
    disabled?: boolean;
}

export default function StatusFilter({
                                         value = [],
                                         onChange,
                                         options = [],
                                         label = "Durum",
                                         disabled = false
                                     }: StatusFilterProps) {
    const [search, setSearch] = useState("");
    const hasValue = value.length > 0;

    const buttonText = () => {
        if (!hasValue) return label;
        if (value.length === 1) {
            const option = options.find(opt => opt.value === value[0]);
            return option?.label || value[0];
        }
        if (value.length === options.length) return "Tümü Seçili";
        return `${value.length} durum seçili`;
    };

    const filteredOptions = useMemo(() =>
        options.filter(opt =>
            opt.label.toLowerCase().includes(search.toLowerCase())
        ), [options, search]);

    const handleToggle = (optionValue: string) => {
        const newValue = value.includes(optionValue)
            ? value.filter(v => v !== optionValue)
            : [...value, optionValue];
        onChange(newValue);
    };

    return (
        <Dropdown placement="bottom-start">
            <Dropdown.Trigger>
                <Button
                    variant={hasValue ? "filled" : "outline"}
                    color={hasValue ? "primary" : "neutral"}
                    size="sm"
                    disabled={disabled}
                    className="group gap-2"
                >
                    <CheckCircle className="h-4 w-4" />
                    {buttonText()}
                    <ChevronDown className="h-4 w-4 text-slate-400 group-hover:rotate-180 transition-transform" />
                </Button>
            </Dropdown.Trigger>

            <Dropdown.Content className="min-w-[300px] p-0" closeOnClickInside={false}>
                <div className="p-4 border-b border-slate-200">
                    <div className="relative">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" />
                        <Input
                            placeholder="Durum ara..."
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                            className="pl-9 bg-slate-50"
                            size="sm"
                        />
                    </div>
                </div>

                <div className="p-2 max-h-64 overflow-y-auto">
                    {filteredOptions.length > 0 ? filteredOptions.map((option) => (
                        <div
                            key={option.value}
                            className="flex items-center gap-3 p-2 rounded-md hover:bg-slate-50 cursor-pointer"
                            onClick={() => handleToggle(option.value)}
                        >
                            <Checkbox checked={value.includes(option.value)} readOnly size="sm" />
                            <span className="text-sm font-medium text-slate-700">{option.label}</span>
                        </div>
                    )) : (
                        <div className="text-center text-sm text-slate-500 p-4">Sonuç bulunamadı.</div>
                    )}
                </div>

                {hasValue && (
                    <div className="p-2 border-t border-slate-200">
                        <Button variant="ghost" size="sm" onClick={() => onChange([])} className="w-full text-red-600 hover:text-red-700 hover:bg-red-50">
                            <X className="h-4 w-4 mr-2" />
                            Seçimi Temizle
                        </Button>
                    </div>
                )}
            </Dropdown.Content>
        </Dropdown>
    );
}
