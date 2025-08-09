// src/utils/validation/index.ts
// Email validations
export {
    validateEmail,
    isValidEmailDomain,
    getEmailProvider
} from './email';

// Password validations
export {
    validatePassword,
    getPasswordStrength,
    validatePasswordConfirmation
} from './password';

// Form validations
export {
    validateLoginForm,
    validateRegisterForm,
    type LoginFormData,
    type RegisterFormData
} from './form';

// Common validations
export {
    validateRequired,
    validateMinLength,
    validateMaxLength,
    validatePattern,
    validatePhoneNumber,
    validateTCKN
} from './common';

// Types
export type {
    ValidationResult,
    ValidationRule,
    FormValidationResult,
    EmailValidationOptions,
    PasswordValidationOptions
} from './types';

// Validation constants
export const VALIDATION_RULES = {
    EMAIL: {
        MAX_LENGTH: 254,
        MIN_LENGTH: 5
    },
    PASSWORD: {
        MIN_LENGTH: 8,
        MAX_LENGTH: 128
    },
    NAME: {
        MIN_LENGTH: 2,
        MAX_LENGTH: 50
    },
    PHONE: {
        LENGTH: 11
    },
    TCKN: {
        LENGTH: 11
    }
} as const;