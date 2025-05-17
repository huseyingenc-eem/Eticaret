import React, { Suspense } from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import { Windmill } from '@windmill/react-ui';

import App from './App.jsx';
import myTheme from './assets/theme/myTheme';

import './assets/css/custom.css';
import './assets/css/tailwind.css';
import './assets/css/tailwind.output.css';
import '@pathofdev/react-tag-input/build/index.css';

import ThemeSuspense from './components/theme/ThemeSuspense.jsx';
import { AdminProvider } from './contexts/AdminContext.jsx';
import { SidebarProvider } from './contexts/SidebarContext.jsx';

const root = ReactDOM.createRoot(document.getElementById('root'));

root.render(
    <React.StrictMode>
        <AdminProvider>
            <SidebarProvider>
                <BrowserRouter>
                    <Suspense fallback={<ThemeSuspense />}>
                        <Windmill usePreferences theme={myTheme}>
                            <App />
                        </Windmill>
                    </Suspense>
                </BrowserRouter>
            </SidebarProvider>
        </AdminProvider>
    </React.StrictMode>,
);
