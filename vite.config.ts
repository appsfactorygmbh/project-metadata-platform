/// <reference types="vitest" />
import { fileURLToPath } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'
import AutoImport from 'unplugin-auto-import/vite'

const baseSrc = fileURLToPath(new URL('./src', import.meta.url))

// https://vitejs.dev/config/
export default defineConfig({
  resolve: {
    alias: [
      {
        find: 'dayjs',
        replacement: 'dayjs/esm',
      },
      {
        find: /^dayjs\/locale/,
        replacement: 'dayjs/esm/locale',
      },
      {
        find: /^dayjs\/plugin/,
        replacement: 'dayjs/esm/plugin',
      },
      {
        find: /^ant-design-vue\/es$/,
        replacement: 'ant-design-vue/es',
      },
      {
        find: /^ant-design-vue\/dist$/,
        replacement: 'ant-design-vue/dist',
      },
      {
        find: /^ant-design-vue\/lib$/,
        replacement: 'ant-design-vue/es',
      },
      {
        find: /^ant-design-vue$/,
        replacement: 'ant-design-vue/es',
      },
      {
        find: 'lodash',
        replacement: 'lodash-es',
      },
      {
        find: '@',
        replacement: baseSrc,
      },
    ],
  },
  plugins: [
    vue(), 
    vueJsx(),
    AutoImport({
      imports: [
        'vue',
        'vue-router',
      ],
      dts: 'types/auto-imports.d.ts',
    }),
  ],
  test: {
    globals: true,
    environment: 'jsdom',
    exclude: ['node_modules', 'dist', 'coverage', 'lib', '*.d.ts'],
    coverage: {
      enabled: true,
      reporter: ['text', 'html'],
      exclude: [
        'node_modules',
        'dist',
        'coverage',
        'lib',
        '*.d.ts',
      ],
    },

    setupFiles: './tests/setup.ts',
    reporters: ['default', 'html'],
  },
})
