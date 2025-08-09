import type { ValidationResult } from '../types';

export const validateTCKN = (tckn: string): ValidationResult => {
    if (!tckn || tckn.length !== 11) {
        return {
            isValid: false,
            error: 'TC Kimlik Numarası 11 haneli olmalıdır'
        };
    }

    if (!/^\d+$/.test(tckn)) {
        return {
            isValid: false,
            error: 'TC Kimlik Numarası sadece rakam içerebilir'
        };
    }

    if (tckn[0] === '0') {
        return {
            isValid: false,
            error: 'TC Kimlik Numarası 0 ile başlayamaz'
        };
    }

    const digits = tckn.split('').map(Number);
    const oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
    const evenSum = digits[1] + digits[3] + digits[5] + digits[7];

    const checkDigit10 = ((oddSum * 7) - evenSum) % 10;
    const checkDigit11 = (oddSum + evenSum + digits[9]) % 10;

    if (digits[9] !== checkDigit10 || digits[10] !== checkDigit11) {
        return {
            isValid: false,
            error: 'Geçersiz TC Kimlik Numarası'
        };
    }

    return { isValid: true };
};