// src/components/forms/auth/LoginForm.tsx
import { useState, type FormEvent } from 'react';
import { validateLoginForm, type LoginFormData } from '@/utils/validation';
import { Input, Button, Card, CardContent, CardFooter } from '@/components/ui';
import { useAuth } from '@/hooks';
import { Eye, EyeOff } from 'lucide-react';
import SocialButtons from './SocailButtons';

type AppError =
    | { kind: 'validation'; status: number; errors: Record<string, string[]>; title?: string; code?: string }
    | { kind: 'problem'; status: number; title?: string; message?: string; code?: string }
    | { kind: 'network'; message: string };

interface LoginFormProps {
    onSwitchToRegister: () => void;
}

const LoginForm = ({ onSwitchToRegister }: LoginFormProps) => {
    const { login, loading } = useAuth();

    const [formData, setFormData] = useState<LoginFormData>({ email: '', password: '' });
    const [errors, setErrors] = useState<Record<string, string>>({});
    const [showPassword, setShowPassword] = useState(false);

    const applyServerValidation = (err: AppError) => {
        if (err.kind !== 'validation') return false;

        const mapped: Record<string, string> = {};
        for (const [serverField, messages] of Object.entries(err.errors)) {
            const localField =
                serverField in { Email: 1, Password: 1 }
                    ? serverField.toLowerCase()
                    : serverField.charAt(0).toLowerCase() + serverField.slice(1);

            mapped[localField] = messages.join(' ');
        }
        setErrors(prev => ({ ...prev, ...mapped }));
        return true;
    };

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();

        const { isValid, errors: vErrors } = validateLoginForm(formData);
        if (!isValid) {
            setErrors(vErrors);
            return;
        }

        setErrors({});
        try {
            await login(formData);
        } catch (error) {
            const handled = applyServerValidation(error as AppError);
            if (!handled) {
                //
            }
        }
    };

    const handleInputChange = (field: keyof LoginFormData, value: string) => {
        setFormData(prev => ({ ...prev, [field]: value }));
        if (errors[field]) setErrors(prev => ({ ...prev, [field]: '' }));
    };

    const fillTestData = () => {
        setFormData({ email: 'huseyin@gmail.com', password: 'test123' });
        setErrors({});
    };

    return (
        <Card variant="default" padding="none" >
            <form onSubmit={handleSubmit} noValidate>
                <CardContent className="space-y-4">


                    {/* Divider */}


                    {errors.general && (
                        <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600">
                            {errors.general}
                        </div>
                    )}

                    <Input
                        type="email"
                        label="E-posta"
                        placeholder="ornek@email.com"
                        value={formData.email}
                        onChange={(e) => handleInputChange('email', e.target.value)}
                        error={errors.email}
                        name="password"
                        autoComplete={showPassword ? 'off' : 'current-password'}
                        aria-invalid={Boolean(errors.password) || undefined}
                    />

                    <Input
                        type={showPassword ? 'text' : 'password'}
                        label="Şifre"
                        placeholder="Şifrenizi girin"
                        value={formData.password}
                        onChange={(e) => handleInputChange('password', e.target.value)}
                        error={errors.password}
                        rightIcon={
                            <button
                                type="button"
                                aria-label={showPassword ? 'Şifreyi gizle' : 'Şifreyi göster'}
                                onClick={() => setShowPassword((s) => !s)}
                                className="transition-colors hover:text-gray-600"
                            >
                                {showPassword ? <EyeOff className="h-5 w-5 shrink-0" /> : <Eye className="h-5 w-5 shrink-0" />}
                            </button>
                        }
                        name="password"
                        autoComplete={showPassword ? 'off' : 'current-password'}
                        aria-invalid={Boolean(errors.password) || undefined}
                    />

                    <div className="text-right">
                        <button type="button" className="text-sm text-blue-600 transition-colors hover:text-blue-800">
                            Şifremi unuttum
                        </button>
                    </div>

                    {/* ✅ Yeni Button API */}
                    <Button
                        type="submit"
                        color="primary"
                        size="md"
                        className="w-full"
                        loading={loading}
                    >
                        Giriş Yap
                    </Button>

                    {import.meta.env.DEV && (
                        <Button
                            type="button"
                            variant="soft"
                            color="neutral"
                            size="sm"
                            className="w-full"
                            onClick={fillTestData}
                        >
                            Test Verilerini Doldur
                        </Button>
                    )}
                    <SocialButtons
                        showDivider={true}
                    />
                </CardContent>

                <CardFooter>
                    <div className="w-full border-t border-gray-200 pt-4 text-center">
                        <p className="text-sm text-gray-600">
                            Hesabınız yok mu?{' '}
                            <button
                                type="button"
                                onClick={onSwitchToRegister}
                                className="font-medium text-blue-600 hover:text-blue-800"
                            >
                                Kayıt olun
                            </button>
                        </p>
                    </div>
                </CardFooter>
            </form>
        </Card>
    );
};

export default LoginForm;
