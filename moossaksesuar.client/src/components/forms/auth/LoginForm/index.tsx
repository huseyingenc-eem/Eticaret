import { useState, type FormEvent } from 'react';
import { validateLoginForm, type LoginFormData } from '@/utils/validation';
import { Input, Button, Card, CardContent, CardFooter } from '@/components/ui';
import { useAuth } from '@/hooks/useAuth';

interface LoginFormProps {
    onSwitchToRegister: () => void;
}

const LoginForm = ({ onSwitchToRegister }: LoginFormProps) => {
    const { login, loading } = useAuth();

    const [formData, setFormData] = useState<LoginFormData>({
        email: '',
        password: ''
    });
    const [errors, setErrors] = useState<Record<string, string>>({});
    const [showPassword, setShowPassword] = useState(false);

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();

        const validationResult = validateLoginForm(formData);

        if (!validationResult.isValid) {
            setErrors(validationResult.errors);
            return;
        }

        setErrors({});

        try {
            await login(formData);
            // Başarılı login sonrası yönlendirme
            window.location.href = '/';
        } catch (error) {
            // Hata zaten baseApi errorHandler'ında yakalandı ve toast gösterildi
            // Burada sadece form-specific error handling yapabilirsin
            console.error('Login failed:', error);
        }
    };

    const handleInputChange = (field: keyof LoginFormData, value: string) => {
        setFormData(prev => ({ ...prev, [field]: value }));
        if (errors[field]) {
            setErrors(prev => ({ ...prev, [field]: '' }));
        }
    };

    // Test için hızlı doldur fonksiyonu
    const fillTestData = () => {
        setFormData({
            email: 'huseyin@gmail.com',
            password: 'test123'
        });
        setErrors({});
    };

    const EyeIcon = () => (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
        </svg>
    );

    const EyeOffIcon = () => (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L3 3m6.878 6.878L21 21" />
        </svg>
    );

    return (
        <Card variant="default" padding="none">
            <form onSubmit={handleSubmit}>
                <CardContent className="space-y-4">
                    {/* General Error */}
                    {errors.general && (
                        <div className="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-lg text-sm">
                            {errors.general}
                        </div>
                    )}

                    {/* Email Input */}
                    <Input
                        type="email"
                        label="E-posta"
                        placeholder="ornek@email.com"
                        value={formData.email}
                        onChange={(e) => handleInputChange('email', e.target.value)}
                        error={errors.email}
                    />

                    {/* Password Input */}
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
                                onClick={() => setShowPassword(!showPassword)}
                                className="hover:text-gray-600 transition-colors"
                            >
                                {showPassword ? <EyeOffIcon /> : <EyeIcon />}
                            </button>
                        }
                    />

                    {/* Forgot Password */}
                    <div className="text-right">
                        <button
                            type="button"
                            className="text-sm text-blue-600 hover:text-blue-800 transition-colors"
                        >
                            Şifremi unuttum
                        </button>
                    </div>

                    {/* Submit Button */}
                    <Button
                        type="submit"
                        variant="primary"
                        size="md"
                        fullWidth
                        loading={loading}
                    >
                        Giriş Yap
                    </Button>

                    {/* Development Test Button */}
                    {import.meta.env.DEV && (
                        <Button
                            type="button"
                            variant="secondary"
                            size="sm"
                            fullWidth
                            onClick={fillTestData}
                        >
                            Test Verilerini Doldur
                        </Button>
                    )}
                </CardContent>

                <CardFooter>
                    <div className="text-center w-full pt-4 border-t border-gray-200">
                        <p className="text-sm text-gray-600">
                            Hesabınız yok mu?{' '}
                            <button
                                type="button"
                                onClick={onSwitchToRegister}
                                className="text-blue-600 hover:text-blue-800 font-medium"
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