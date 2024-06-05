// @ts-check

import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';
import pluginVue from 'eslint-plugin-vue';
import eslintConfigPrettier from 'eslint-config-prettier';

export default tseslint.config(
  eslint.configs.recommended,
  ...tseslint.configs.recommended,
  // @ts-expect-error mismatched types
  ...pluginVue.configs['flat/recommended'],
  {
    name: 'linter-config',
    files: ['eslint.config.js', 'vite.config.ts'],
    languageOptions: {
      parserOptions: {
        parser: tseslint.parser,
        project: './tsconfig.eslint.json',
        sourceType: 'module',
      },
    },
  },
  {
    name: 'custom-config',
    files: ['src/**/*.{ts,tsx,vue}'],
    plugins: {
      'typescript-eslint': tseslint.plugin,
    },
    languageOptions: {
      parserOptions: {
        parser: tseslint.parser,
        extraFileExtensions: ['.vue'],
        project: './tsconfig.json',
        sourceType: 'module',
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
  // keep as last item to override conflicting rules
  eslintConfigPrettier,
);
