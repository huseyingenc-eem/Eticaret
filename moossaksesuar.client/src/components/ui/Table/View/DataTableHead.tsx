// ===============================
// File: DataTableHead.tsx (optimized)
// ===============================
import React, { memo } from "react";
import { type ColumnDef, type SortDir } from "./types";

type HeadProps<T> = {
    columns: ColumnDef<T>[];
    selectableRows?: boolean;
    sortBy: string | null;
    sortDir: SortDir;
    onToggleSort: (col: ColumnDef<T>) => void;
    variant?: "default" | "minimal" | "elevated";
    density?: "compact" | "normal" | "comfortable";
    onSelectAll?: (checked: boolean) => void;
    allSelected?: boolean;
    someSelected?: boolean;
};

const SortIcon = ({ direction, isActive }: { direction?: SortDir; isActive: boolean }) => {
    if (!isActive) {
        return (
            <svg className="w-3 h-3 text-slate-400 opacity-0 group-hover:opacity-100 transition-opacity" fill="currentColor" viewBox="0 0 20 20">
                <path d="M5 12l5-5 5 5H5z" />
            </svg>
        );
    }

    if (direction === "asc") {
        return (
            <svg className="w-3 h-3 text-slate-600" fill="currentColor" viewBox="0 0 20 20">
                <path d="M5 12l5-5 5 5H5z" />
            </svg>
        );
    }

    if (direction === "desc") {
        return (
            <svg className="w-3 h-3 text-slate-600" fill="currentColor" viewBox="0 0 20 20">
                <path d="M15 8l-5 5-5-5h10z" />
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
                                       allSelected = false,
                                       someSelected = false,
                                   }: HeadProps<T>) {
    const variantStyles = {
        default: {
            header: `bg-gradient-to-r from-slate-50 via-gray-50/90 to-slate-50 border-b border-slate-200/60 backdrop-blur-sm`,
            sticky: "bg-gradient-to-r from-slate-50 to-gray-50/90 backdrop-blur-sm",
            text: "text-slate-700",
            sortButton: "hover:bg-slate-100/60 active:bg-slate-200/40",
        },
        minimal: {
            header: "bg-white border-b border-slate-200",
            sticky: "bg-white",
            text: "text-slate-600",
            sortButton: "hover:bg-slate-50 active:bg-slate-100",
        },
        elevated: {
            header: `bg-gradient-to-r from-white via-slate-50/50 to-white border-b border-slate-200/80 shadow-sm backdrop-blur-sm`,
            sticky: "bg-gradient-to-r from-white to-slate-50/50 backdrop-blur-sm shadow-sm",
            text: "text-slate-700",
            sortButton: "hover:bg-slate-100/50 hover:shadow-sm active:bg-slate-200/30",
        },
    } as const;

    const densityStyles = { compact: "px-3 py-2", normal: "px-4 py-3", comfortable: "px-5 py-4" } as const;

    const currentVariant = variantStyles[variant];
    const cellPadding = densityStyles[density];

    return (
        <thead className={`relative ${currentVariant.header}`}>
        <tr className="text-left">
            {selectableRows && (
                <th className={`sticky left-0 z-20 ${currentVariant.sticky} border-r border-slate-200/50 ${cellPadding}`}>
                    <div className="flex items-center justify-center">
                        <input
                            type="checkbox"
                            checked={allSelected}
                            ref={(el) => {
                                if (el) el.indeterminate = someSelected && !allSelected;
                            }}
                            onChange={(e) => onSelectAll?.(e.target.checked)}
                            className={`h-4 w-4 rounded border-2 border-slate-300 text-blue-600 focus:ring-2 focus:ring-blue-500/20 transition-colors duration-150 ${
                                allSelected || someSelected ? "bg-blue-600 border-blue-600" : "hover:border-blue-400"
                            }`}
                            aria-label="Tüm satırları seç"
                        />
                    </div>
                </th>
            )}

            {columns.map((col) => {
                const id = (col.id || "").toLowerCase();
                const isId = id === "id";
                const isActions = id === "actions";
                const isSorted = sortBy === col.id;

                const ariaSort: React.ThHTMLAttributes<HTMLTableHeaderCellElement>["aria-sort"] =
                    isSorted ? (sortDir === "asc" ? "ascending" : sortDir === "desc" ? "descending" : "none") : "none";

                const suggestedWidth = col.width
                    ? col.width
                    : isId
                        ? "w-[80px] max-w-[80px]"
                        : isActions
                            ? "w-24 md:w-28"
                            : "";

                const stickyCls = col.sticky
                    ? `sticky ${col.sticky === "left" ? "left-0" : "right-0"} z-10 ${currentVariant.sticky} border-r border-slate-200/50`
                    : "";

                const cellClasses = [
                    "relative whitespace-nowrap select-none",
                    cellPadding,
                    currentVariant.text,
                    col.className ?? "",
                    suggestedWidth,
                    stickyCls,
                    isId && "text-center",
                    isActions && "text-right",
                ]
                    .filter(Boolean)
                    .join(" ");

                const headerTextClasses = [
                    "font-semibold text-xs uppercase tracking-wider",
                    isId && "font-mono",
                    isActions && "text-right",
                ]
                    .filter(Boolean)
                    .join(" ");

                return (
                    <th key={col.id} className={cellClasses} aria-sort={ariaSort}>
                        {col.sortable ? (
                            <button
                                type="button"
                                className={`group flex items-center gap-2 w-full rounded-md px-2 py-1 -mx-2 -my-1 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500/20 transition-all duration-150 ease-in-out ${
                                    currentVariant.sortButton
                                } ${isActions ? "justify-end text-right" : "justify-start text-left"} ${
                                    isSorted ? "text-slate-900" : "text-slate-600 hover:text-slate-800"
                                }`}
                                onClick={() => onToggleSort(col)}
                                aria-label={`${col.header} sütununa göre sırala`}
                            >
                                <span className={`truncate min-w-0 ${headerTextClasses}`}>{col.header}</span>
                                <div className="flex-shrink-0 flex items-center">
                                    <SortIcon direction={sortDir} isActive={isSorted} />
                                </div>
                            </button>
                        ) : (
                            <div className={`flex items-center gap-2 min-w-0 ${
                                isActions ? "justify-end text-right" : "justify-start text-left"
                            }`}>
                                <span className={`truncate ${headerTextClasses}`}>{col.header}</span>
                            </div>
                        )}
                    </th>
                );
            })}
        </tr>

        {variant === "elevated" && (
            <tr className="absolute inset-x-0 bottom-0 h-px">
                <td colSpan={columns.length + (selectableRows ? 1 : 0)} className="h-px">
                    <div className="h-full bg-gradient-to-r from-transparent via-slate-300/40 to-transparent" />
                </td>
            </tr>
        )}
        </thead>
    );
}

export default memo(DataTableHeadComponent) as typeof DataTableHeadComponent;
