// src/components/ui/Table/hooks/useClientSideDataProcessing.ts
import { useMemo, useDeferredValue } from 'react';
import type { ColumnDef, FilterValue, SortDir } from '../types';
import { getCellValue, defaultPredicate, toComparable } from '../utils';

export function useClientSideDataProcessing<T>({
                                                   data,
                                                   columns,
                                                   filters,
                                                   sortBy,
                                                   sortDir,
                                                   page,
                                                   pageSize,
                                                   searchInput,
                                               }: {
    data: T[];
    columns: ColumnDef<T>[];
    filters: Record<string, FilterValue>;
    sortBy: string | null;
    sortDir: SortDir;
    page: number;
    pageSize: number;
    searchInput: string;
}) {
    const deferredSearch = useDeferredValue(searchInput);

    return useMemo(() => {
        let rows = [...data];

        // 1. Adım: Sütun Filtreleri
        rows = rows.filter(row =>
            columns.every(col => {
                const conf = col.filter;
                if (!conf) return true;
                const fv = filters[col.id];
                if (fv == null || fv === '' || (Array.isArray(fv) && fv.length === 0)) return true;
                const cell = getCellValue(row, col);
                if (conf.predicate) return conf.predicate(cell, fv, row);
                return defaultPredicate(conf.type, cell, fv);
            })
        );

        // 2. Adım: Genel Arama
        const needle = deferredSearch.trim().toLowerCase();
        if (needle) {
            rows = rows.filter(row =>
                columns.some(col =>
                    String(getCellValue(row, col) ?? '').toLowerCase().includes(needle)
                )
            );
        }

        // 3. Adım: Sıralama
        if (sortBy && sortDir) {
            const col = columns.find(c => c.id === sortBy);
            if (col) {
                const dir = sortDir === 'asc' ? 1 : -1;
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

        const total = rows.length;

        // 4. Adım: Sayfalama
        const start = (page - 1) * pageSize;
        const pagedRows = rows.slice(start, start + pageSize);

        return { processedRows: pagedRows, totalCount: total };
    }, [data, columns, filters, sortBy, sortDir, page, pageSize, deferredSearch]);
}