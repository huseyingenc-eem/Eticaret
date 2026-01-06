// src/components/ui/Table/View/DataTableHead.tsx
import React, { memo } from "react";
import { type ColumnDef, type SortDir } from "../types";
import { Button } from "@/components/ui";

type DataTableHeadProps<T> = {
    columns?: ColumnDef<T>[];
    selectableRows?: boolean;
    sortBy: string | null;
    sortDir: SortDir;
    onToggleSort: (col: ColumnDef<T>) => void;
    variant?: "default" | "modern" | "minimal";
    density?: "compact" | "normal" | "comfortable";
    onSelectAll?: (checked: boolean) => void;
    allSelected?: boolean;
    someSelected?: boolean;
};

const SortIcon = ({ direction, isActive }: { direction?: SortDir; isActive: boolean }) => {
    const baseClass = "w-3.5 h-3.5 transition-all duration-200";
    const colorClass = isActive ? "text-blue-600" : "text-slate-400 group-hover:text-slate-600";

    if (!isActive) {
        return (
            <svg className={`${baseClass} ${colorClass} opacity-0 group-hover:opacity-100`} fill="none" strokeWidth="2" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" d="M8 9l4-4 4 4M8 15l4 4 4-4" />
            </svg>
        );
    }

    if (direction === "asc") {
        return (
            <svg className={`${baseClass} ${colorClass}`} fill="none" strokeWidth="2" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" d="M5 15l7-7 7 7" />
            </svg>
        );
    }

    if (direction === "desc") {
        return (
            <svg className={`${baseClass} ${colorClass}`} fill="none" strokeWidth="2" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
            </svg>
        );
    }

    return null;
};

function DataTableHeadComponent<T>({
                                       columns,
                                       selectableRows,
                                       sortBy,
                                       sortDir,
                                       onToggleSort,
                                       variant = "default",
                                       density = "normal",
                                       onSelectAll,
                                       allSelected,
                                       someSelected
                                   }: DataTableHeadProps<T>) {
    // Columns kontrolü
    if (!columns || !Array.isArray(columns)) {
        return null;
    }

    // Density'ye göre padding ayarları
    const paddingClasses = {
        compact: "px-3 py-1.5",
        normal: "px-4 py-2.5",
        comfortable: "px-6 py-3.5"
    };

    // Variant'a göre stil ayarları
    const variantClasses = {
        default: "bg-slate-50/50 border-b border-slate-200",
        modern: "bg-gradient-to-r from-slate-50 to-white border-b border-slate-200",
        minimal: "border-b border-slate-100"
    };

    const padding = paddingClasses[density];
    const headerClass = variantClasses[variant];

    return (
        <thead className={headerClass}>
        <tr>
            {selectableRows && (
                <th className={`w-12 text-center ${padding}`}>
                    <div className="flex items-center justify-center">
                        <input
                            type="checkbox"
                            checked={allSelected}
                            onChange={(e) => onSelectAll?.(e.target.checked)}
                            className={`
                                    h-4 w-4 rounded border-slate-300
                                    text-blue-600 focus:ring-2 focus:ring-offset-0 focus:ring-blue-500/50
                                    transition-colors cursor-pointer
                                    ${someSelected && !allSelected ? "indeterminate:bg-blue-600" : ""}
                                `}
                            ref={(el) => {
                                if (el) el.indeterminate = someSelected && !allSelected;
                            }}
                        />
                    </div>
                </th>
            )}

            {columns.map((col) => {
                if (!col || !col.id) {
                    return null;
                }

                const isSorted = sortBy === col.id;

                return (
                    <th
                        key={col.id}
                        className={`
                                text-left text-xs font-medium uppercase tracking-wider
                                text-slate-600 ${padding} ${col.className || ""}
                            `}
                        style={{
                            minWidth: col.minWidth,
                            width: col.width
                        }}
                    >
                        {col.sortable ? (
                            <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => onToggleSort(col)}
                                className={`
                                        group -ml-2 h-auto py-0 px-2
                                        flex items-center gap-1.5
                                        hover:bg-transparent hover:text-blue-600
                                        transition-colors duration-200
                                        ${isSorted ? "text-blue-600 font-semibold" : ""}
                                    `}
                            >
                                    <span className="select-none">
                                        {col.header || col.id}
                                    </span>
                                <SortIcon
                                    direction={isSorted ? sortDir : undefined}
                                    isActive={isSorted}
                                />
                            </Button>
                        ) : (
                            <span className="inline-flex items-center select-none">
                                    {col.header || col.id}
                                </span>
                        )}
                    </th>
                );
            })}
        </tr>
        </thead>
    );
}

export default memo(DataTableHeadComponent) as typeof DataTableHeadComponent;