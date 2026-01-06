// src/pages/categories/_components/CategoryTable.tsx
import React from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import toast from "react-hot-toast";

import { DataTable } from "@/components/ui";

import type { ColumnDef } from "@/components/ui/Table/types";
import type { CategoryListItem } from "@/services/api/endpoints/category/category.types";
import { categoryApi } from "@/services/api";
import { buildCategoryPath } from "@/utils/paths";
import { makeCategoryColumns } from "../_utils/categoryColumns";

type Props = {
    items?: CategoryListItem[];
    disableStatusFilter?: boolean;
    onDelete?: (id: number) => void;
    onToggleActive?: (id: number, next: boolean) => void;
    onEdit?: (id: number) => void;
    columnsOverride?: ColumnDef<CategoryListItem>[];
};

export default function CategoryTable({
                                          items,
                                          disableStatusFilter,
                                          onDelete,
                                          onToggleActive,
                                          onEdit,
                                      }: Props) {
    const [data, setData] = React.useState<CategoryListItem[]>(items ?? []);
    const [loading, setLoading] = React.useState(false);
    const [pageSize] = React.useState(10);
    const [sp] = useSearchParams();
    const nav = useNavigate();

    const status = (sp.get("status") as "all" | "active" | "inactive") ?? "all";
    const controlled = Array.isArray(items);

    React.useEffect(() => {
        if (controlled) setData(items ?? []);
    }, [items, controlled]);

    const load = React.useCallback(async () => {
        if (controlled) return;
        setLoading(true);
        try {
            const res = await categoryApi.list({ parentId: null, status });
            setData(res?.items ?? []);
        } catch (e: any) {
            toast.error(e?.message ?? "Kategori listesi alınamadı");
        } finally {
            setLoading(false);
        }
    }, [status, controlled]);

    React.useEffect(() => {
        void load();
    }, [load]);

    const handleDelete = async (id: number) => {
        if (onDelete) return onDelete(id);
        if (!confirm("Bu kategoriyi silmek istiyor musunuz?")) return;
        try {
            await categoryApi.remove(id);
            setData((list) => list.filter((x) => x.id !== id));
            toast.success("Kategori silindi");
        } catch {
            toast.error("Silme işleminde hata");
        }
    };

    const handleToggleActive = async (id: number, next: boolean) => {
        if (onToggleActive) return onToggleActive(id, next);
        const prev = [...data];
        const row = data.find((x) => x.id === id);
        if (!row) return;
        setData((list) => list.map((x) => (x.id === id ? { ...x, isActive: next } : x)));
        try {
            await categoryApi.update(id, { isActive: next, name: row.name, parentId: row.parentId ?? null });
            toast.success(next ? "Aktif yapıldı" : "Pasif yapıldı");
        } catch {
            setData(prev);
            toast.error("Durum güncellenemedi");
        }
    };

    const handleEdit = (id: number) => {
        if (onEdit) return onEdit(id);
        const row = data.find((x) => x.id === id);
        nav(buildCategoryPath(id, row?.name), { state: { name: row?.name } });
    };

    // Kolonları tek bir yerden üret (actions + durum cell’leri burada bağlanıyor)
    const columns = React.useMemo(
        () =>
            makeCategoryColumns({
                onEdit: handleEdit,
                onDelete: handleDelete,
                onToggleActive: handleToggleActive,
                disableStatusFilter,
            }),
        [handleEdit, handleDelete, handleToggleActive, disableStatusFilter]
    );

    // Basit client-side filtre (URL status paramına göre)
    const filteredData = React.useMemo(() => {
        return data.filter((row) => {
            if (status === "active" && !row.isActive) return false;
            if (status === "inactive" && row.isActive) return false;
            return true;
        });
    }, [data, status]);

    return (
        <DataTable
            key={`cat-${pageSize}`}
            title="Kategoriler"
            emptyStateTitle="Kategoriler Bulunamadı"
            emptyStateDescription="Bu kategorinin alt kategorisi yok ya da filtrelerinizi gözden geçirin."
            density="normal"
            columns={columns}
            data={filteredData}
            selectableRows={true}
            initialPageSize={pageSize}
            showFilterButtons={true}
            loading={loading}
            clickColumnId="name"
            onCellClick={(row) => {
                const id = (row as any)?.id ?? (row as any)?.original?.id;
                if (typeof id === "number") handleEdit(id);
            }}
        />
    );
}
