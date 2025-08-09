// src/utils/validation/types.ts
export interface ValidationResult {
    isValid: boolean;
    error?: string;
}

export interface ValidationRule {
    validator: (value: string) => boolean;
    message: string;
}

export interface FormValidationResult {
    isValid: boolean;
    errors: Record<string, string>;
}

export interface EmailValidationOptions {
    required?: boolean;
    domains?: string[];
    blacklistedDomains?: string[];
}

export interface PasswordValidationOptions {
    minLength?: number;
    maxLength?: number;
    requireUppercase?: boolean;
    requireLowercase?: boolean;
    requireNumbers?: boolean;
    requireSpecialChars?: boolean;
    forbiddenPatterns?: string[];
}