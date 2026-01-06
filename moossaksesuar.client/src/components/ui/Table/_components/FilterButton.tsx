import { useMemo, useRef } from "react"; // useState kaldırıldı, useRef eklendi
import { ChevronDown, X } from "lucide-react";
import type { ColumnDef, FilterValue } from "../types";
import {Button, Dropdown} from "@/components/ui";

// Dropdown'unuzun ref üzerinden sunduğu metodların tipini tanımlıyoruz.
// Eğer bu tipi Dropdown.tsx'den export ediyorsanız, onu import edebilirsiniz.
type DropdownHandle = {
    open: () => void;
    close: () => void;
    toggle: () => void;
};

interface Props {
    column: ColumnDef<any>;
    value: FilterValue;
    onChange: (value: FilterValue) => void;
}

export default function FilterButton({ column, value, onChange }: Props) {
    // Dropdown'ı programatik olarak kontrol etmek için bir ref oluşturuyoruz.
    const dropdownRef = useRef<DropdownHandle>(null);

    const hasValue = useMemo(() => {
        if (!value) return false;
        if (Array.isArray(value)) return value.length > 0;
        if (typeof value === 'object') {
            return Object.values(value).some(v => v != null && v !== '');
        }
        return value !== '' && value != null;
    }, [value]);

    const buttonLabel = column.filter?.buttonLabel || column.header;

    const getValueDisplay = () => {
        if (!hasValue) return null;

        if (column.filter?.type === 'select' && Array.isArray(value)) {
            return value.length === 1 ? value[0] : `${value.length} seçili`;
        }

        if (column.filter?.type === 'numberRange' && value) {
            const { min, max } = value as any;
            if (min != null && max != null) return `${min}-${max}`;
            if (min != null) return `${min}+`;
            if (max != null) return `≤${max}`;
        }

        if (column.filter?.type === 'dateRange' && value) {
            const { from, to } = value as any;
            if (from && to) return `${from} - ${to}`;
            if (from) return `${from} sonrası`;
            if (to) return `${to} öncesi`;
        }

        if (typeof value === 'string' && value.length > 15) {
            return value.substring(0, 15) + '...';
        }

        return String(value);
    };

    const valueDisplay = getValueDisplay();

    const handleClear = () => {
        onChange(undefined);
        // Değeri temizledikten sonra ref üzerinden dropdown'ı kapatıyoruz.
        dropdownRef.current?.close();
    };

    return (
        <Dropdown ref={dropdownRef} placement="bottom-start">
            <Dropdown.Trigger>
                <Button
                    variant="outline"
                    color={hasValue ? 'primary' : 'neutral'}
                    size="sm"
                >
                    <span>{buttonLabel}</span>
                    {valueDisplay && (
                        <>
                            <span className="text-slate-400">:</span>
                            <span className="font-semibold">{valueDisplay}</span>
                        </>
                    )}
                    <ChevronDown className="h-4 w-4" />
                </Button>
            </Dropdown.Trigger>
            <Dropdown.Content closeOnClickInside={false}>
                <div className="space-y-3 min-w-[250px]">
                    <div className="flex items-center justify-between">
                        <h4 className="font-medium text-slate-700">{buttonLabel}</h4>
                        {hasValue && (
                            <Button
                                isIcon
                                variant="ghost"
                                color="neutral"
                                size="xs"
                                onClick={handleClear}
                                title="Temizle"
                            >
                                <X className="h-3 w-3" />
                            </Button>
                        )}
                    </div>

                    {column.filter?.renderer && column.filter.renderer(value, onChange)}
                </div>
            </Dropdown.Content>
        </Dropdown>
    );
}