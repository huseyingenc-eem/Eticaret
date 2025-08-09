import type { ValidationResult } from '../types';

export const validateRequired = (value: string, fieldName: string): ValidationResult => {
    if (!value || value.trim() === '') {
        return {
            isValid: false,
            error: `${fieldName} gereklidir`
        };
    }
    return { isValid: true };
};

export const validateMinLength = (
    value: string,
    minLength: number,
    fieldName: string
): ValidationResult => {
    if (value.length < minLength) {
        return {
            isValid: false,
            error: `${fieldName} en az ${minLength} karakter olmalıdır`
        };
    }
    return { isValid: true };
};

export const validateMaxLength = (
    value: string,
    maxLength: number,
    fieldName: string
): ValidationResult => {
    if (value.length > maxLength) {
        return {
            isValid: false,
            error: `${fieldName} en fazla ${maxLength} karakter olabilir`
        };
    }
    return { isValid: true };
};

export const validatePattern = (
    value: string,
    pattern: RegExp,
    errorMessage: string
): ValidationResult => {
    if (!pattern.test(value)) {
        return {
            isValid: false,
            error: errorMessage
        };
    }
    return { isValid: true };
};