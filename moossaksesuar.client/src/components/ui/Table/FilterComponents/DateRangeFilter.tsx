import { useState, useEffect } from "react";
import { Calendar, ChevronDown, X } from "lucide-react";
import { Dropdown, Button, Input } from "@/components/ui";

interface DateRangeFilterProps {
    value: { from?: string; to?: string };
    onChange: (value: { from?: string; to?: string }) => void;
    label?: string;
    disabled?: boolean;
}

export default function DateRangeFilter({
                                            value = {},
                                            onChange,
                                            label = "Tarih Aralığı",
                                            disabled = false
                                        }: DateRangeFilterProps) {
    // Dropdown içindeki geçici değeri tutar, sadece "Uygula" denince dışarıya yansır.
    const [localValue, setLocalValue] = useState(value);

    // Ana bileşenden gelen 'value' prop'u değişirse, iç state'i de güncelleyelim.
    // Bu, "Tümünü Temizle" gibi dışarıdan gelen değişikliklere uyum sağlar.
    useEffect(() => {
        setLocalValue(value);
    }, [value]);

    const hasValue = value.from || value.to;

    // Ana düğmenin metnini formatlar.
    const buttonText = () => {
        if (!hasValue) return label;
        const formatDate = (dateStr: string) => new Date(dateStr).toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' });

        if (value.from && value.to) {
            if (value.from === value.to) return formatDate(value.from);
            return `${formatDate(value.from)} - ${formatDate(value.to)}`;
        }
        if (value.from) return `${formatDate(value.from)} sonrası`;
        if (value.to) return `${formatDate(value.to)} öncesi`;
        return label;
    };

    // "Uygula" butonuna basıldığında state'i dışarı aktarır.
    const handleApply = () => {
        onChange(localValue);
    };

    // "Temizle" butonuna basıldığında hem iç hem de dış state'i temizler.
    const handleClear = () => {
        const cleared = {};
        setLocalValue(cleared);
        onChange(cleared);
    };

    // Hızlı tarih seçenekleri için mantık
    const quickOptions = [
        {
            label: 'Bugün',
            getValue: () => ({ from: new Date().toISOString().split('T')[0], to: new Date().toISOString().split('T')[0] })
        },
        {
            label: 'Bu Hafta',
            getValue: () => {
                const now = new Date();
                const dayOfWeek = now.getDay() === 0 ? 6 : now.getDay() - 1; // Pzt=0, Pzr=6
                const firstDay = new Date(now.setDate(now.getDate() - dayOfWeek));
                const lastDay = new Date(firstDay);
                lastDay.setDate(lastDay.getDate() + 6);
                return { from: firstDay.toISOString().split('T')[0], to: lastDay.toISOString().split('T')[0] };
            }
        },
        {
            label: 'Bu Ay',
            getValue: () => {
                const now = new Date();
                const firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
                const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0);
                return { from: firstDay.toISOString().split('T')[0], to: lastDay.toISOString().split('T')[0] };
            }
        },
        {
            label: 'Son 30 Gün',
            getValue: () => {
                const now = new Date();
                const to = new Date(now);
                const from = new Date(now.setDate(now.getDate() - 29));
                return { from: from.toISOString().split('T')[0], to: to.toISOString().split('T')[0] };
            }
        }
    ];

    return (
        <Dropdown placement="bottom-start">
            <Dropdown.Trigger>
                <Button
                    variant={hasValue ? "filled" : "outline"}
                    color={hasValue ? "primary" : "neutral"}
                    size="sm"
                    disabled={disabled}
                    className={`gap-2 group ${hasValue ? 'shadow-lg shadow-blue-500/20' : ''}`}
                >
                    <Calendar className="h-4 w-4" />
                    <span className="max-w-48 truncate">{buttonText()}</span>
                    <ChevronDown className="h-4 w-4 text-slate-400 group-hover:rotate-180 transition-transform" />
                </Button>
            </Dropdown.Trigger>

            <Dropdown.Content className="min-w-[380px]" closeOnClickInside={false}>
                <div className="p-5">
                    {/* Başlık ve Temizle Butonu */}
                    <div className="flex items-center justify-between mb-4">
                        <div className="flex items-center gap-2">
                            <Calendar className="h-4 w-4 text-blue-500" />
                            <span className="text-sm font-semibold text-slate-900">{label}</span>
                        </div>
                        {hasValue && (
                            <button onClick={handleClear} className="p-1 hover:bg-slate-100 rounded-full group" title="Temizle">
                                <X className="h-3 w-3 text-slate-400 group-hover:text-red-500" />
                            </button>
                        )}
                    </div>

                    {/* Hızlı Seçimler */}
                    <div className="mb-4">
                        <div className="text-xs font-medium text-slate-700 mb-2">Hızlı Seçim</div>
                        <div className="grid grid-cols-2 gap-2">
                            {quickOptions.map((option) => (
                                <Button
                                    key={option.label}
                                    variant="ghost"
                                    size="sm"
                                    className="text-xs justify-start bg-slate-50 hover:bg-blue-50 hover:text-blue-700"
                                    onClick={() => setLocalValue(option.getValue())}
                                >
                                    {option.label}
                                </Button>
                            ))}
                        </div>
                    </div>

                    {/* Manuel Tarih Seçimi */}
                    <div className="space-y-4">
                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <label className="block text-xs font-medium text-slate-700 mb-2">Başlangıç Tarihi</label>
                                <Input
                                    type="date"
                                    value={localValue.from || ""}
                                    onChange={(e) => setLocalValue(prev => ({ ...prev, from: e.target.value || undefined }))}
                                    size="sm"
                                    className="bg-slate-50"
                                />
                            </div>
                            <div>
                                <label className="block text-xs font-medium text-slate-700 mb-2">Bitiş Tarihi</label>
                                <Input
                                    type="date"
                                    value={localValue.to || ""}
                                    onChange={(e) => setLocalValue(prev => ({ ...prev, to: e.target.value || undefined }))}
                                    size="sm"
                                    className="bg-slate-50"
                                />
                            </div>
                        </div>
                    </div>

                    {/* Aksiyon Butonları */}
                    <div className="flex gap-2 mt-6 pt-4 border-t border-slate-200">
                        <Button variant="outline" size="sm" onClick={() => { setLocalValue(value); /* Dropdown'ı kapatacak bir şey ekleyin, örn. ref.current.close() */ }} className="flex-1">
                            Vazgeç
                        </Button>
                        <Button variant="filled" color="primary" size="sm" onClick={handleApply} className="flex-1">
                            Uygula
                        </Button>
                    </div>
                </div>
            </Dropdown.Content>
        </Dropdown>
    );
}
