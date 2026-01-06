// src/components/Table/View/DataTableRow.tsx
import React from 'react';
import type { ColumnDef } from '../types';
import DataTableCell from './DataTableCell';

interface DataTableRowProps<T> {
    row: T;
    columns: ColumnDef<T>[];
    selectableRows?: boolean;
    isSelected: boolean;
    onToggleSelect: () => void;
    onRowClick?: (row: T) => void;
    clickColumnId?: string;
    onCellClick?: (row: T) => void;
    // DÜZELTME: Bu propları opsiyonel (?) yapıyoruz
    variant?: 'default' | 'modern' | 'minimal';
    density?: 'compact' | 'normal' | 'comfortable';
}

function DataTableRow<T>({
                             row,
                             columns,
                             selectableRows,
                             isSelected,
                             onToggleSelect,
                             onRowClick,
                             clickColumnId,
                             onCellClick,
                             // DÜZELTME: Tıpkı Body'de olduğu gibi burada da varsayılan değerler atıyoruz.
                             variant = 'default',
                             density = 'normal',
                         }: DataTableRowProps<T>) {
    const variantStyles = {
        default: 'border-b border-slate-100 hover:bg-slate-50',
        modern: 'border-b border-slate-100 hover:bg-gradient-to-r hover:from-slate-50',
        minimal: 'border-b border-slate-50 hover:bg-slate-25',
    };
    const selectedVariantStyles = {
        default: 'bg-blue-50 hover:bg-blue-100 border-blue-200',
        modern: 'bg-gradient-to-r from-blue-50 to-indigo-50',
        minimal: 'bg-slate-100',
    };

    const rowClickable = !!onRowClick && !clickColumnId;
    const baseClasses = "group transition-colors duration-200";
    const variantClass = isSelected ? selectedVariantStyles[variant] : variantStyles[variant];
    const rowClasses = `${baseClasses} ${variantClass} ${rowClickable ? 'cursor-pointer' : ''}`;

    return (
        <tr className={rowClasses} aria-selected={isSelected} onClick={() => rowClickable && onRowClick?.(row)}>
            {selectableRows && (
                <td className={`sticky left-0 z-10 bg-inherit border-r border-slate-100 ${density === 'compact' ? 'px-3' : 'px-4'} w-12`}>
                    <div className="flex items-center justify-center">
                        <input
                            type="checkbox"
                            checked={isSelected}
                            onClick={(e) => e.stopPropagation()}
                            onChange={onToggleSelect}
                            className="h-4 w-4 rounded border-2 border-slate-300 text-blue-600 focus:ring-2 focus:ring-blue-500/20"
                        />
                    </div>
                </td>
            )}
            {columns.map((col) => (
                <DataTableCell
                    key={col.id}
                    row={row}
                    col={col}
                    density={density}
                    variant={variant}
                    clickColumnId={clickColumnId}
                    onCellClick={onCellClick}
                />
            ))}
        </tr>
    );
}

export default React.memo(DataTableRow) as typeof DataTableRow;