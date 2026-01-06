import axios, { AxiosError } from "axios";

const isDev = import.meta.env.MODE === "development";

const baseURL = isDev
    ? "/api"
    : import.meta.env.VITE_API_BASE || "https://localhost:7053";

export interface ProblemDetails {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
    errorCode?: string;
    userFriendlyMessage?: string;
    developerDetail?: string;
    additionalData?: Record<string, unknown>;
    errors?: Record<string, string[]>; // validation hataları
}

export type AppError =
    | {
    kind: "validation";
    status: number;
    errors: Record<string, string[]>; // input bazlı hatalar
    title?: string;
    code?: string;
}
    | {
    kind: "problem";
    status: number;
    title?: string;
    message?: string;
    code?: string;
}
    | { kind: "network"; message: string };

export const http = axios.create({
    baseURL,
    withCredentials: true,
    headers: {
        "Content-Type": "application/json",
        Accept: "application/json, application/problem+json",
    },
});

function toAppError(err: unknown): AppError {
    const ax = err as AxiosError;
    if (!ax.response)
        return { kind: "network", message: ax.message || "Bağlantı hatası" };

    const status = ax.response.status ?? 0;
    const data = ax.response.data as ProblemDetails | undefined;

    // Validation hatası varsa
    if (data?.errors) {
        return {
            kind: "validation",
            status,
            errors: data.errors,
            title: data.title,
            code: data.errorCode,
        };
    }

    // Diğer problem detayları
    const message =
        data?.userFriendlyMessage?.trim() ||
        data?.detail?.trim() ||
        data?.title?.trim() ||
        ax.message;

    return {
        kind: "problem",
        status,
        title: data?.title,
        message,
        code: data?.errorCode,
    };
}

// Global hata yakalayıcı interceptor
http.interceptors.response.use(
    (r) => r,
    (error) => {
        const appError = toAppError(error);

        // Bütün hataları global olarak event ile yay
        window.dispatchEvent(
            new CustomEvent("app:error", { detail: appError })
        );

        // Validation ise form bileşenleri doğrudan kullanabilsin
        if (appError.kind === "validation") {
            // Form kütüphaneleri ile otomatik map edilebilir
            console.warn("Validation errors:", appError.errors);
        }

        return Promise.reject(appError);
    }
);
