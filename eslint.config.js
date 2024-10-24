// @ts-check

import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';
import pluginVue from 'eslint-plugin-vue';
import eslintConfigPrettier from 'eslint-config-prettier';

import path from 'path';
import { fileURLToPath } from 'url';
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

export default tseslint.config(
  eslint.configs.recommended,
  ...tseslint.configs.recommended,
  ...pluginVue.configs['flat/recommended'],
  {
    ignores: [
      '.yarn',
      '.vscode',
      '.git',
      'coverage',
      'dist',
      'html',
      'node_modules',
      'types',
    ],
  },
  {
    name: 'linter-config',
    files: ['eslint.config.js', 'vite.config.ts'],
    languageOptions: {
      parserOptions: {
        parser: tseslint.parser,
        project: path.resolve(__dirname, './tsconfig.node.json'),
        tsconfigRootDir: __dirname,
        sourceType: 'module',
      },
    },
  },
  {
    name: 'custom-config',
    files: ['src/**/*.{ts,tsx,vue}'],
    ignores: ['src/**/__tests__/**/*.{ts,tsx}'],
    plugins: {
      'typescript-eslint': tseslint.plugin,
    },
    languageOptions: {
      parserOptions: {
        parser: tseslint.parser,
        extraFileExtensions: ['.vue'],
        project: path.resolve(__dirname, './tsconfig.app.json'),
        tsconfigRootDir: __dirname,
        sourceType: 'module',
        ecmaFeatures: {
          modules: true,
          jsx: true,
        },
      },
      // to support unplugin-auto-import
      globals: {
        ref: 'readonly',
        computed: 'readonly',
        watch: 'readonly',
        watchEffect: 'readonly',
      },
    },
  },
  {
    name: 'vitest-config',
    files: ['src/**/__tests__/**/*.{ts,tsx}'],
    languageOptions: {
      parserOptions: {
        parser: tseslint.parser,
        project: path.resolve(__dirname, './tsconfig.vitest.json'),
        tsconfigRootDir: __dirname,
        sourceType: 'module',
      },
    },
  },
  // keep as last item to override conflicting rules
  eslintConfigPrettier,
);
