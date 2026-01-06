import type { ColumnDef, FilterValue } from '../types';
import DataTableHeader from './DataTableHeader';
import FilterButton from '../_components/FilterButton';

interface DataTableToolbarProps {
    title?: string;
    description?: string;

    // Header Props
    onExport: (format: 'csv' | 'xlsx' | 'pdf') => void;
    onPrint: () => void;
    columns: ColumnDef<any>[];
    visibleColumns: Record<string, boolean>;
    onVisibleColumnsChange: (columns: Record<string, boolean>) => void;
    pageSize: number;
    onPageSizeChange: (size: number) => void;
    density: "compact" | "normal" | "comfortable";

    // Filter Props
    filters: Record<string, FilterValue>;
    onFilterChange: (columnId: string, value: FilterValue) => void;
}

export default function DataTableToolbar({
                                             columns,
                                             filters,
                                             onFilterChange,
                                             ...headerProps
                                         }: DataTableToolbarProps) {

    const filterableColumns = columns.filter(col => col.filter);

    return (
        <div>
            <DataTableHeader
                columns={columns}
                {...headerProps}
            />

            {/* Eğer filtrelenebilir kolon varsa, filtre barını göster */}
            {filterableColumns.length > 0 && (
                <div className="flex items-center gap-2 px-4 py-3 border-b border-slate-200 bg-slate-50/50">
                    <span className="text-sm font-medium text-slate-600">Filtreler:</span>
                    {filterableColumns.map((col) => (
                        <FilterButton
                            key={col.id}
                            column={col}
                            value={filters[col.id]}
                            onChange={(value) => onFilterChange(col.id, value)}
                        />
                    ))}
                </div>
            )}
        </div>
    );
}