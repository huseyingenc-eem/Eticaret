import { baseApi } from '../baseApi.ts';
import type {
    Address,
    CreateAddressRequest,
    UpdateAddressRequest,
    PaginatedResponse
} from '../types/address.types';

export const addressApi = {
    // User endpoints
    getMyAddresses: () =>
        baseApi.get<Address[]>('/Address/my-addresses'),

    getById: (id: string) =>
        baseApi.get<Address>(`/Address/getbyid/${id}`),

    create: (data: CreateAddressRequest) =>
        baseApi.post<Address>('/Address/create', data),

    update: (data: UpdateAddressRequest) =>
        baseApi.put<Address>('/Address/update', data),

    delete: (id: string) =>
        baseApi.delete(`/Address/delete/${id}`),

    // Admin endpoints
    getDefaultShipping: (page?: number, size?: number) =>
        baseApi.get<PaginatedResponse<Address>>('/Address/admin/default-shipping', {
            params: { page, size }
        }),

    getUserAllAddresses: (userId: string) =>
        baseApi.get<PaginatedResponse<Address>>(`/Address/admin/user/${userId}/all`),
};