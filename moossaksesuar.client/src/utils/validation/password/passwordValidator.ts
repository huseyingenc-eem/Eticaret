// src/utils/validation/password/passwordValidator.ts
import type { ValidationResult, PasswordValidationOptions } from '../types';

// Yaygın zayıf şifreler
const COMMON_PASSWORDS = [
    '123456', 'password', '123456789', '12345678', '12345',
    '1234567', 'qwerty', 'abc123', 'Password', '123123',
    'admin', 'letmein', 'welcome', 'monkey', '1234567890'
];

// Türkçe yaygın şifreler
const TURKISH_COMMON_PASSWORDS = [
    'sifre', 'parola', '123456', 'asdasd', 'qwerty',
    'galatasaray', 'fenerbahce', 'besiktas', 'trabzon'
];

export const validatePassword = (
    password: string,
    options: PasswordValidationOptions = {}
): ValidationResult => {
    // ... mevcut kod aynı kalacak
    const {
        minLength = 8,
        maxLength = 128,
        requireUppercase = true,
        requireLowercase = true,
        requireNumbers = true,
        requireSpecialChars = true,
        forbiddenPatterns = []
    } = options;

    if (!password) {
        return {
            isValid: false,
            error: 'Şifre gereklidir'
        };
    }

    if (password.length < minLength) {
        return {
            isValid: false,
            error: `Şifre en az ${minLength} karakter olmalıdır`
        };
    }

    if (password.length > maxLength) {
        return {
            isValid: false,
            error: `Şifre en fazla ${maxLength} karakter olmalıdır`
        };
    }

    if (requireUppercase && !/[A-Z]/.test(password)) {
        return {
            isValid: false,
            error: 'Şifre en az bir büyük harf içermelidir'
        };
    }

    if (requireLowercase && !/[a-z]/.test(password)) {
        return {
            isValid: false,
            error: 'Şifre en az bir küçük harf içermelidir'
        };
    }

    if (requireNumbers && !/\d/.test(password)) {
        return {
            isValid: false,
            error: 'Şifre en az bir rakam içermelidir'
        };
    }

    if (requireSpecialChars && !/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) {
        return {
            isValid: false,
            error: 'Şifre en az bir özel karakter içermelidir (!@#$%^&*)'
        };
    }

    const lowerPassword = password.toLowerCase();
    if (COMMON_PASSWORDS.includes(lowerPassword) ||
        TURKISH_COMMON_PASSWORDS.includes(lowerPassword)) {
        return {
            isValid: false,
            error: 'Bu şifre çok yaygın kullanılıyor, daha güçlü bir şifre seçin'
        };
    }

    if (/(.)\1{3,}/.test(password)) {
        return {
            isValid: false,
            error: 'Şifre çok fazla tekrarlayan karakter içeriyor'
        };
    }

    const sequences = ['123', '234', '345', '456', '567', '678', '789', '890',
        'abc', 'bcd', 'cde', 'def', 'efg', 'fgh', 'ghi', 'hij',
        'qwe', 'wer', 'ert', 'rty', 'tyu', 'yui', 'uio', 'iop'];

    for (const seq of sequences) {
        if (lowerPassword.includes(seq) || lowerPassword.includes(seq.split('').reverse().join(''))) {
            return {
                isValid: false,
                error: 'Şifre sıralı karakterler içermemelidir'
            };
        }
    }

    for (const pattern of forbiddenPatterns) {
        if (password.includes(pattern)) {
            return {
                isValid: false,
                error: 'Şifre yasaklı kelime veya pattern içeriyor'
            };
        }
    }

    return { isValid: true };
};

export const validatePasswordConfirmation = (
    password: string,
    confirmPassword: string
): ValidationResult => {
    if (!confirmPassword) {
        return {
            isValid: false,
            error: 'Şifre tekrarı gereklidir'
        };
    }

    if (password !== confirmPassword) {
        return {
            isValid: false,
            error: 'Şifreler eşleşmiyor'
        };
    }

    return { isValid: true };
};