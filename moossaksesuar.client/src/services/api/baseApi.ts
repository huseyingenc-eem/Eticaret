import type { AxiosRequestConfig } from "axios";
import { http } from "./http";

async function get<T>(url: string, config?: AxiosRequestConfig) {
    const { data } = await http.get<T>(url, config);
    return data;
}
async function post<TRes, TBody = unknown>(url: string, body?: TBody, config?: AxiosRequestConfig) {
    const { data } = await http.post<TRes>(url, body, config);
    return data;
}
async function put<TRes, TBody = unknown>(url: string, body?: TBody, config?: AxiosRequestConfig) {
    const { data } = await http.put<TRes>(url, body, config);
    return data;
}
async function patch<TRes, TBody = unknown>(url: string, body?: TBody, config?: AxiosRequestConfig) {
    const { data } = await http.patch<TRes>(url, body, config);
    return data;
}
async function del<T>(url: string, config?: AxiosRequestConfig) {
    const { data } = await http.delete<T>(url, config);
    return data;
}

export const baseApi = { get, post, put, patch, delete: del };
export type { AppError } from "./http";
