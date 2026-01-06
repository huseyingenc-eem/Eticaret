// src/pages/AuthPages.tsx
import { useState } from "react";
import { useLocation } from "react-router-dom";
import { LoginForm, RegisterForm } from "@/components/forms/auth";
import { motion, AnimatePresence } from "framer-motion";
import { Button } from "@/components/ui";


export default function AuthPages() {
    const [isLogin, setIsLogin] = useState(true);
    const location = useLocation();
    const fromPath = (location.state as any)?.from?.pathname as string | undefined;

    return (
        <div className="flex min-h-screen items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 p-4">
            <div className="w-full max-w-md">
                <div className="overflow-hidden rounded-2xl bg-white shadow-sm">
                    {/* Header */}
                    <div className="bg-gradient-to-r from-blue-600 to-indigo-600 p-6 text-center">
                        <h1 className="mb-1 text-2xl font-bold text-white">E-Ticaret Platformu</h1>
                        <p className="text-blue-100">
                            {isLogin ? "Hesabınıza giriş yapın" : "Yeni hesap oluşturun"}
                        </p>

                        {fromPath && (
                            <div className="mt-2 text-xs text-blue-100/90">
                                Giriş sonrası otomatik yönlendirme:{" "}
                                <span className="underline underline-offset-2">{fromPath}</span>
                            </div>
                        )}
                    </div>

                    {/* Tabs (Button bileşeni ile) */}
                    <div role="tablist" aria-label="Kimlik doğrulama sekmeleri" className="flex bg-gray-50">
                        <Button
                            type="button"
                            size="sm"
                            color={isLogin ? "primary" : "neutral"}
                            variant={isLogin ? "filled" : "ghost"}
                            className="flex-1 rounded-none py-3"
                            aria-selected={isLogin}
                            onClick={() => setIsLogin(true)}
                        >
                            Giriş Yap
                        </Button>
                        <Button
                            type="button"
                            size="sm"
                            color={!isLogin ? "primary" : "neutral"}
                            variant={!isLogin ? "filled" : "ghost"}
                            className="flex-1 rounded-none py-3"
                            aria-selected={!isLogin}
                            onClick={() => setIsLogin(false)}
                        >
                            Kayıt Ol
                        </Button>
                    </div>

                    {/* Body */}
                    <div className="space-y-4 p-6">
                        {/* Social giriş */}

                        {/* Forms (animated) */}
                        <AnimatePresence mode="wait">
                            <motion.div
                                key={isLogin ? "login" : "register"}
                                initial={{ opacity: 0, x: isLogin ? -20 : 20 }}
                                animate={{ opacity: 1, x: 0 }}
                                exit={{ opacity: 0, x: isLogin ? 20 : -20 }}
                                transition={{ duration: 0.25 }}
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

            </div>
        </div>
    );
}
