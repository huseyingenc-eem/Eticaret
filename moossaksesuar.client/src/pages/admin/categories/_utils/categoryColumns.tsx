// src/pages/categories/_utils/categoryColumns.tsx
import React from "react";
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
 * Kolonlar TSX olarak yazıldı:
 * - description: 3 satır çok satırlı ellipsis + genişlik kontrolü
 * - isActive: metin görünümlü buton; tıklanınca aktif/pasif toggle (confirm satırı yorumda)
 * - actions: sadece Düzenle + Sil
 */
export function makeCategoryColumns(opts: BuildOpts): ColumnDef<CategoryListItem>[] {
    const cols: ColumnDef<CategoryListItem>[] = [
        { id: "id", header: "ID", accessor: (r) => r.id, sortable: true, width: "w-24" },

        {
            id: "name",
            header: "Ad",
            accessor: (r) => r.name,
            sortable: true,
            className: "text-left cursor-pointer underline-offset-2 hover:underline",
        },

        {
            id: "description",
            header: "Açıklama",
            accessor: (r) => r.description ?? "",
            sortable: false,
            className: "min-w-0 w-[38ch] max-w-[42ch]",
            width: "w-[38ch] max-w-[42ch]",
            cell: (val: string) => {
                const text = String(val ?? "");
                return (
                    <div
                        className="min-w-0 max-w-[42ch] overflow-hidden whitespace-normal break-words leading-snug text-slate-700"
                title={text}
                style={{
                    display: "-webkit-box",
                        WebkitLineClamp: 3,
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
            // Dışarıdaki StatusFilter'ı kullanıyorsun; burada filter vermiyoruz.
            className: "text-left",
            cell: (_: string, row: CategoryListItem) => {
                const active = !!row.isActive;
                const label = active ? "Aktif" : "Pasif";
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
                    // İLKTE SORMAK İSTERSEN AÇ:
                    // if (!confirm(`${row.name ?? "Kategori"} ${active ? "pasif" : "aktif"} yapılsın mı?`)) return;
                    opts.onToggleActive(row.id, !active);
                }}
                title={`${row.name ?? "Kategori"} ${active ? "pasif" : "aktif"} yapılsın`}
            >
                {label}
                </Button>
            );
            },
        },

        {
            id: "actions",
            header: "ACTIONS",
            sortable: false,
            sticky: "right",
            width: "w-[96px] md:w-[112px]",
            className: "text-right shrink-0",
            // SADECE Düzenle + Sil
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
