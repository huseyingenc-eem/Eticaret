import { useState } from 'react';

export const useForm = <T extends Record<string, any>>(
    initialValues: T,
    validationRules?: Record<keyof T, (value: any) => string | null>
) => {
    const [values, setValues] = useState<T>(initialValues);
    const [errors, setErrors] = useState<Partial<Record<keyof T, string>>>({});
    const [touched, setTouched] = useState<Partial<Record<keyof T, boolean>>>({});

    const handleChange = (name: keyof T, value: any) => {
        setValues(prev => ({ ...prev, [name]: value }));

        // Validation
        if (validationRules && validationRules[name]) {
            const error = validationRules[name](value);
            setErrors(prev => ({ ...prev, [name]: error || undefined }));
        }
    };

    const handleBlur = (name: keyof T) => {
        setTouched(prev => ({ ...prev, [name]: true }));
    };

    const validateAll = (): boolean => {
        if (!validationRules) return true;

        const newErrors: Partial<Record<keyof T, string>> = {};
        let hasErrors = false;

        Object.keys(validationRules).forEach(key => {
            const error = validationRules[key as keyof T](values[key as keyof T]);
            if (error) {
                newErrors[key as keyof T] = error;
                hasErrors = true;
            }
        });

        setErrors(newErrors);
        setTouched(Object.keys(values).reduce((acc, key) => ({
            ...acc,
            [key]: true
        }), {}));

        return !hasErrors;
    };

    const reset = () => {
        setValues(initialValues);
        setErrors({});
        setTouched({});
    };

    return {
        values,
        errors,
        touched,
        handleChange,
        handleBlur,
        validateAll,
        reset,
        isValid: Object.keys(errors).length === 0
    };
};