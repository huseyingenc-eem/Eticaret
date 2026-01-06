// vite.config.ts
import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import { resolve } from 'path'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')

  const frontendPort = parseInt(env.VITE_FRONTEND_PORT) || 5173
  const backendPort = parseInt(env.VITE_BACKEND_PORT) || 9000
  const backendUrl = env.VITE_BACKEND_URL || 'http://localhost'

  return {
    plugins: [
      react({
        jsxRuntime: 'automatic',
        babel: {
          plugins: []
        }
      }),
      tailwindcss()
    ],
    resolve: {
      alias: {
        '@': resolve(__dirname, './src')
      }
    },
    server: {
      port: frontendPort,
      strictPort: true,
      host: true,
      cors: true,
      proxy: {
        '/api': {
          target: `${backendUrl}:${backendPort}`,
          changeOrigin: true,
          secure: false,
        }
      }
    },
    proxy: {
      '/api': {
        target: `${env.VITE_BACKEND_URL}:${env.VITE_BACKEND_PORT}`,
        changeOrigin: true,
        secure: false,
      }
    },
    preview: {
      port: frontendPort,
      strictPort: true,
    }
  }
})