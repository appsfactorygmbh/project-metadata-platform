/// <reference types="vitest/globals" />
/// <reference types="vite/client" />
import { fileURLToPath } from 'node:url';
import { defineConfig, ConfigEnv, UserConfig } from 'vite';
import { configDefaults } from 'vitest/config';
import vue from '@vitejs/plugin-vue';
import vueJsx from '@vitejs/plugin-vue-jsx';
import VueDevTools from 'vite-plugin-vue-devtools';
import AutoImport from 'unplugin-auto-import/vite';
import Components from 'unplugin-vue-components/vite';
import AntdvResolver from 'antdv-component-resolver';

const baseSrc = fileURLToPath(new URL('./src', import.meta.url));

// https://vitejs.dev/config/
// eslint-disable-next-line @typescript-eslint/no-unused-vars
export default defineConfig(({ mode }: ConfigEnv): UserConfig => {
  return {
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
      VueDevTools(),
      AutoImport({
        imports: ['vue', 'vue-router', 'vitest'],
        dts: 'types/auto-imports.d.ts',
      }),
      Components({
        resolvers: [AntdvResolver()],
        dts: 'types/components.d.ts',
        dirs: ['src/components'],
      }),
    ],
    build: {
      chunkSizeWarningLimit: 4096,
      outDir: 'dist',
      rollupOptions: {
        output: {
          manualChunks: {
            vue: ['vue', 'vue-router'],
            antd: ['ant-design-vue', '@ant-design/icons-vue', 'dayjs'],
          },
        },
      },
    },
    server: {
      port: 5173,
    },
    test: {
      globals: true,
      environment: 'jsdom',
      exclude: [...configDefaults.exclude, '*.d.ts'],
      root: fileURLToPath(new URL('./', import.meta.url)),
      coverage: {
        enabled: true,
        reporter: ['text', 'html', 'cobertura', 'lcov'],
        exclude: ['node_modules', 'dist', 'coverage', 'html', 'lib', '*.d.ts'],
      },
      setupFiles: './tests/setup.ts',
      reporters: ['default', 'html'],
      outputFile: {
        junit: './junit.xml',
      },
    },
  };
});
