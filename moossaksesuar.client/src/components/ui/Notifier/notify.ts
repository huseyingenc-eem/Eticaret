import type { NotifyPayload } from "./Notifier";

export const notify = {
    success(message: string, description?: string, duration?: number) {
        window.dispatchEvent(
            new CustomEvent<NotifyPayload>("app:notify", {
                detail: { type: "success", message, description, duration },
            })
        );
    },
    error(message: string, description?: string, duration?: number) {
        window.dispatchEvent(
            new CustomEvent<NotifyPayload>("app:notify", {
                detail: { type: "error", message, description, duration },
            })
        );
    },
    info(message: string, description?: string, duration?: number) {
        window.dispatchEvent(
            new CustomEvent<NotifyPayload>("app:notify", {
                detail: { type: "info", message, description, duration },
            })
        );
    },
    warning(message: string, description?: string, duration?: number) {
        window.dispatchEvent(
            new CustomEvent<NotifyPayload>("app:notify", {
                detail: { type: "warning", message, description, duration },
            })
        );
    },
};
