// src/pages/suppliers/_utils/supplierColumns.tsx
import { Button } from "@/components/ui";
import { Pencil, Trash2 } from "lucide-react";

import type { ColumnDef } from "@/components/ui/Table/types";
import type { SupplierListItem } from "@/services/api/endpoints/supplier/supplier.types";

type BuildOpts = {
    onEdit: (id: string) => void;
    onDelete: (id: string) => void;
    onToggleActive: (id: string, next: boolean) => void;
    disableStatusFilter?: boolean;
};

export function makeSupplierColumns(
    opts: BuildOpts
): ColumnDef<SupplierListItem>[] {
    const shortId = (id: string) => (id?.length > 8 ? id.slice(0, 8) : id ?? "");
    const safe = (v?: string | null, fallback = "—") =>
        (v ?? "").trim() || fallback;

    const cols: ColumnDef<SupplierListItem>[] = [
        {
            id: "id",
            header: "ID",
            accessor: (r) => r.id,
            sortable: true,
            className: "text-left min-w-24 md:min-w-32 font-mono",
            cell: (val: string) => (
                <span title={val} className="tabular-nums">
          {shortId(val)}
                    {val?.length > 8 ? "…" : ""}
        </span>
            ),
        },

        {
            id: "companyName",
            header: "Firma",
            accessor: (r) => r.companyName,
            sortable: true,
            className:
                "text-left min-w-[16ch] md:min-w-[20ch] max-w-[22ch] md:max-w-[28ch] whitespace-nowrap overflow-hidden text-ellipsis cursor-pointer underline-offset-2 hover:underline",
        },

        {
            id: "contactPerson",
            header: "İlgili",
            accessor: (r) => r.contactPerson ?? "",
            sortable: true,
            className:
                "text-left min-w-[10ch] md:min-w-[14ch] max-w-[20ch] whitespace-nowrap overflow-hidden text-ellipsis",
            cell: (val?: string | null) => safe(val),
        },

        {
            id: "phoneNumber",
            header: "Telefon",
            accessor: (r) => r.phoneNumber ?? "",
            sortable: true,
            className: "text-left min-w-[12ch] md:min-w-[14ch] whitespace-nowrap",
            cell: (val?: string | null) => {
                const txt = safe(val);
                return (
                    <a
                        href={val ? `tel:${val}` : undefined}
                        onClick={(e) => !val && e.preventDefault()}
                        className={val ? "underline underline-offset-2" : "pointer-events-none"}
                        title={val ?? ""}
                    >
                        {txt}
                    </a>
                );
            },
        },

        {
            id: "isActive",
            header: "Durum",
            accessor: (r) => (r.isActive ? "Aktif" : "Pasif"),
            sortable: true,
            className: "text-left min-w-[14ch] md:min-w-[16ch]",
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
            cell: (_: string, row: SupplierListItem) => {
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
                            opts.onToggleActive(row.id, !active);
                        }}
                        title={`${row.companyName ?? "Tedarikçi"} ${
                            active ? "pasif" : "aktif"
                        } yapılsın`}
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
            cell: (_: any, row: SupplierListItem) => (
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

    // Emniyet: yanlışlıkla undefined/bozuk eklenmişse ele
    return cols.filter(
        (c): c is ColumnDef<SupplierListItem> => !!c && !!c.id && typeof (c as any).header !== "undefined"
    );
}
