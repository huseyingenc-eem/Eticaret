import {useState, type FormEvent} from 'react';
import {validateRegisterForm, getPasswordStrength, type RegisterFormData} from '@/utils/validation';
import {Input, Button, Checkbox, Card, CardContent, CardFooter} from '@/components/ui';

interface RegisterFormProps {
    onSwitchToLogin: () => void;
}

const RegisterForm = ({onSwitchToLogin}: RegisterFormProps) => {
    const [formData, setFormData] = useState<RegisterFormData>({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: '',
        acceptTerms: false
    });
    const [errors, setErrors] = useState<Record<string, string>>({});
    const [loading, setLoading] = useState(false);
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);

    const passwordStrength = formData.password ? getPasswordStrength(formData.password) : null;

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();

        const validationResult = validateRegisterForm(formData);
        if (!validationResult.isValid) {
            setErrors(validationResult.errors);
            return;
        }

        setErrors({});
        setLoading(true);

        try {
            setTimeout(() => {
                setLoading(false);
                alert('Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz.');
                onSwitchToLogin();
            }, 1000);
        } catch (error) {
            setLoading(false);
            console.error('Register error:', error);
            setErrors({general: 'Kayıt olurken bir hata oluştu'});
        }
    };

    const handleInputChange = (field: keyof RegisterFormData, value: string | boolean) => {
        setFormData(prev => ({...prev, [field]: value}));
        if (errors[field]) setErrors(prev => ({...prev, [field]: ''}));
    };

    const getStrengthColor = (level: string) => {
        switch (level) {
            case 'Çok Zayıf':
                return 'bg-red-500';
            case 'Zayıf':
                return 'bg-orange-500';
            case 'Orta':
                return 'bg-yellow-500';
            case 'Güçlü':
                return 'bg-blue-500';
            case 'Çok Güçlü':
                return 'bg-green-500';
            default:
                return 'bg-gray-300';
        }
    };

    const getStrengthWidth = (score: number) => `${(score / 8) * 100}%`;

    const EyeIcon = () => (
        <svg className="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                  d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
        </svg>
    );

    const EyeOffIcon = () => (
        <svg className="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                  d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L3 3m6.878 6.878L21 21"/>
        </svg>
    );

    return (
        <Card variant="default" padding="none">
            <form onSubmit={handleSubmit}>
                <CardContent className="space-y-4">
                    {errors.general && (
                        <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600">
                            {errors.general}
                        </div>
                    )}

                    {/* Name Fields */}
                    <div className="grid grid-cols-2 gap-3">
                        <Input
                            type="text"
                            label="Ad"
                            placeholder="Adınız"
                            value={formData.firstName}
                            onChange={(e) => handleInputChange('firstName', e.target.value)}
                            error={errors.firstName}
                            size="md"
                        />
                        <Input
                            type="text"
                            label="Soyad"
                            placeholder="Soyadınız"
                            value={formData.lastName}
                            onChange={(e) => handleInputChange('lastName', e.target.value)}
                            error={errors.lastName}
                            size="md"
                        />
                    </div>

                    {/* Email */}
                    <Input
                        type="email"
                        label="E-posta"
                        placeholder="ornek@email.com"
                        value={formData.email}
                        onChange={(e) => handleInputChange('email', e.target.value)}
                        error={errors.email}
                    />

                    {/* Password */}
                    <div>
                        <Input
                            type={showPassword ? 'text' : 'password'}
                            label="Şifre"
                            placeholder="En az 8 karakter"
                            value={formData.password}
                            onChange={(e) => handleInputChange('password', e.target.value)}
                            error={errors.password}
                            rightIcon={
                                <button
                                    type="button"
                                    onClick={() => setShowPassword(!showPassword)}
                                    className="transition-colors hover:text-gray-600"
                                >
                                    {showPassword ? <EyeOffIcon/> : <EyeIcon/>}
                                </button>
                            }
                        />

                        {formData.password && passwordStrength && (
                            <div className="mt-2">
                                <div className="mb-1 flex items-center justify-between text-xs">
                                    <span className="text-gray-600">Şifre Gücü:</span>
                                    <span
                                        className={`font-medium ${
                                            passwordStrength.level === 'Çok Güçlü' ? 'text-green-600'
                                                : passwordStrength.level === 'Güçlü' ? 'text-blue-600'
                                                    : passwordStrength.level === 'Orta' ? 'text-yellow-600'
                                                        : 'text-red-600'
                                        }`}
                                    >
                    {passwordStrength.level}
                  </span>
                                </div>
                                <div className="h-2 w-full rounded-full bg-gray-200">
                                    <div
                                        className={`h-2 rounded-full transition-all duration-300 ${getStrengthColor(passwordStrength.level)}`}
                                        style={{width: getStrengthWidth(passwordStrength.score)}}
                                    />
                                </div>
                                {passwordStrength.suggestions.length > 0 && (
                                    <ul className="mt-1 text-xs text-gray-600">
                                        {passwordStrength.suggestions.map((s, i) => (
                                            <li key={i}>• {s}</li>
                                        ))}
                                    </ul>
                                )}
                            </div>
                        )}
                    </div>

                    {/* Confirm Password */}
                    <Input
                        type={showConfirmPassword ? 'text' : 'password'}
                        label="Şifre Tekrar"
                        placeholder="Şifrenizi tekrar girin"
                        value={formData.confirmPassword}
                        onChange={(e) => handleInputChange('confirmPassword', e.target.value)}
                        error={errors.confirmPassword}
                        rightIcon={
                            <button
                                type="button"
                                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                className="transition-colors hover:text-gray-600"
                            >
                                {showConfirmPassword ? <EyeOffIcon/> : <EyeIcon/>}
                            </button>
                        }
                    />

                    {/* Terms */}
                    <Checkbox
                        checked={formData.acceptTerms}
                        onChange={(e) => handleInputChange('acceptTerms', e.target.checked)}
                        label={
                            <span className="text-sm text-gray-700">
                <a href="#" className="text-blue-600 hover:text-blue-800">Kullanım Koşulları</a>{' '}
                                ve{' '}
                                <a href="#" className="text-blue-600 hover:text-blue-800">Gizlilik Politikası</a>
                'nı kabul ediyorum.
              </span>
                        }
                        error={errors.acceptTerms}
                    />

                    {/* ✅ Yeni Button API */}
                    <Button
                        type="submit"
                        color="primary"
                        size="md"
                        className="w-full"
                        loading={loading}
                    >
                        Hesap Oluştur
                    </Button>
                </CardContent>

                <CardFooter>
                    <div className="w-full border-t border-gray-200 pt-4 text-center">
                        <p className="text-sm text-gray-600">
                            Zaten hesabınız var mı?{' '}
                            <button
                                type="button"
                                onClick={onSwitchToLogin}
                                className="font-medium text-blue-600 hover:text-blue-800"
                            >
                                Giriş yapın
                            </button>
                        </p>
                    </div>
                </CardFooter>
            </form>
        </Card>
    );
};

export default RegisterForm;
