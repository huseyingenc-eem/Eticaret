// src/components/ui/Table/DataTableFilters.tsx
import React from "react";
import { Search, RefreshCw, X } from "lucide-react";
import type { ColumnDef, FilterValue } from "./types";
import { StatusFilter, DateRangeFilter, NumberRangeFilter } from "./FilterComponents";

interface Props {
    searchValue: string;
    onSearchChange: (value: string) => void;
    searchPlaceholder: string;
    columns: ColumnDef<any>[];
    filters: Record<string, FilterValue>;
    onFilterChange: (columnId: string, value: FilterValue) => void;
    onClearAll: () => void;
    showFilterButtons: boolean;
    onRefresh: () => void;
    loading: boolean;
    customFiltersContent?: React.ReactNode;
    /* Aşağıdakiler DataTableView tarafından gönderiliyordu; opsiyonel ekledik */
    onExport?: (format: 'csv' | 'xlsx' | 'pdf') => void;
    onPrint?: () => void;
    allColumns?: ColumnDef<any>[];
    visibleColumns?: Record<string, boolean>;
    onVisibleColumnsChange?: (columns: Record<string, boolean>) => void;
    pageSize?: number;
    onPageSizeChange?: (size: number) => void;
    density?: "compact" | "normal" | "comfortable";
    customSettingsContent?: React.ReactNode;
}

export default function DataTableFilters({
                                             searchValue,
                                             onSearchChange,
                                             searchPlaceholder,
                                             columns,
                                             filters,
                                             onFilterChange,
                                             onClearAll,
                                             showFilterButtons,
                                             onRefresh,
                                             loading,
                                             customFiltersContent
                                         }: Props) {
    const filterColumns = columns.filter(col => col.filter?.showAsButton);
    const hasActiveFilters =
        Object.values(filters).some(value =>
            value != null &&
            value !== "" &&
            (!Array.isArray(value) || value.length > 0)
        ) || searchValue.trim() !== "";

    const renderFilterButton = (column: ColumnDef<any>) => {
        const filterConfig = column.filter!;
        const currentValue = filters[column.id];

        switch (filterConfig.type) {
            case "select":
                return (
                    <StatusFilter
                        key={column.id}
                        value={Array.isArray(currentValue) ? currentValue : []}
                        onChange={(value) => onFilterChange(column.id, value)}
                        options={filterConfig.options || []}
                        label={filterConfig.buttonLabel || (column.header as string)}
                    />
                );
            case "dateRange":
                return (
                    <DateRangeFilter
                        key={column.id}
                        value={currentValue || {}}
                        onChange={(value) => onFilterChange(column.id, value)}
                        label={filterConfig.buttonLabel || (column.header as string)}
                    />
                );
            case "numberRange":
                return (
                    <NumberRangeFilter
                        key={column.id}
                        value={currentValue || {}}
                        onChange={(value) => onFilterChange(column.id, value)}
                        label={filterConfig.buttonLabel || (column.header as string)}
                        formatValue={filterConfig.formatValue}
                    />
                );
            default:
                return null;
        }
    };

    return (
        <div className="bg-slate-50 border-b border-slate-200 px-6 py-4">
            <div className="flex items-center justify-between gap-4">
                <div className="flex items-center gap-4 flex-1">
                    <div className="relative">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" />
                        <input
                            type="text"
                            value={searchValue}
                            onChange={(e) => onSearchChange(e.target.value)}
                            placeholder={searchPlaceholder}
                            className="pl-10 pr-4 py-2 w-64 border border-slate-300 rounded-lg bg-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 transition-all duration-200 outline-none text-sm"
                        />
                        {searchValue && (
                            <button
                                onClick={() => onSearchChange("")}
                                className="absolute right-2 top-1/2 -translate-y-1/2 p-1 hover:bg-slate-100 rounded transition-colors"
                            >
                                <X className="h-3 w-3 text-slate-400" />
                            </button>
                        )}
                    </div>

                    {showFilterButtons && filterColumns.length > 0 && (
                        <div className="flex items-center gap-2 flex-wrap">
                            {filterColumns.map(renderFilterButton)}
                        </div>
                    )}

                    {customFiltersContent}

                    {hasActiveFilters && (
                        <button
                            onClick={onClearAll}
                            className="flex items-center gap-2 px-3 py-2 text-sm font-medium text-slate-600 bg-white border border-slate-300 rounded-lg hover:bg-slate-50 transition-colors"
                        >
                            <X className="h-4 w-4" />
                            Temizle
                        </button>
                    )}
                </div>

                <div className="flex items-center gap-2">
                    <button
                        onClick={onRefresh}
                        disabled={loading}
                        className="flex items-center gap-2 px-3 py-2 text-sm font-medium text-slate-600 bg-white border border-slate-300 rounded-lg hover:bg-slate-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        <RefreshCw className={`h-4 w-4 ${loading ? "animate-spin" : ""}`} />
                        Yenile
                    </button>
                </div>
            </div>
        </div>
    );
}