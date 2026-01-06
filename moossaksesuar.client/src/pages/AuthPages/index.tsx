import { useState } from 'react';
import LoginForm from '@/components/forms/auth/LoginForm';
import RegisterForm from '@/components/forms/auth/RegisterForm';
import { motion, AnimatePresence } from 'framer-motion';

const AuthPage = () => {
    const [isLogin, setIsLogin] = useState(true);

    return (
        <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
            <div className="max-w-md w-full">
                <div className="bg-white rounded-2xl  overflow-hidden">
                    <div className="bg-gradient-to-r from-blue-600 to-indigo-600 p-6 text-center">
                        <h1 className="text-2xl font-bold text-white mb-2">
                            E-Ticaret Platformu
                        </h1>
                        <p className="text-blue-100">
                            {isLogin ? 'Hesabınıza giriş yapın' : 'Yeni hesap oluşturun'}
                        </p>
                    </div>

                    <div className="flex bg-gray-50">
                        <button
                            onClick={() => setIsLogin(true)}
                            className={`flex-1 py-3 px-4 text-sm font-medium transition-all duration-300 ${
                                isLogin
                                    ? 'bg-white text-blue-600 border-b-2 border-blue-600'
                                    : 'text-gray-600 hover:bg-gray-100'
                            }`}
                        >
                            Giriş Yap
                        </button>
                        <button
                            onClick={() => setIsLogin(false)}
                            className={`flex-1 py-3 px-4 text-sm font-medium transition-all duration-300 ${
                                !isLogin
                                    ? 'bg-white text-blue-600 border-b-2 border-blue-600'
                                    : 'text-gray-600 hover:bg-gray-100'
                            }`}
                        >
                            Kayıt Ol
                        </button>
                    </div>

                    <div className="p-6 relative min-h-[520px] sm:min-h-[500px]">
                        <AnimatePresence mode="wait">
                            <motion.div
                                key={isLogin ? 'login' : 'register'}
                                initial={{ opacity: 0, x: isLogin ? -20 : 20 }}
                                animate={{ opacity: 1, x: 0 }}
                                exit={{ opacity: 0, x: isLogin ? 20 : -20 }}
                                transition={{ duration: 0.25 }}
                                className="absolute top-6 left-6 right-6"
                            >
                                {isLogin ? (
                                    <LoginForm onSwitchToRegister={() => setIsLogin(false)} />
                                ) : (
                                    <RegisterForm onSwitchToLogin={() => setIsLogin(true)} />
                                )}
                            </motion.div>
                        </AnimatePresence>
                    </div>
                </div>

                <div className="text-center mt-6 text-sm text-gray-600">
                    <p>© 2025 E-Ticaret. Tüm hakları saklıdır.</p>
                </div>
            </div>
        </div>
    );
};

export default AuthPage;