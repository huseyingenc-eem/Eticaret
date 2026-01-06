// src/components/Table/View/DataTableBody.tsx

import React, { memo, useCallback } from 'react';
import { FileX } from 'lucide-react';
import type { ColumnDef } from '../types';
import DataTableRow from './DataTableRow';

interface Props<T> {
    rows: T[];
    columns: ColumnDef<T>[];
    selectableRows?: boolean;
    selected: Set<React.Key>;
    // HATA BURADAYDI: Tip, fonksiyonel güncellemeleri (prev => ...) desteklemiyordu.
    // DÜZELTİLMİŞ HALİ: React'in standart state setter tipini kullanıyoruz.
    setSelected: React.Dispatch<React.SetStateAction<Set<React.Key>>>;
    onRowClick?: (row: T) => void;
    clickColumnId?: string;
    onCellClick?: (row: T) => void;
    rowId?: (row: T, index: number) => React.Key;
    emptyStateTitle?: string;
    emptyStateDescription?: string;
    variant?: 'default' | 'modern' | 'minimal';
    density?: 'compact' | 'normal' | 'comfortable';
}

function DataTableBody<T>({
                              rows,
                              columns,
                              rowId,
                              selected,
                              setSelected,
                              emptyStateTitle,
                              emptyStateDescription,
                              ...rest
                          }: Props<T>) {

    // Bu fonksiyon artık doğru tiplerle çalışacaktır.
    const toggleSelect = useCallback((key: React.Key) => {
        setSelected((prev) => { // 'prev' artık 'Set<React.Key>' olarak doğru şekilde tiplenecek.
            const next = new Set(prev);
            next.has(key) ? next.delete(key) : next.add(key);
            return next;
        });
    }, [setSelected]);

    return (
        <tbody className="bg-white divide-y divide-slate-100">
        {rows.length > 0 ? (
            rows.map((row, rowIndex) => {
                const key = rowId ? rowId(row, rowIndex) : rowIndex;
                const isSelected = selected.has(key);
                return (
                    <DataTableRow
                        key={key}
                        row={row}
                        columns={columns}
                        isSelected={isSelected}
                        onToggleSelect={() => toggleSelect(key)}
                        {...rest}
                    />
                );
            })
        ) : (
            <tr>
                <td colSpan={columns.length + (rest.selectableRows ? 1 : 0)} className="px-6 py-16 text-center">
                    <div className="flex flex-col items-center justify-center space-y-3">
                        <div className="w-16 h-16 bg-slate-100 rounded-full flex items-center justify-center">
                            <FileX className="w-8 h-8 text-slate-400" />
                        </div>
                        <div>
                            <h3 className="text-base font-medium text-slate-600">{emptyStateTitle}</h3>
                            <p className="text-sm text-slate-400">{emptyStateDescription}</p>
                        </div>
                    </div>
                </td>
            </tr>
        )}
        </tbody>
    );
}

export default memo(DataTableBody) as typeof DataTableBody;