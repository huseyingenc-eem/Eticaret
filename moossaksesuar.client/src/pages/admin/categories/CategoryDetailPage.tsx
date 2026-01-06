import { useCallback, useEffect, useMemo, useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import toast from "react-hot-toast";
import { Plus, RefreshCw } from "lucide-react";
import { Button } from "@/components/ui";

import { categoryApi } from "@/services/api";
import type {
    AncestorNode,
    CategoryListItem,
    CategoryTreeNode,
    CategoryDetails,
    UpdateCategoryRequest
} from "@/services/api/endpoints/category/category.types";
import { buildCategoryPath, parseIdFromIdSlug } from "@/utils/paths";

// Sayfaya özel alt bileşenler
import { CategoryTable, CategoryBreadcrumb, CategoryCreateModal, CategoryEditForm } from "./_components";


// Helper: Verilen bir ID'ye göre kategori ağacındaki yolu (ataları) bulan fonksiyon.
function findPath(nodes: CategoryTreeNode[], targetId: number): AncestorNode[] | null {
    for (const node of nodes) {
        if (node.id === targetId) {
            return [];
        }
        if (node.children && node.children.length > 0) {
            const subPath = findPath(node.children, targetId);
            if (subPath !== null) {
                return [{ id: node.id, name: node.name }, ...subPath];
            }
        }
    }
    return null;
}

export default function CategoryDetailPage() {
    // --- Hooks ve State Tanımlamaları ---
    const { idSlug } = useParams();
    const nav = useNavigate();
    const location = useLocation();

    const id = useMemo(() => parseIdFromIdSlug(idSlug), [idSlug]);
    const hasId = Number.isFinite(id);

    // Component State'leri
    const [categoryDetails, setCategoryDetails] = useState<CategoryDetails | null>(null);
    const [ancestors, setAncestors] = useState<AncestorNode[]>([]);
    const [children, setChildren] = useState<CategoryListItem[]>([]);
    const [loading, setLoading] = useState(true);
    const [openCreate, setOpenCreate] = useState(false);

    // --- Veri Yükleme Mantığı ---

    const loadPageData = useCallback(async () => {
        if (!hasId) return;
        setLoading(true);
        try {
            const [details, tree, childrenPaged] = await Promise.all([
                categoryApi.getDetails(id as number),
                categoryApi.getFullTree(),
                categoryApi.list({ parentId: id as number, status: "all" }),
            ]);

            if (!details || !tree) throw new Error("Kategori bilgileri alınamadı.");

            const canonicalPath = buildCategoryPath(details.id, details.name);
            if (location.pathname !== canonicalPath) {
                nav(`${canonicalPath}${location.search}`, { replace: true });
                return;
            }

            const path = findPath(tree, id as number);

            setCategoryDetails(details);
            setAncestors(path ?? []);
            setChildren(childrenPaged?.items ?? []);
        } catch (error: any) {
            toast.error(error?.message ?? `Kategori #${id} bulunamadı.`);
            nav("/admin/categories");
        } finally {
            setLoading(false);
        }
    }, [id, hasId, location.pathname, location.search, nav]);

    useEffect(() => {
        void loadPageData();
    }, [loadPageData]);


    // --- Olay Yöneticileri (Event Handlers) ---

    const handleDetailsSave = async (payload: UpdateCategoryRequest) => {
        if (!categoryDetails) return;
        try {
            await categoryApi.update(categoryDetails.id, {
                ...payload,
                parentId: categoryDetails.parentId, // ParentId'nin değişmediğini garantile
            });
            await loadPageData(); // Tüm sayfa verisini yenile
        } catch (error: any) {
            toast.error(error?.message ?? "Güncelleme başarısız.");
            throw error;
        }
    };

    const loadChildren = useCallback(async () => {
        if (!hasId) return;
        setLoading(true);
        try {
            const listPaged = await categoryApi.list({ parentId: id as number, status: "all" });
            setChildren(listPaged?.items ?? []);
        } catch (e: any) {
            toast.error(e?.message ?? "Alt kategoriler alınamadı");
        } finally {
            setLoading(false);
        }
    }, [id, hasId]);

    const onDelete = async (cid: number) => {
        if (!confirm("Bu kategoriyi silmek istiyor musunuz?")) return;
        try {
            await categoryApi.remove(cid);
            await loadChildren();
            toast.success("Kategori silindi");
        } catch (e: any) {
            toast.error(e?.message ?? "Silme işleminde hata");
        }
    };

    const onToggleActive = async (cid: number, next: boolean) => {
        const row = children.find((x) => x.id === cid);
        if (!row) return;

        const originalChildren = [...children];
        setChildren((list) => list.map((x) => (x.id === cid ? { ...x, isActive: next } : x)));

        try {
            await categoryApi.update(cid, { isActive: next, name: row.name, parentId: id as number });
            toast.success(next ? "Aktif yapıldı" : "Pasif yapıldı");
        } catch (e: any) {
            setChildren(originalChildren);
            toast.error(e?.message ?? "Durum güncellenemedi");
        }
    };

    const onEdit = (cid: number, name?: string) => {
        const categoryName = name ?? children.find((x) => x.id === cid)?.name;
        nav(buildCategoryPath(cid, categoryName), { state: { name: categoryName } });
    };

    // --- Render (JSX) ---
    return (
        <div className="p-6 space-y-6">
            {/* Başlık ve Aksiyonlar */}
            <div className="flex flex-wrap items-center justify-between gap-3">
                <div className="space-y-1">
                    <h1 className="text-xl font-semibold">{categoryDetails?.name || (hasId ? `Kategori #${id}` : "Kategori Detay")}</h1>
                    <CategoryBreadcrumb
                        path={ancestors}
                        current={categoryDetails ? { id: categoryDetails.id, name: categoryDetails.name } : null}
                    />
                </div>
                <div className="flex items-center gap-2">
                    <Button  color={"info"} size={"xs"}
                        onClick={loadChildren}
                        disabled={loading || !hasId} >
                        <RefreshCw className={`h-4 w-4 ${loading ? "animate-spin" : ""}`} />
                        Yenile
                    </Button>
                    <Button
                        onClick={() => setOpenCreate(true)}
                        variant={"ghost"}  size={"xs"} color={"primary"}
                        disabled={!hasId}
                    >
                        <Plus className="h-4 w-4" />
                        Yeni Alt Kategori
                    </Button>
                </div>
            </div>

            {/* Kategori Detay/Düzenleme Formu (Rozet) */}
            {loading && !categoryDetails ? (
                <div className="p-4 border rounded-lg bg-white text-center text-gray-500">Yükleniyor...</div>
            ) : categoryDetails ? (
                <CategoryEditForm
                    initialData={categoryDetails}
                    onSave={handleDetailsSave}
                />
            ) : null}

            <hr/>

            {/* Alt Kategoriler Alanı */}
            <div className="space-y-4">
                <CategoryTable
                    items={children}
                    onDelete={onDelete}
                    onToggleActive={onToggleActive}
                    onEdit={onEdit}
                />
            </div>

            {/* Yeni Alt Kategori Oluşturma Modalı */}
            <CategoryCreateModal
                open={openCreate}
                onClose={() => setOpenCreate(false)}
                defaultParentId={hasId ? (id as number) : null}
                onSaved={loadChildren}
            />
        </div>
    );
}