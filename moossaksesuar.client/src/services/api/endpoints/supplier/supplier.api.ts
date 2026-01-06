// src/services/api/endpoints/supplier/supplier.api.ts

import { baseApi } from "@/services/api/baseApi.ts";
import type {
    SupplierListItem,
    SupplierDetails,
    CreateSupplierRequest,
    UpdateSupplierRequest,
    CreateSupplierResponse,
    UpdateSupplierResponse,
    DeleteSupplierResponse,
    SupplierListParams,
    PagedResult,
    Paged
} from "./supplier.types.ts";

// Raw DTO types from backend
type RawSupplierListDto = {
    id: string;
    companyName: string;
    contactPerson?: string | null;
    phoneNumber?: string | null;
    isActive: boolean;
    createdTime: string;
};

type RawSupplierDetailsDto = {
    id: string;
    companyName: string;
    contactPerson?: string | null;
    contactEmail?: string | null;
    phoneNumber?: string | null;
    address?: string | null;
    isActive: boolean;
    createdTime: string;
    updateTime?: string | null;
};

// API Base Path
const BASE = "/Suppliers";

// Filter Helper
function filterByStatus(
    items: SupplierListItem[],
    status: "all" | "active" | "inactive" = "all"
) {
    if (status === "all") return items;
    const wantActive = status === "active";
    return items.filter((x) => !!x.isActive === wantActive);
}

// --- List Function ---
async function list(params: SupplierListParams = {}): Promise<Paged<SupplierListItem>> {
    const queryParams = new URLSearchParams();

    if (params.pageIndex !== undefined) queryParams.append('pageIndex', params.pageIndex.toString());
    if (params.pageSize !== undefined) queryParams.append('pageSize', params.pageSize.toString());
    if (params.companyNameSearch) queryParams.append('companyNameSearch', params.companyNameSearch);
    if (params.onlyActive !== undefined) queryParams.append('onlyActive', params.onlyActive.toString());

    const url = `${BASE}/list${queryParams.toString() ? '?' + queryParams.toString() : ''}`;

    const response = await baseApi.get<PagedResult<RawSupplierListDto>>(url);

    const items: SupplierListItem[] = (response.items ?? []).map((x) => ({
        id: x.id,
        companyName: x.companyName,
        contactPerson: x.contactPerson,
        phoneNumber: x.phoneNumber,
        isActive: x.isActive,
        createdTime: x.createdTime
    }));

    const filtered = filterByStatus(items, params.status);

    return {
        items: filtered,
        total: response.count
    };
}

// --- Get Details Function ---
async function getDetails(id: string): Promise<SupplierDetails> {
    const response = await baseApi.get<RawSupplierDetailsDto>(`${BASE}/${id}`);

    return {
        id: response.id,
        companyName: response.companyName,
        contactPerson: response.contactPerson,
        contactEmail: response.contactEmail,
        phoneNumber: response.phoneNumber,
        address: response.address,
        isActive: response.isActive,
        createdTime: response.createdTime,
        updateTime: response.updateTime
    };
}

// --- CRUD Functions ---
async function create(payload: CreateSupplierRequest): Promise<CreateSupplierResponse> {
    return await baseApi.post<CreateSupplierResponse, CreateSupplierRequest>(
        `${BASE}/create`,
        payload
    );
}

async function update(id: string, payload: UpdateSupplierRequest): Promise<UpdateSupplierResponse> {
    return await baseApi.put<UpdateSupplierResponse, UpdateSupplierRequest & { id: string }>(
        `${BASE}/update`,
        { id, ...payload }
    );
}

async function remove(id: string): Promise<DeleteSupplierResponse> {
    return await baseApi.delete<DeleteSupplierResponse>(`${BASE}/delete/${id}`);
}

export const supplierApi = {
    list,
    getDetails,
    create,
    update,
    remove
};