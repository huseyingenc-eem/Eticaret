import type { UseFormSetError, Path } from "react-hook-form";
import type { AppError } from "@/services/api/http.ts";

export function applyValidationErrors<T extends Record<string, unknown>>(
    err: AppError,
    setError: UseFormSetError<T>,
    fieldMap?: Record<string, keyof T>
) {
    if (err.kind !== "validation") return false;

    Object.entries(err.errors).forEach(([serverField, messages]) => {
        const localField =
            fieldMap?.[serverField] ??
            (serverField.charAt(0).toLowerCase() + serverField.slice(1));

        setError(localField as Path<T>, {
            type: "server",
            message: messages.join(" "),
        });
    });

    return true;
}
