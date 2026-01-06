// src/pages/suppliers/index.tsx
import { useCallback, useEffect, useMemo, useState } from "react";
import {DataTable} from "@/components/ui"; // default export
import type { ColumnDef } from "@/components/ui/Table/types";
import { makeSupplierColumns } from "./_TableComponents/supplierColumns";
import { supplierApi, } from "@/services/api/endpoints/supplier/supplier.api";
import type { SupplierListItem, SupplierDetails } from "@/services/api/endpoints/supplier/supplier.types";

export default function SuppliersPage() {
    const [rows, setRows] = useState<SupplierListItem[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | undefined>(undefined);

    // --- DATA FETCH (tüm sayfaları topla) ---
    const fetchAll = useCallback(async () => {
        setLoading(true);
        setError(undefined);
        try {
            const pageSize = 100; // ihtiyaca göre büyütebilirsiniz
            let pageIndex = 1;
            let all: SupplierListItem[] = [];

            // ilk sayfa
            let page = await supplierApi.list({ pageIndex, pageSize, onlyActive: undefined });
            all = [...page.items];

            // kalan sayfalar
            while (all.length < (page.total ?? all.length)) {
                pageIndex += 1;
                page = await supplierApi.list({ pageIndex, pageSize, onlyActive: undefined });
                all = all.concat(page.items);
            }

            setRows(all);
        } catch (e: any) {
            setError(e?.message || "Liste alınırken bir hata oluştu.");
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchAll();
    }, [fetchAll]);

    // --- HANDLERS ---
    const handleEdit = useCallback((id: string) => {
        // Projenize göre yönlendirme/modal:
        // react-router: navigate(`/suppliers/${id}`)
        // Next.js: router.push(`/suppliers/${id}`)
        console.log("edit", id);
    }, []);

    const handleDelete = useCallback(async (id: string) => {
        if (!confirm("Bu tedarikçiyi silmek istiyor musunuz?")) return;
        setLoading(true);
        try {
            await supplierApi.remove(id);
            await fetchAll();
        } catch (e: any) {
            setError(e?.message || "Silme işleminde bir hata oluştu.");
        } finally {
            setLoading(false);
        }
    }, [fetchAll]);

    const handleToggleActive = useCallback(async (id: string, next: boolean) => {
        setLoading(true);
        try {
            // Update endpoint tam payload istediği için önce detayları alıyoruz:
            const d: SupplierDetails = await supplierApi.getDetails(id);
            await supplierApi.update(id, {
                companyName: d.companyName,
                contactPerson: d.contactPerson ?? null,
                contactEmail: d.contactEmail ?? null,
                phoneNumber: d.phoneNumber ?? null,
                address: d.address ?? null,
                isActive: next,
            });
            await fetchAll();
        } catch (e: any) {
            setError(e?.message || "Durum güncellenemedi.");
        } finally {
            setLoading(false);
        }
    }, [fetchAll]);

    // --- COLUMNS ---
    const columns: ColumnDef<SupplierListItem>[] = useMemo(
        () =>
            makeSupplierColumns({
                onEdit: handleEdit,
                onDelete: handleDelete,
                onToggleActive: handleToggleActive,
            }),
        [handleEdit, handleDelete, handleToggleActive]
    );

    return (
        <div className="p-6">
            <DataTable<SupplierListItem>
                data={rows}
                columns={columns}
                loading={loading}
                error={error}
                title="Tedarikçiler"
                description="Tedarikçi kayıtlarını görüntüle, ara, filtrele ve yönet."
                searchPlaceholder="Firma, kişi, e-posta, telefon ara…"
                showFilterButtons
                // Görsel varyantlar (sizde varsa):
                variant="modern"
                density="normal"
                // Boş durum metinleri:
                emptyStateTitle="Kayıt bulunamadı"
                emptyStateDescription="Filtreleri temizleyip tekrar deneyin veya yeni tedarikçi ekleyin."
                // Satır tıklaması / hücre tıklaması (opsiyonel)
                clickColumnId="companyName"
                onCellClick={(row) => handleEdit((row as SupplierListItem).id)}
            />
        </div>
    );
}
