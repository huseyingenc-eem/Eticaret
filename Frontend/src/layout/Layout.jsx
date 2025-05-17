import React, { useContext, Suspense, useEffect, lazy } from 'react';
import { Routes, Route, Navigate, useLocation } from 'react-router-dom';

import Main from './Main.jsx';
import routes from '../routes/index.js';
import Header from '../components/header/Header.jsx';
import Sidebar from '../components/sidebar/Sidebar.jsx';
import { SidebarContext } from '../contexts/SidebarContext.jsx';
import ThemeSuspense from '../components/theme/ThemeSuspense.jsx';

const Page404 = lazy(() => import('../pages/404.jsx'));

const Layout = () => {
  const { isSidebarOpen, closeSidebar } = useContext(SidebarContext);
  const location = useLocation();

  useEffect(() => {
    closeSidebar();
  }, [location]);

  return (
      <div
          className={`flex h-screen bg-gray-50 dark:bg-gray-900 ${
              isSidebarOpen ? 'overflow-hidden' : ''
          }`}
      >
        <Sidebar />

        <div className="flex flex-col flex-1 w-full">
          <Header />
          <Main>
            <Suspense fallback={<ThemeSuspense />}>
              <Routes>
                {routes.map((route, i) => {
                  const Component = route.component;
                  return (
                      <Route key={i} path={route.path} element={<Component />} />
                  );
                })}
                <Route path="/" element={<Navigate to="/dashboard" replace />} />
                {/*<Route path="*" element={<Page404 />} />*/}
              </Routes>
            </Suspense>
          </Main>
        </div>
      </div>
  );
};

export default Layout;
