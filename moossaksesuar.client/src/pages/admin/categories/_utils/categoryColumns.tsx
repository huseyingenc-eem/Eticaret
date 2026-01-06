// src/pages/categories/_utils/categoryColumns.tsx
import { Button } from "@/components/ui";
import { Pencil, Trash2 } from "lucide-react";

import type { ColumnDef } from "@/components/ui/Table/types";
import type { CategoryListItem } from "@/services/api/endpoints/category/category.types";

type BuildOpts = {
    onEdit: (id: number) => void;
    onDelete: (id: number) => void;
    onToggleActive: (id: number, next: boolean) => void;
    disableStatusFilter?: boolean;
};

/**
 * - ID / Ad / Durum / Actions → min genişlik verildi (sabit dar alanı garanti eder).
 * - Ad → tek satır + ellipsis; dar tutuldu.
 * - Açıklama → mobilde sütun tamamen gizlenir, masaüstünde kalan alanı doldurur (1 satır clamp).
 * - Hiçbir kolona 'w-*' ile sabit genişlik verilmedi; sadece 'min-w-*' kullanıldı.
 * - Filtreler:
 *    • ID → numberRange (buton)
 *    • Durum → select (buton)
 */
export function makeCategoryColumns(opts: BuildOpts): ColumnDef<CategoryListItem>[] {
    const cols: ColumnDef<CategoryListItem>[] = [
        {
            id: "id",
            header: "ID",
            accessor: (r) => r.id,
            sortable: true,
            className: "text-left min-w-20 md:min-w-30",
            // 🔎 ID aralığı filtresi (buton)
            filter: {
                type: "numberRange",
                showAsButton: true,
                buttonLabel: "ID",
                // opsiyonel: görünümü güzelleştirmek istersen
                formatValue: (n: number) => (typeof n === "number" ? n.toLocaleString("tr-TR") : ""),
            },
        },

        {
            id: "name",
            header: "Ad",
            accessor: (r) => r.name,
            sortable: true,
            className:
                "text-left min-w-[12ch] md:min-w-[16ch] max-w-[18ch] md:max-w-[24ch] whitespace-nowrap overflow-hidden text-ellipsis cursor-pointer underline-offset-2 hover:underline",
            // Not: text filtre tipiniz butonla render edilmiyor; genel arama input’u zaten "Ad" üzerinde çalışır.
            // İstersen özel predicate ile butonlu text filter eklenebilir.
        },

        {
            id: "description",
            header: <span className="hidden md:inline">Açıklama</span>,
            accessor: (r) => r.description ?? "",
            sortable: false,
            className: "hidden md:table-cell md:min-w-0 md:align-top",
            cell: (val: string) => {
                const text = String(val ?? "");
                return (
                    <div
                        className="min-w-0 overflow-hidden whitespace-normal break-words leading-snug text-slate-700"
                        title={text}
                        style={{
                            display: "-webkit-box",
                            WebkitLineClamp: 1,
                            WebkitBoxOrient: "vertical" as any,
                            overflow: "hidden",
                        }}
                    >
                        {text}
                    </div>
                );
            },
        },

        {
            id: "isActive",
            header: "Durum",
            accessor: (r) => (r.isActive ? "Aktif" : "Pasif"),
            sortable: true,
            className: "text-left min-w-[14ch] md:min-w-[16ch]",
            // 🔎 Durum filtresi (buton)
            filter: !opts.disableStatusFilter
                ? {
                    type: "select",
                    showAsButton: true,
                    buttonLabel: "Durum",
                    options: [
                        { label: "Aktif", value: "Aktif" },
                        { label: "Pasif", value: "Pasif" },
                    ],
                }
                : undefined,
            cell: (_: string, row: CategoryListItem) => {
                const active = !!row.isActive;
                return (
                    <Button
                        size="sm"
                        variant="ghost"
                        className={[
                            "px-2 py-0.5 h-6 rounded-full text-xs",
                            active
                                ? "text-green-700 hover:text-green-800 hover:bg-green-50"
                                : "text-amber-700 hover:text-amber-800 hover:bg-amber-50",
                        ].join(" ")}
                        onClick={(e) => {
                            e.stopPropagation();
                            // if (!confirm(`${row.name ?? "Kategori"} ${active ? "pasif" : "aktif"} yapılsın mı?`)) return;
                            opts.onToggleActive(row.id, !active);
                        }}
                        title={`${row.name ?? "Kategori"} ${active ? "pasif" : "aktif"} yapılsın`}
                    >
                        {active ? "Aktif" : "Pasif"}
                    </Button>
                );
            },
        },

        {
            id: "actions",
            header: "ACTIONS",
            sortable: false,
            sticky: "right",
            className: "text-right min-w-[64px] md:min-w-[88px]",
            cell: (_: any, row: CategoryListItem) => (
                <div className="flex items-center justify-end gap-1">
                    <Button
                        size="sm"
                        variant="ghost"
                        title="Düzenle"
                        onClick={(e) => {
                            e.stopPropagation();
                            opts.onEdit(row.id);
                        }}
                    >
                        <Pencil className="h-4 w-4" />
                        <span className="sr-only">Düzenle</span>
                    </Button>

                    <Button
                        size="sm"
                        variant="ghost"
                        title="Sil"
                        onClick={(e) => {
                            e.stopPropagation();
                            opts.onDelete(row.id);
                        }}
                    >
                        <Trash2 className="h-4 w-4" />
                        <span className="sr-only">Sil</span>
                    </Button>
                </div>
            ),
        },
    ];

    return cols;
}
