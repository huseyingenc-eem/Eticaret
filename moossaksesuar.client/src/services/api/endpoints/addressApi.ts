import { apiClient } from '../baseApi.ts';
import type {
    Address,
    CreateAddressRequest,
    UpdateAddressRequest,
    PaginatedResponse
} from '../types/address.types';

export const addressApi = {
    // User endpoints
    getMyAddresses: () =>
        apiClient.get<Address[]>('/Address/my-addresses'),

    getById: (id: string) =>
        apiClient.get<Address>(`/Address/getbyid/${id}`),

    create: (data: CreateAddressRequest) =>
        apiClient.post<Address>('/Address/create', data),

    update: (data: UpdateAddressRequest) =>
        apiClient.put<Address>('/Address/update', data),

    delete: (id: string) =>
        apiClient.delete(`/Address/delete/${id}`),

    // Admin endpoints
    getDefaultShipping: (page?: number, size?: number) =>
        apiClient.get<PaginatedResponse<Address>>('/Address/admin/default-shipping', {
            params: { page, size }
        }),

    getUserAllAddresses: (userId: string) =>
        apiClient.get<PaginatedResponse<Address>>(`/Address/admin/user/${userId}/all`),
};