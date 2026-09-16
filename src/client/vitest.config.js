import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { defineConfig } from 'vitest/config';
import fable from 'vite-plugin-fable';

const currentDir = path.dirname(fileURLToPath(import.meta.url));
// The same project and the same plugin the app is built with, so what the tests exercise is what
// ships. A Fable of their own, as a dotnet tool or a second project, is a Fable free to drift.
const fsproj = path.join(currentDir, 'fsharp/FantomasTools.fsproj');

export default defineConfig({
  plugins: [fable({ fsproj })],
  test: {
    include: ['fsharp/test/**/*.test.fs'],
    environment: 'node',
    // Compiling the client through Fable is nearly all of the run, and none of it changes between
    // runs unless the F# does.
    fsModuleCache: true,
    // A link the tool cannot read is reported by the tool before it falls back, so the tests that
    // feed it one print what the app prints. Worth seeing when a test fails, noise when it passes.
    silent: 'passed-only',
  },
});
