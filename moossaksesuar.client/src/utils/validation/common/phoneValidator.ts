// src/utils/validation/common/phoneValidator.ts
import type { ValidationResult } from '../types';

export const validatePhoneNumber = (phone: string): ValidationResult => {
    const phoneRegex = /^(\+90|0)?[5][0-9]{9}$/;
    const cleanPhone = phone.replace(/\s|-|\(|\)/g, '');

    if (!phoneRegex.test(cleanPhone)) {
        return {
            isValid: false,
            error: 'Geçerli bir telefon numarası girin (örn: 05xxxxxxxxx)'
        };
    }

    return { isValid: true };
};