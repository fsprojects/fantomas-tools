import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fable from 'vite-plugin-fable';

const currentDir = path.dirname(fileURLToPath(import.meta.url));
const fsproj = path.join(currentDir, 'fsharp/FantomasTools.fsproj');

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
  preview: {
    port: 9060,
  },
});
