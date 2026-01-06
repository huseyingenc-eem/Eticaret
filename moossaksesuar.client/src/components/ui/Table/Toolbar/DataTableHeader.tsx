import { Settings, Download, Printer, ChevronDown, Eye, EyeOff, Database } from "lucide-react";
import type { ColumnDef } from "../types";
import {Dropdown, Button} from "@/components/ui";

interface HeaderProps {
    title?: string;
    description?: string;
    onExport: (format: 'csv' | 'xlsx' | 'pdf') => void;
    onPrint: () => void;
    columns: ColumnDef<any>[];
    visibleColumns: Record<string, boolean>;
    onVisibleColumnsChange: (columns: Record<string, boolean>) => void;
    pageSize: number;
    onPageSizeChange: (size: number) => void;
    density: "compact" | "normal" | "comfortable";
    customSettingsContent?: React.ReactNode;
    totalRecords?: number;
    currentRecords?: number;
}

export default function DataTableHeader({
                                            title,
                                            description,
                                            onExport,
                                            onPrint,
                                            columns,
                                            visibleColumns,
                                            onVisibleColumnsChange,
                                            pageSize,
                                            customSettingsContent,
                                            totalRecords,
                                        }: HeaderProps) {
    const visibleColumnCount = Object.values(visibleColumns).filter(Boolean).length;
    const totalColumnCount = columns.length;

    const settingsContent = customSettingsContent || (
        <div className="w-60">
            {/* Kolon Yönetimi */}
            <div className="p-5 border-b border-slate-100">
                <div className="flex items-center justify-between mb-4">
                    <div className="flex items-center gap-3">
                        <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-blue-600 rounded-lg flex items-center justify-center">
                            <Eye className="h-4 w-4 text-white" />
                        </div>
                        <div>
                            <h4 className="text-sm font-semibold text-slate-900">Kolon Görünürlüğü</h4>
                            <p className="text-xs text-slate-500">Tabloyu özelleştir</p>
                        </div>
                    </div>
                    <div className="flex items-center gap-2">
                        <span className="text-xs text-slate-500 bg-slate-100 px-2 py-1 rounded-full font-medium">
                            {visibleColumnCount}/{totalColumnCount}
                        </span>
                    </div>
                </div>

                <div className="space-y-1 max-h-72 overflow-y-auto pr-2 scrollbar-thin scrollbar-track-slate-100 scrollbar-thumb-slate-300">
                    {columns.map((col) => {
                        // 'col' objesinin tanımsız olma ihtimaline karşı bir kontrol ekleyelim.
                        if (!col || !col.id) return null;

                        const isVisible = visibleColumns[col.id] !== false;

                        return (
                            // Her bir satırın kapsayıcısı. px-2 ile soldan boşluk, py-1.5 ile dikey boşluk azaltıldı.
                            <div key={col.id} className={`group rounded-lg transition-all`}>
                                <label className="flex items-center justify-between w-full p-2 py-1.5 rounded-lg cursor-pointer transition-colors hover:bg-slate-100">
                                    <div className="flex items-center gap-3">
                                        {/* Checkbox'ın kendisi */}
                                        <input
                                            type="checkbox"
                                            className="h-4 w-4 rounded border-slate-300 text-blue-600 focus:ring-2 focus:ring-offset-0 focus:ring-blue-500/50"
                                            checked={isVisible}
                                            onChange={(e) => onVisibleColumnsChange({
                                                ...visibleColumns,
                                                [col.id]: e.target.checked
                                            })}
                                        />
                                        {/* Sütun Adı */}
                                        <span className={`text-sm font-medium transition-colors ${
                                            isVisible ? 'text-slate-800' : 'text-slate-500'
                                        }`}>
                            {col.header}
                        </span>
                                    </div>

                                    {/* Göz İkonu (sağa yaslı) */}
                                    <div className={`w-6 h-6 rounded-full flex items-center justify-center transition-colors ${
                                        isVisible ? 'bg-blue-100' : 'bg-slate-200'
                                    }`}>
                                        {isVisible ? (
                                            <Eye className="h-3.5 w-3.5 text-blue-600" />
                                        ) : (
                                            <EyeOff className="h-3.5 w-3.5 text-slate-400" />
                                        )}
                                    </div>
                                </label>
                            </div>
                        );
                    })}
                </div>

                <div className="flex gap-2 mt-4">
                    <Button
                        variant="ghost"
                        size="sm"
                        className="flex-1 bg-green-50 hover:bg-green-100 text-green-700"
                        onClick={() => {
                            const allVisible = columns.reduce((acc, col) => ({
                                ...acc,
                                [col.id]: true
                            }), {});
                            onVisibleColumnsChange(allVisible);
                        }}
                    >
                        <Eye className="h-3 w-3" />
                        Tümünü Göster
                    </Button>
                </div>
            </div>
        </div>
    );

    return (
        <div className="bg-gradient-to-br from-white via-slate-50 to-slate-100 border-b border-slate-200/60">
            <div className="px-6 py-6">
                <div className="flex items-start justify-between gap-6">
                    {/* Sol Taraf - Başlık ve İstatistikler */}
                    <div className="min-w-0 flex-1">
                        <div className="flex items-start gap-4">
                            <div className="flex-1">
                                {title && (
                                    <div className="flex items-center gap-3 mb-2">
                                        <h1 className="text-2xl font-bold text-slate-900 tracking-tight">
                                            {title}
                                        </h1>
                                        {totalRecords !== undefined && (
                                            <div className="flex items-center gap-2">
                                                <div className="w-6 h-6 bg-blue-100 rounded-full flex items-center justify-center">
                                                    <Database className="h-3 w-3 text-blue-600" />
                                                </div>
                                                <span className="text-sm font-semibold text-blue-600">
                                                    {totalRecords.toLocaleString()}
                                                </span>
                                            </div>
                                        )}
                                    </div>
                                )}
                                {description && (
                                    <p className="text-slate-600 text-sm leading-relaxed max-w-2xl mb-3">
                                        {description}
                                    </p>
                                )}

                            </div>
                        </div>
                    </div>

                    {/* Sağ Taraf - Eylem Butonları */}
                    <div className="flex items-center gap-3 flex-shrink-0">
                        {/* Yazdır */}
                        <Button
                            variant="outline"
                            color="neutral"
                            size="sm"
                            className="group bg-white/80 backdrop-blur-sm hover:bg-white shadow-sm"
                            onClick={onPrint}
                        >
                            <Printer className="h-4 w-4 group-hover:scale-110 transition-transform" />
                            <span className="hidden sm:inline">Yazdır</span>
                        </Button>

                        {/* Dışa Aktar */}
                        <Dropdown placement="bottom-end">
                            <Dropdown.Trigger>
                                <Button
                                    variant="filled"
                                    color="primary"
                                    size="sm"
                                    className="group shadow-lg hover:shadow-xl transition-all duration-300 bg-gradient-to-r from-blue-600 to-blue-700"
                                >
                                    <Download className="h-4 w-4 group-hover:scale-110 transition-transform" />
                                    <span className="hidden sm:inline">Dışa Aktar</span>
                                    <ChevronDown className="h-4 w-4 group-hover:rotate-180 transition-transform" />
                                </Button>
                            </Dropdown.Trigger>
                            <Dropdown.Content>
                                <div className="w-64 p-3">
                                    <div className="mb-3">
                                        <h5 className="text-sm font-semibold text-slate-900 mb-1">Dışa Aktar</h5>
                                        <p className="text-xs text-slate-500">Verilerinizi farklı formatlarda indirin</p>
                                    </div>
                                    <div className="space-y-2">
                                        {[
                                            { format: 'csv', label: 'CSV Formatı', desc: 'Tablolama uygulamaları için', color: 'green', code: 'CSV' },
                                            { format: 'xlsx', label: 'Excel Formatı', desc: 'Microsoft Excel için', color: 'blue', code: 'XLS' },
                                            { format: 'pdf', label: 'PDF Formatı', desc: 'Yazdırma ve paylaşım için', color: 'red', code: 'PDF' }
                                        ].map(({ format, label, desc, color, code }) => (
                                            <Button
                                                key={format}
                                                variant="ghost"
                                                color="neutral"
                                                className="w-full justify-start group p-3 h-auto"
                                                onClick={() => onExport(format as 'csv' | 'xlsx' | 'pdf')}
                                            >
                                                <div className={`w-10 h-10 bg-${color}-100 rounded-lg flex items-center justify-center mr-3 group-hover:bg-${color}-200 transition-colors`}>
                                                    <span className={`text-xs font-bold text-${color}-700`}>{code}</span>
                                                </div>
                                                <div className="text-left">
                                                    <div className="font-medium text-slate-900 text-sm">{label}</div>
                                                    <div className="text-xs text-slate-500">{desc}</div>
                                                </div>
                                            </Button>
                                        ))}
                                    </div>
                                </div>
                            </Dropdown.Content>
                        </Dropdown>

                        {/* Ayarlar */}
                        <Dropdown placement="bottom-end">
                            <Dropdown.Trigger>
                                <Button
                                    variant="outline"
                                    color="neutral"
                                    size="sm"
                                    isIcon
                                    className="group relative bg-white/80 backdrop-blur-sm hover:bg-white shadow-sm"
                                >
                                    <Settings className="h-4 w-4 group-hover:rotate-90 transition-transform duration-300" />
                                    <span className="sr-only">Ayarlar</span>
                                    {(visibleColumnCount < totalColumnCount || pageSize !== 10) && (
                                        <span className="absolute -top-1 -right-1 h-3 w-3 bg-gradient-to-r from-orange-400 to-red-500 rounded-full border-2 border-white animate-pulse"></span>
                                    )}
                                </Button>
                            </Dropdown.Trigger>
                            <Dropdown.Content closeOnClickInside={false}>
                                {settingsContent}
                            </Dropdown.Content>
                        </Dropdown>
                    </div>
                </div>
            </div>

        </div>
    );
}