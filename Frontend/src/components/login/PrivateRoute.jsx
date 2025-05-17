// Dosya Adı: Frontend/src/components/login/PrivateRoute.jsx
import React, { useContext } from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { AdminContext } from '../../contexts/AdminContext.jsx'; // AdminContext.jsx olarak güncellendiğini varsayıyoruz

/**
 * PrivateRoute bileşeni, kullanıcının kimliği doğrulanmışsa alt bileşenleri (children)
 * veya bir Outlet'i render eder. Aksi takdirde kullanıcıyı giriş sayfasına yönlendirir.
 *
 * Kullanım şekli (React Router DOM v6'da):
 * <Routes>
 * <Route path="/login" element={<LoginPage />} />
 * <Route element={<PrivateRoute />}>
 * <Route path="/dashboard" element={<DashboardPage />} />
 * <Route path="/profile" element={<ProfilePage />} />
 * </Route>
 * </Routes>
 *
 * Veya belirli bir bileşeni sarmalamak için:
 * <PrivateRoute>
 * <MyProtectedComponent />
 * </PrivateRoute>
 */
const PrivateRoute = ({ children }) => {
    const { state } = useContext(AdminContext);
    const location = useLocation();

    // adminInfo ve token varlığını kontrol edin.
    // Gerçek kimlik doğrulama mantığınız burada olmalı.
    // Örnek olarak state.isAuthenticated veya state.user?.token gibi bir kontrol olabilir.
    // Bu örnekte, eski yapıdaki adminInfo.token kontrolünü kullanıyoruz.
    const isAuthenticated = state.adminInfo && state.adminInfo.token;

    if (!isAuthenticated) {
        // Kullanıcı kimliği doğrulanmamışsa, giriş sayfasına yönlendir.
        // state={{ from: location }} kısmı, giriş yaptıktan sonra kullanıcının
        // geldiği sayfaya geri yönlendirilmesine olanak tanır.
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    // Kimlik doğrulanmışsa, ya doğrudan geçirilen children'ı ya da <Outlet />'i render et.
    // Eğer bu PrivateRoute'u <Route element={<PrivateRoute />}> ... </Route> şeklinde
    // bir layout route olarak kullanıyorsanız, Outlet iç içe route'ları render edecektir.
    // Eğer <PrivateRoute><SomeComponent /></PrivateRoute> şeklinde kullanıyorsanız,
    // children render edilecektir.
    return children ? children : <Outlet />;
};

export default PrivateRoute;
