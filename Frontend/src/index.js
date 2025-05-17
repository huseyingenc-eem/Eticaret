import React, { Suspense } from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import { Windmill } from '@windmill/react-ui';

import App from './App.jsx';
import myTheme from './assets/theme/myTheme';

import './assets/css/custom.css';
import './assets/css/tailwind.css';
import './assets/css/tailwind.output.css';
import '@pathofdev/react-tag-input/build/index.css';

import ThemeSuspense from './components/theme/ThemeSuspense';
import { AdminProvider } from './contexts/AdminContext';
import { SidebarProvider } from './contexts/SidebarContext';

ReactDOM.render(
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
    document.getElementById('root')
);
