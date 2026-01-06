// src/components/ui/Table/types.ts
import React from "react";

export type SortDir = "asc" | "desc" | null;

export type FilterValue = any;

export interface NumberRangeValue {
    min?: number;
    max?: number;
}

export interface DateRangeValue {
    from?: string;
    to?: string;
}

export type SelectValue = string | string[];

export interface FilterConfig {
    type: "text" | "numberRange" | "dateRange" | "select";
    showAsButton?: boolean;
    buttonLabel?: string;
    placeholder?: string;
    options?: { label: string; value: string; color?: string }[];
    predicate?: (cellValue: any, filterValue: FilterValue, row: any) => boolean;
    renderer?: (value: FilterValue, onChange: (value: FilterValue) => void) => React.ReactNode;
    formatValue?: (value: number) => string;
}

export interface ColumnDef<T> {
    id: string;
    header: React.ReactNode;
    accessor?: keyof T | ((row: T) => any);
    cell?: (value: any, row: T) => React.ReactNode;
    sortable?: boolean;
    sortFn?: (a: T, b: T) => number;
    filter?: FilterConfig;
    className?: string;
    width?: string;
    minWidth?: string;
    maxWidth?: string;
    sticky?: "left" | "right";
}

export interface DataTableQuery {
    page: number;
    pageSize: number;
    sortBy: string | null;
    sortDir: SortDir;
    filters: Record<string, FilterValue>;
    search?: string;
}

export interface DataTableProps<T> {
    // Required
    data: T[];
    columns: ColumnDef<T>[];

    // Optional basic
    selectableRows?: boolean;
    initialPageSize?: number;
    initialFilters?: Record<string, FilterValue>;

    // Server/Client mode
    mode?: "client" | "server";
    totalCount?: number;
    onQueryChange?: (query: DataTableQuery) => void;

    // UI
    title?: string;
    description?: string;
    toolbarRight?: React.ReactNode;
    variant?: "default" | "modern" | "minimal";
    density?: "compact" | "normal" | "comfortable";
    showFilterButtons?: boolean;
    className?: string;
    tableHeight?: string;

    // Interactions
    clickColumnId?: string;
    onCellClick?: (row: T) => void;
    onRowClick?: (row: T) => void;

    // States
    loading?: boolean;
    error?: string;
    emptyStateTitle?: string;
    emptyStateDescription?: string;

    // Customization
    searchPlaceholder?: string;
    pageSizeOptions?: number[];
    filtersDropdownContent?: React.ReactNode;
    settingsDropdownContent?: React.ReactNode;
    controlled?: boolean;
}