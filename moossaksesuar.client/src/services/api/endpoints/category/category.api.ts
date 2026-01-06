// src/services/api/endpoints/category/category.api.ts

import { baseApi } from "@/services/api/baseApi.ts";
import type {
    CategoryListItem,
    CreateCategoryRequest,
    Paged,
    UpdateCategoryRequest,
    CategoryDetails,
    CategoryTreeNode
} from "./category.types.ts";

type RawCategoryDto = {
    id: number;
    name: string;
    description?: string | null;
    parentId?: number | null;
    isActive: boolean;
    childCount?: number;
};

// API Base Path
const BASE = "/Category";

// Filter Helper
function filterByStatus(
    items: CategoryListItem[],
    status: "all" | "active" | "inactive" = "all"
) {
    if (status === "all") return items;
    const wantActive = status === "active";
    return items.filter((x) => !!x.isActive === wantActive);
}


async function getFullTree(): Promise<CategoryTreeNode[]> {
    return await baseApi.get<CategoryTreeNode[]>(`${BASE}/GetCategoryTree`);
}

// --- List Parent/Child ---
async function list(params: {
    parentId: number | null;
    status?: "all" | "active" | "inactive";
}): Promise<Paged<CategoryListItem>> {
    const url =
        params.parentId === null
            ? `${BASE}/GetParentCategories`
            : `${BASE}/GetChildCategories/${params.parentId}`;

    const rawItems = await baseApi.get<RawCategoryDto[]>(url);

    const items: CategoryListItem[] = (rawItems ?? []).map((x) => ({
        id: x.id,
        name: x.name,
        description: x.description ?? "",
        parentId: x.parentId ?? null,
        isActive: x.isActive,
        childCount: x.childCount ?? 0
    }));

    const filtered = filterByStatus(items, params.status);
    return { items: filtered, total: filtered.length };
}

// --- CRUD Functions ---
function create(payload: CreateCategoryRequest): Promise<void> {
    return baseApi.post<void, CreateCategoryRequest>(`${BASE}/add`, payload);
}

function update(id: number, payload: UpdateCategoryRequest): Promise<void> {
    return baseApi.put<void, UpdateCategoryRequest & { id: number }>(
        `${BASE}/update`,
        { id, ...payload }
    );
}

function updateRange(payload: any): Promise<void> {
    return baseApi.post<void, any>(`${BASE}/update-range`, payload);
}

function remove(id: number): Promise<void> {
    return baseApi.delete<void>(`${BASE}/delete/${id}`);
}

function getDetails(id: number): Promise<CategoryDetails> {
    return baseApi.get<CategoryDetails>(`${BASE}/GetById/${id}`);
}

export const categoryApi = {
    list,
    create,
    update,
    updateRange,
    remove,
    getFullTree,
    getDetails
};
