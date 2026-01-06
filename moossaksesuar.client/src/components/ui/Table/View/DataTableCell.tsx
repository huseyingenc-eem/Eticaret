import React from 'react';
import type { ColumnDef } from '../types';

interface DataTableCellProps<T> {
    row: T;
    col: ColumnDef<T>;
    density: 'compact' | 'normal' | 'comfortable';
    variant: 'default' | 'modern' | 'minimal';
    clickColumnId?: string;
    onCellClick?: (row: T) => void;
}

function getCellValue<T>(row: T, col: ColumnDef<T>): any {
    if (typeof col.accessor === 'function') return col.accessor(row);
    if (typeof col.accessor === 'string') return (row as any)[col.accessor];
    return (row as any)[col.id];
}

export default function DataTableCell<T>({
                                             row,
                                             col,
                                             density,
                                             variant,
                                             clickColumnId,
                                             onCellClick,
                                         }: DataTableCellProps<T>) {
    const variantStyles = {
        default: { cell: 'text-slate-700' },
        modern: { cell: 'text-slate-700' },
        minimal: { cell: 'text-slate-600' },
    };
    const densityStyles = {
        compact: 'px-3 py-2',
        normal: 'px-4 py-3',
        comfortable: 'px-6 py-4',
    };

    const currentVariant = variantStyles[variant];
    const cellPadding = densityStyles[density];
    const rawValue = getCellValue(row, col);
    const isClickCell = !!clickColumnId && col.id === clickColumnId;

    const cellClasses = [
        cellPadding,
        currentVariant.cell,
        col.className || '',
        col.sticky ? `sticky ${col.sticky === 'left' ? 'left-0' : 'right-0'} z-10 bg-inherit border-r border-slate-100` : '',
        'whitespace-nowrap overflow-hidden text-ellipsis',
        isClickCell ? 'text-blue-600 hover:text-blue-700 font-medium' : '',
    ].filter(Boolean).join(' ');

    const content = col.cell ? col.cell(rawValue, row) : (
        <span className={rawValue == null ? 'text-slate-400 italic' : ''}>
            {String(rawValue ?? '—')}
        </span>
    );

    const handleCellClick = (e: React.MouseEvent) => {
        e.stopPropagation();
        onCellClick?.(row);
    };

    return (
        <td className={cellClasses} title={String(rawValue ?? '')}>
            {isClickCell ? (
                <div role="button" tabIndex={0} className="inline-flex cursor-pointer" onClick={handleCellClick}>
                    {content}
                </div>
            ) : (
                content
            )}
        </td>
    );
}