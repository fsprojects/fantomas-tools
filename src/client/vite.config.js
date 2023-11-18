import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react({ jsxRuntime: 'classic' })],
  server: {
    port: 9060,
    watch: {
      ignored: [
        '**/*.fs',
        '**/*.fsi', // Don't watch F# files
      ],
    },
  },
  build: {
    outDir: 'build',
  },
});
