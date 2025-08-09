// src/utils/validation/form/formValidator.ts
import type { FormValidationResult } from '../types';
import { validateEmail } from '../email';
import { validatePassword, validatePasswordConfirmation } from '../password';

export interface LoginFormData {
    email: string;
    password: string;
}

export interface RegisterFormData {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
    acceptTerms: boolean;
}

export const validateLoginForm = (data: LoginFormData): FormValidationResult => {
    const errors: Record<string, string> = {};

    const emailResult = validateEmail(data.email);
    if (!emailResult.isValid && emailResult.error) {
        errors.email = emailResult.error;
    }

    if (!data.password) {
        errors.password = 'Şifre gereklidir';
    }

    return {
        isValid: Object.keys(errors).length === 0,
        errors
    };
};

export const validateRegisterForm = (data: RegisterFormData): FormValidationResult => {
    const errors: Record<string, string> = {};

    if (!data.firstName.trim()) {
        errors.firstName = 'Ad gereklidir';
    } else if (data.firstName.trim().length < 2) {
        errors.firstName = 'Ad en az 2 karakter olmalıdır';
    } else if (!/^[a-zA-ZçğıöşüÇĞIÖŞÜ\s]+$/.test(data.firstName)) {
        errors.firstName = 'Ad sadece harf içerebilir';
    }

    if (!data.lastName.trim()) {
        errors.lastName = 'Soyad gereklidir';
    } else if (data.lastName.trim().length < 2) {
        errors.lastName = 'Soyad en az 2 karakter olmalıdır';
    } else if (!/^[a-zA-ZçğıöşüÇĞIÖŞÜ\s]+$/.test(data.lastName)) {
        errors.lastName = 'Soyad sadece harf içerebilir';
    }

    const emailResult = validateEmail(data.email);
    if (!emailResult.isValid && emailResult.error) {
        errors.email = emailResult.error;
    }

    const passwordResult = validatePassword(data.password);
    if (!passwordResult.isValid && passwordResult.error) {
        errors.password = passwordResult.error;
    }

    const confirmPasswordResult = validatePasswordConfirmation(data.password, data.confirmPassword);
    if (!confirmPasswordResult.isValid && confirmPasswordResult.error) {
        errors.confirmPassword = confirmPasswordResult.error;
    }

    if (!data.acceptTerms) {
        errors.acceptTerms = 'Kullanım koşullarını kabul etmelisiniz';
    }

    return {
        isValid: Object.keys(errors).length === 0,
        errors
    };
};