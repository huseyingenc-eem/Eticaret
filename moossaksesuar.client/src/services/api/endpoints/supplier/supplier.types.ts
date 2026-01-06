// src/services/api/endpoints/supplier/supplier.types.ts

export type SupplierListItem = {
    id: string;
    companyName: string;
    contactPerson?: string | null;
    phoneNumber?: string | null;
    isActive: boolean;
    createdTime: string;
};

export type SupplierDetails = {
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

export type CreateSupplierRequest = {
    companyName: string;
    contactPerson?: string | null;
    contactEmail?: string | null;
    phoneNumber?: string | null;
    address?: string | null;
    isActive: boolean;
};

export type UpdateSupplierRequest = {
    companyName: string;
    contactPerson?: string | null;
    contactEmail?: string | null;
    phoneNumber?: string | null;
    address?: string | null;
    isActive: boolean;
};

export type CreateSupplierResponse = {
    id: string;
    companyName: string;
    contactPerson?: string | null;
    contactEmail?: string | null;
    phoneNumber?: string | null;
    address?: string | null;
    isActive: boolean;
    createdTime: string;
    message: string;
};

export type UpdateSupplierResponse = {
    id: string;
    companyName: string;
    contactPerson?: string | null;
    contactEmail?: string | null;
    phoneNumber?: string | null;
    address?: string | null;
    isActive: boolean;
    updateTime: string;
    message: string;
};

export type DeleteSupplierResponse = {
    id: string;
    companyName: string;
    message: string;
};

export type SupplierListParams = {
    pageIndex?: number;
    pageSize?: number;
    companyNameSearch?: string | null;
    onlyActive?: boolean;
    status?: "all" | "active" | "inactive";
};

export type PagedResult<T> = {
    items: T[];
    count: number;
    index: number;
    size: number;
    pages: number;
    hasPrevious: boolean;
    hasNext: boolean;
};

export type Paged<T> = { items: T[]; total: number };