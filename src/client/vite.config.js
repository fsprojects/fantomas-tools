import { defineConfig } from 'vite'
import reactRefresh from '@vitejs/plugin-react-refresh'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [reactRefresh()],
  server: {
    port: 9060,
    hmr: {
      port: 443
    }
  },
  build: {
    outDir: 'build'
  }
})
