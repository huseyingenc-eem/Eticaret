import * as React from "react";

/** Sıralama yönü */
export type SortDir = "asc" | "desc" | null;

/** Hücre değerine erişim şekli */
export type Accessor<T> = (keyof T & string) | ((row: T) => any);

/** Filtre değer tipleri */
export type TextValue = string;
export type NumberRangeValue = { min?: number; max?: number };
export type DateRangeValue = { from?: string; to?: string };
export type SelectValue = string | string[];
export type FilterValue =
    | TextValue
    | NumberRangeValue
    | DateRangeValue
    | SelectValue
    | undefined;

/** Filtre tipi */
export type FilterKind = "text" | "numberRange" | "dateRange" | "select";

/** Filtre tanımı (UI bağımsız) */
export type FilterSpec<T = any> = {
    type: FilterKind;
    /** Custom predicate sağlarsan built-in'i bypass eder */
    predicate?: (cell: any, value: FilterValue, row: T) => boolean;
    /** Select filtre için opsiyonlar */
    options?: { label: string; value: string }[];
    /** Toolbar buton metni (opsiyonel) */
    buttonLabel?: string;
    /** Toolbar’da buton olarak görünmesi istenir mi */
    showAsButton?: boolean;
    /** numberRange için gösterim */
    formatValue?: (n: number) => string;
    /** İstersen tamamen custom bir renderer verirsin */
    renderer?: (value: FilterValue, onChange: (v: FilterValue) => void) => React.ReactNode;
};

/** Kolon tanımı (yalın ve güçlü) */
export type Column<T> = {
    id: string; // zorunlu ve stabil
    header?: React.ReactNode;
    accessor?: Accessor<T>; // yoksa id üzerinden erişir
    width?: number | string;
    minWidth?: number | string;
    className?: string;
    sticky?: "left" | "right";

    // Etkileşim
    sortable?: boolean;
    sortFn?: (a: T, b: T) => number; // custom comparator
    cell?: (value: any, row: T) => React.ReactNode;

    // Filtre
    filter?: FilterSpec<T>;
};

export type VisibleColumns = Record<string, boolean>;

/** Tablo state’i */
export type TableState<T> = {
    page: number;
    pageSize: number;
    sortBy: string | null;
    sortDir: SortDir;
    search: string;
    filters: Record<string, FilterValue>;
    visible: VisibleColumns;
    selected: Set<React.Key>;
    keyField: (keyof T & string) | undefined;
};

/** State handler'ları */
export type TableHandlers<T> = {
    setPage: (p: number) => void;
    setPageSize: (n: number) => void;
    toggleSort: (col: Column<T>) => void;
    setSearch: (q: string) => void;
    setFilter: (columnId: string, value: FilterValue) => void;
    clearAll: () => void;
    setVisible: (next: VisibleColumns) => void;
    setSelected: React.Dispatch<React.SetStateAction<Set<React.Key>>>;
};

/** Tablo props (UI'den bağımsız, orkestratör için) */
export type TableProps<T> = {
    data: T[];
    columns?: Column<T>[];
    /** id alanı yoksa otomatik index kullanır */
    keyField?: keyof T & string;

    // mode
    mode?: "client" | "server";
    totalCount?: number; // server mode için

    // durum
    loading?: boolean;
    error?: string;

    // başlangıç
    initialPageSize?: number;
    initialFilters?: Record<string, FilterValue>;
    initialVisible?: VisibleColumns;

    // görünüm (UI bileşeninde kullanılır)
    title?: string;
    description?: string;
};
