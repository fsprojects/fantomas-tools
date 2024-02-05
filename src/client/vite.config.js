import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fable from 'vite-plugin-fable';

const fsproj = await import.meta.resolve('./fsharp/FantomasTools.fsproj');

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react({ jsxRuntime: 'classic' }), fable({ fsproj })],
  server: {
    port: 9060,
  },
  build: {
    outDir: 'build',
  },
  base: '/fantomas-tools/',
});
