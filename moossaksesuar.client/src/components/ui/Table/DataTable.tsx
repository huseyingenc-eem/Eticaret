// src/components/Table/index.tsx
import React, { useState, useMemo, useCallback, useDeferredValue } from 'react';
import type { ColumnDef, DataTableProps, FilterValue, SortDir } from './types';
import { toComparable, inDateRange, inNumberRange } from './utils';
import DataTableView from './DataTableView';

// --- YARDIMCI FONKSİYONLAR ---
function getCellValue<T>(row: T, col: ColumnDef<T>): any {
    if (typeof col.accessor === 'function') return col.accessor(row);
    if (typeof col.accessor === 'string') return (row as any)[col.accessor];
    return (row as any)[col.id];
}

function defaultPredicate(kind: string, cellValue: any, filterValue: FilterValue): boolean {
    switch (kind) {
        case "text":
            if (!filterValue) return true;
            return String(cellValue ?? "").toLowerCase().includes(String(filterValue).toLowerCase());
        case "numberRange":
            return inNumberRange(cellValue, filterValue);
        case "dateRange":
            return inDateRange(cellValue, filterValue);
        case "select":
            if (!filterValue || (Array.isArray(filterValue) && filterValue.length === 0)) return true;
            return Array.isArray(filterValue)
                ? filterValue.includes(String(cellValue))
                : String(cellValue) === String(filterValue);
        default:
            return true;
    }
}

// === ANA DATATABLE BİLEŞENİ ===
export default function DataTable<T>({
                                         data,
                                         columns,
                                         initialPageSize = 10,
                                         initialFilters = {},
                                         mode,
                                         totalCount,
                                         ...props
                                     }: DataTableProps<T>) {

    const isServer = mode === 'server';

    // === STATE MANAGEMENT ===
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(initialPageSize);
    const [sortBy, setSortBy] = useState<string | null>(null);
    const [sortDir, setSortDir] = useState<SortDir>(null);
    const [filters, setFilters] = useState<Record<string, FilterValue | undefined>>(initialFilters);
    const [selected, setSelected] = useState<Set<React.Key>>(new Set());
    const [visibleColumns, setVisibleColumns] = useState<Record<string, boolean>>(
        columns.reduce((acc, col) => ({ ...acc, [col.id]: true }), {})
    );
    const [searchInput, setSearchInput] = useState("");
    const deferredSearch = useDeferredValue(searchInput);

    // === HANDLERS ===
    const onToggleSort = useCallback((col: ColumnDef<T>) => {
        if (!col.sortable) return;
        if (sortBy !== col.id) {
            setSortBy(col.id); setSortDir("asc");
        } else {
            const next = sortDir === "asc" ? "desc" : sortDir === "desc" ? null : "asc";
            setSortDir(next);
            if (next === null) setSortBy(null);
        }
        setPage(1);
    }, [sortBy, sortDir]);

    const handleFilterChange = useCallback((columnId: string, value: FilterValue) => {
        setFilters((prev) => ({ ...prev, [columnId]: value }));
        setPage(1);
    }, []);

    const clearAllFilters = useCallback(() => {
        setFilters({}); setSearchInput(""); setPage(1);
    }, []);

    const handleExport = useCallback((format: 'csv' | 'xlsx' | 'pdf') => { console.log(`Exporting as ${format}`); }, []);
    const handlePrint = useCallback(() => { window.print(); }, []);
    const handleRefresh = useCallback(() => {}, []);

    // === DATA PROCESSING (CLIENT-SIDE) ===
    const finalColumns = useMemo(() => columns.filter((col) => visibleColumns[col.id] !== false), [columns, visibleColumns]);

    const processed = useMemo(() => {
        if (isServer) {
            return { rows: data, total: totalCount ?? data.length };
        }

        let rows = [...data];

        // =======================================
        // === EKSİK OLAN MANTIK BURAYA EKLENDİ ===
        // =======================================

        // 1. Adım: Sütun Filtreleri
        if (finalColumns.some((c) => c.filter)) {
            rows = rows.filter((row) =>
                finalColumns.every((col) => {
                    const conf = col.filter;
                    if (!conf) return true;
                    const fv = filters[col.id];
                    if (fv == null || fv === "" || (Array.isArray(fv) && fv.length === 0)) return true;
                    const cell = getCellValue(row, col);
                    if (conf.predicate) return conf.predicate(cell, fv, row);
                    return defaultPredicate(conf.type, cell, fv);
                })
            );
        }

        // 2. Adım: Genel Arama (Filtrelenmiş veriler üzerinde)
        const needle = deferredSearch.trim().toLowerCase();
        if (needle) {
            rows = rows.filter((row) =>
                finalColumns.some((col) => {
                    const cell = getCellValue(row, col);
                    return String(cell ?? "").toLowerCase().includes(needle);
                })
            );
        }

        // 3. Adım: Sıralama (Arama sonuçları üzerinde)
        if (sortBy && sortDir) {
            const col = finalColumns.find((c) => c.id === sortBy);
            if (col) {
                const dir = sortDir === "asc" ? 1 : -1;
                const cmp = col.sortFn
                    ? (a: T, b: T) => dir * col.sortFn!(a, b)
                    : (a: T, b: T) => {
                        const av = toComparable(getCellValue(a, col));
                        const bv = toComparable(getCellValue(b, col));
                        return dir * (av < bv ? -1 : av > bv ? 1 : 0);
                    };
                rows.sort(cmp);
            }
        }

        // Bu satır artık filtrelenmiş/sıralanmış verinin toplamını alır
        const total = rows.length;

        // 4. Adım: Sayfalama (Sonuçların gösterilecek kısmını alır)
        const start = (page - 1) * pageSize;
        const end = start + pageSize;
        const pagedRows = rows.slice(start, end);

        return { rows: pagedRows, total };
    }, [isServer, data, totalCount, finalColumns, filters, sortBy, sortDir, page, pageSize, deferredSearch]);

    // === SEÇİM MANTIĞI ===
    const allSelected = useMemo(() => processed.rows.length > 0 && processed.rows.every((row, index) => selected.has(row.id ?? index)), [processed.rows, selected]);
    const someSelected = useMemo(() => !allSelected && selected.size > 0, [selected, allSelected]);
    const handleSelectAll = useCallback((checked: boolean) => {
        if (checked) {
            const allRowKeys = new Set(processed.rows.map((row, index) => row.id ?? index));
            setSelected(allRowKeys);
        } else {
            setSelected(new Set());
        }
    }, [processed.rows]);

    // === RENDER ===
    return (
        <DataTableView
            rows={processed.rows}
            total={processed.total}
            columns={finalColumns}
            allColumns={columns}
            loading={props.loading || false}
            error={props.error}
            page={page}
            pageSize={pageSize}
            onPageChange={setPage}
            onPageSizeChange={(size) => { setPageSize(size); setPage(1); }}
            sortBy={sortBy}
            sortDir={sortDir}
            onToggleSort={onToggleSort}
            selectableRows={props.selectableRows || false}
            selected={selected}
            setSelected={setSelected}
            onSelectAll={handleSelectAll}
            allSelected={allSelected}
            someSelected={someSelected}
            title={props.title}
            description={props.description}
            onExport={handleExport}
            onPrint={handlePrint}
            visibleColumns={visibleColumns}
            onVisibleColumnsChange={setVisibleColumns}
            settingsDropdownContent={props.settingsDropdownContent}
            searchValue={searchInput}
            onSearchChange={setSearchInput}
            searchPlaceholder={props.searchPlaceholder}
            filters={filters}
            onFilterChange={handleFilterChange}
            onClearAll={clearAllFilters}
            onRefresh={handleRefresh}
            showFilterButtons={props.showFilterButtons || false}
            customFiltersContent={props.customFiltersContent}
            variant={props.variant || 'default'}
            density={props.density || 'normal'}
            emptyStateTitle={props.emptyStateTitle}
            emptyStateDescription={props.emptyStateDescription}
            clickColumnId={props.clickColumnId}
            onCellClick={props.onCellClick}
        />
    );
}