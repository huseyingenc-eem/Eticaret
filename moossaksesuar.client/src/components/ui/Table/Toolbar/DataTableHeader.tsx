import React from "react";
import { Settings, Download, Printer, ChevronDown } from "lucide-react";
import type { ColumnDef } from "./types";
import {Dropdown, Button} from "@/components/ui";

interface Props {
    title?: string;
    description?: string;
    onExport: (format: 'csv' | 'xlsx' | 'pdf') => void;
    onPrint: () => void;
    columns: ColumnDef<any>[];
    visibleColumns: Record<string, boolean>;
    onVisibleColumnsChange: (columns: Record<string, boolean>) => void;
    pageSize: number;
    onPageSizeChange: (size: number) => void;
    density: "compact" | "normal" | "comfortable"; // Bu prop şu an kullanılmıyor ama gelecekte kullanılabilir.
    customSettingsContent?: React.ReactNode;
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
                                            onPageSizeChange,
                                            customSettingsContent,
                                        }: Props) {
    // Dropdown'lar için olan useState'lere artık gerek yok.

    const settingsContent = customSettingsContent || (
        <div className="space-y-4 min-w-[250px]">
            <div>
                <h4 className="text-sm font-medium text-slate-700 mb-3">Kolon Görünürlüğü</h4>
                <div className="space-y-2 max-h-60 overflow-y-auto">
                    {columns.map((col) => (
                        <label
                            key={col.id}
                            className="flex items-center justify-between gap-3 text-sm py-2 px-2 rounded hover:bg-slate-50 transition-colors cursor-pointer"
                        >
                            <span className="text-slate-700 truncate flex-1">{col.header}</span>
                            <input
                                type="checkbox"
                                checked={visibleColumns[col.id] !== false}
                                onChange={(e) => onVisibleColumnsChange({
                                    ...visibleColumns,
                                    [col.id]: e.target.checked
                                })}
                                className="h-4 w-4 rounded border-slate-300 text-blue-600 focus:ring-2 focus:ring-blue-500/20"
                            />
                        </label>
                    ))}
                </div>
            </div>

            <div className="pt-3 border-t border-slate-200">
                <h4 className="text-sm font-medium text-slate-700 mb-2">Sayfa Boyutu</h4>
                <select
                    className="w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
                    value={pageSize}
                    onChange={(e) => onPageSizeChange(Number(e.target.value))}
                >
                    {[5, 10, 20, 50, 100].map((size) => (
                        <option key={size} value={size}>{size} kayıt</option>
                    ))}
                </select>
            </div>

            <div className="pt-3 border-t border-slate-200">
                <Button
                    variant="ghost"
                    color="neutral"
                    size="sm"
                    className="w-full"
                    onClick={() => {
                        const defaultColumns = columns.reduce((acc, col) => ({
                            ...acc,
                            [col.id]: true
                        }), {});
                        onVisibleColumnsChange(defaultColumns);
                        onPageSizeChange(10);
                        // Ayarlar sıfırlandığında menü açık kalabilir, kullanıcı dışarı tıklar.
                        // İstenirse ref ile programatik olarak kapatılabilir.
                    }}
                >
                    Varsayılanlara Dön
                </Button>
            </div>
        </div>
    );

    return (
        <div className="bg-white border-b border-slate-200 px-6 py-4">
            <div className="flex items-center justify-between">
                <div className="min-w-0 flex-1">
                    {title && (
                        <h2 className="text-xl font-semibold text-slate-800 mb-1">
                            {title}
                        </h2>
                    )}
                    {description && (
                        <p className="text-sm text-slate-500">
                            {description}
                        </p>
                    )}
                </div>

                <div className="flex items-center gap-2 ml-4">
                    <Button variant="outline" color="neutral" size="sm" onClick={onPrint}>
                        <Printer className="h-4 w-4" />
                        <span>Yazdır</span>
                    </Button>

                    <Dropdown placement="bottom-end">
                        <Dropdown.Trigger>
                            <Button variant="outline" color="neutral" size="sm">
                                <Download className="h-4 w-4" />
                                <span>Dışa Aktar</span>
                                <ChevronDown className="h-4 w-4" />
                            </Button>
                        </Dropdown.Trigger>
                        <Dropdown.Content>
                            <div className="space-y-1 min-w-[180px]">
                                <Button variant="ghost" color="neutral" className="w-full !justify-start" onClick={() => onExport('csv')}>
                                    CSV olarak dışa aktar
                                </Button>
                                <Button variant="ghost" color="neutral" className="w-full !justify-start" onClick={() => onExport('xlsx')}>
                                    Excel olarak dışa aktar
                                </Button>
                                <Button variant="ghost" color="neutral" className="w-full !justify-start" onClick={() => onExport('pdf')}>
                                    PDF olarak dışa aktar
                                </Button>
                            </div>
                        </Dropdown.Content>
                    </Dropdown>

                    <Dropdown placement="bottom-end">
                        <Dropdown.Trigger>
                            <Button variant="outline" color="neutral" size="sm" isIcon>
                                <Settings className="h-4 w-4" />
                                <span className="sr-only">Ayarlar</span>
                            </Button>
                        </Dropdown.Trigger>
                        <Dropdown.Content closeOnClickInside={false}>
                            {settingsContent}
                        </Dropdown.Content>
                    </Dropdown>
                </div>
            </div>
        </div>
    );
}