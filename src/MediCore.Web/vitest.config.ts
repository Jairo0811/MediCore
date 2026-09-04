import { defineConfig } from 'vitest/config';

export default defineConfig({
  test: {
    environment: 'jsdom',
    setupFiles: './src/test/setup.ts',
    restoreMocks: true,
    css: true,
    include: ['src/**/*.test.{ts,tsx}'],
  },
});
