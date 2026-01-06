// src/components/ui/Table/hooks/useDataTableState.ts
import { useReducer, useCallback } from 'react';
import type { ColumnDef, FilterValue, SortDir } from '../types';

interface State<T> {
    page: number;
    pageSize: number;
    sortBy: string | null;
    sortDir: SortDir;
    filters: Record<string, FilterValue>;
    visibleColumns: Record<string, boolean>;
    searchInput: string;
}

// Reducer eylemleri
type Action<T> =
    | { type: 'SET_PAGE'; payload: number }
    | { type: 'SET_PAGE_SIZE'; payload: number }
    | { type: 'TOGGLE_SORT'; payload: ColumnDef<T> }
    | { type: 'SET_FILTER'; payload: { columnId: string; value: FilterValue } }
    | { type: 'SET_VISIBLE_COLUMNS'; payload: Record<string, boolean> }
    | { type: 'SET_SEARCH'; payload: string }
    | { type: 'CLEAR_ALL_FILTERS' };

function initState<T>(columns: ColumnDef<T>[], initialPageSize: number, initialFilters: Record<string, FilterValue>): State<T> {
    return {
        page: 1,
        pageSize: initialPageSize,
        sortBy: null,
        sortDir: null,
        filters: initialFilters,
        visibleColumns: columns.reduce((acc, col) => ({ ...acc, [col.id]: true }), {}),
        searchInput: '',
    };
}

function reducer<T>(state: State<T>, action: Action<T>): State<T> {
    switch (action.type) {
        case 'SET_PAGE':
            return { ...state, page: action.payload };
        case 'SET_PAGE_SIZE':
            return { ...state, pageSize: action.payload, page: 1 }; // Sayfa boyutu değişince 1. sayfaya dön
        case 'TOGGLE_SORT':
            if (state.sortBy !== action.payload.id) {
                return { ...state, sortBy: action.payload.id, sortDir: 'asc', page: 1 };
            }
            const nextDir = state.sortDir === 'asc' ? 'desc' : state.sortDir === 'desc' ? null : 'asc';
            return {
                ...state,
                sortDir: nextDir,
                sortBy: nextDir === null ? null : state.sortBy,
                page: 1,
            };
        case 'SET_FILTER':
            return {
                ...state,
                filters: { ...state.filters, [action.payload.columnId]: action.payload.value },
                page: 1,
            };
        case 'SET_VISIBLE_COLUMNS':
            return { ...state, visibleColumns: action.payload };
        case 'SET_SEARCH':
            return { ...state, searchInput: action.payload, page: 1 };
        case 'CLEAR_ALL_FILTERS':
            return { ...state, filters: {}, searchInput: '', page: 1 };
        default:
            return state;
    }
}

export function useDataTableState<T>({ columns, initialPageSize = 10, initialFilters = {} }: {
    columns: ColumnDef<T>[];
    initialPageSize?: number;
    initialFilters?: Record<string, FilterValue>;
}) {
    const [state, dispatch] = useReducer<React.Reducer<State<T>, Action<T>>>(
        reducer,
        initState(columns, initialPageSize, initialFilters)
    );

    // Handler'ları useCallback ile sarmalayarak performansı artırıyoruz
    const onPageChange = useCallback((page: number) => dispatch({ type: 'SET_PAGE', payload: page }), []);
    const onPageSizeChange = useCallback((size: number) => dispatch({ type: 'SET_PAGE_SIZE', payload: size }), []);
    const onToggleSort = useCallback((col: ColumnDef<T>) => dispatch({ type: 'TOGGLE_SORT', payload: col }), []);
    const onFilterChange = useCallback((columnId: string, value: FilterValue) => dispatch({ type: 'SET_FILTER', payload: { columnId, value } }), []);
    const onVisibleColumnsChange = useCallback((cols: Record<string, boolean>) => dispatch({ type: 'SET_VISIBLE_COLUMNS', payload: cols }), []);
    const onSearchChange = useCallback((value: string) => dispatch({ type: 'SET_SEARCH', payload: value }), []);
    const onClearAll = useCallback(() => dispatch({ type: 'CLEAR_ALL_FILTERS' }), []);

    return {
        state,
        handlers: {
            onPageChange,
            onPageSizeChange,
            onToggleSort,
            onFilterChange,
            onVisibleColumnsChange,
            onSearchChange,
            onClearAll,
        },
    };
}