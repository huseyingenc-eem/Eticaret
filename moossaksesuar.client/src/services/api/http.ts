import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import { navReplace } from "@/services/navigation";

const isDev = import.meta.env.DEV;
const API_BASE =
    isDev
        ? "/api"
        : (import.meta.env.VITE_API_BASE ??
            `${import.meta.env.VITE_BACKEND_URL}:${import.meta.env.VITE_BACKEND_PORT}`);

export interface ProblemDetails {
    type?: string; title?: string; status?: number; detail?: string; instance?: string;
    errorCode?: string; userFriendlyMessage?: string; developerDetail?: string;
    additionalData?: Record<string, unknown>;
    errors?: Record<string, string[]>;
}

export type AppError =
    | { kind: "validation"; status: number; errors: Record<string, string[]>; title?: string; code?: string }
    | { kind: "problem"; status: number; title?: string; message?: string; code?: string }
    | { kind: "network"; message: string };

export const http = axios.create({
    baseURL: API_BASE,
    withCredentials: true, // sadece cookie tabanlı auth kullanıyorsan gerekli
    headers: {
        "Content-Type": "application/json",
        "Accept": "application/json, application/problem+json",
    },
});

// İsteğe token ekle (Bearer)
http.interceptors.request.use((config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers = config.headers ?? {};
        (config.headers as any).Authorization = `Bearer ${token}`;
    }
    return config;
});

function toAppError(err: unknown): AppError {
    const ax = err as AxiosError;
    if (!ax.response) return { kind: "network", message: ax.message || "Bağlantı hatası" };

    const status = ax.response.status ?? 0;
    const data = ax.response.data as ProblemDetails | undefined;

    if (data?.errors) {
        return { kind: "validation", status, errors: data.errors, title: data.title, code: data.errorCode };
    }

    const message =
        data?.userFriendlyMessage?.trim() ||
        data?.detail?.trim() ||
        data?.title?.trim() ||
        ax.message;

    return { kind: "problem", status, title: data?.title, message, code: data?.errorCode };
}

http.interceptors.response.use(
    (r) => r,
    (error) => {
        const appError = toAppError(error);

        const current = window.location.pathname + window.location.search + window.location.hash;

        if (appError.kind === "problem") {
            if (appError.status === 401) {
                navReplace("/auth/login", { state: { from: { pathname: current } } });
            } else if (appError.status === 403) {
                navReplace("/403", { state: { from: { pathname: current } } });
            }
        }

        window.dispatchEvent(new CustomEvent("app:error", { detail: appError }));
        return Promise.reject(appError);
    }
);
