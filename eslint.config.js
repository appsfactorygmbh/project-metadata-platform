// @ts-check

import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';
// import tsParser from '@typescript-eslint/parser';
import tsPlugin from '@typescript-eslint/eslint-plugin';
import pluginVue from 'eslint-plugin-vue';
import * as vueParser from 'vue-eslint-parser';
import eslintConfigPrettier from 'eslint-config-prettier';
import unusedImports from 'eslint-plugin-unused-imports';
import globals from 'globals';

import path from 'path';
import { fileURLToPath } from 'url';
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const tsParser = tseslint.parser;

export default tseslint.config(
  eslint.configs.recommended,
  ...tseslint.configs.recommendedTypeChecked,
  ...pluginVue.configs['flat/base'],
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
      'src/api/generated/',
    ],
  },
  {
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.node,
      },
    },
  },
  {
    name: 'linter-config',
    files: ['eslint.config.js', 'vite.config.ts'],
    languageOptions: {
      parser: tsParser,
      ecmaVersion: 'latest',
      sourceType: 'module',

      parserOptions: {
        parser: tsParser,
        project: path.resolve(__dirname, './tsconfig.node.json'),
        tsconfigRootDir: __dirname,
        sourceType: 'module',
      },
    },
  },
  {
    name: 'vitest-config',
    files: ['src/**/__tests__/**/*.{ts,tsx}'],
    languageOptions: {
      parser: tsParser,
      ecmaVersion: 'latest',
      sourceType: 'module',

      parserOptions: {
        parser: tsParser,
        project: path.resolve(__dirname, './tsconfig.vitest.json'),
        tsconfigRootDir: __dirname,
        sourceType: 'module',
      },
    },
  },
  {
    name: 'typescript-config',
    files: ['src/**/*.{ts,tsx}'],
    ignores: [
      'src/**/__tests__/**/*.{ts,tsx}',
      'src/api/generated/**/*.{ts,tsx}',
    ],
    plugins: {
      'typescript-eslint': tseslint.plugin,
      'unused-imports': unusedImports,
    },
    languageOptions: {
      parser: tsParser,
      ecmaVersion: 'latest',
      sourceType: 'module',

      parserOptions: {
        parser: tsParser,
        // project: path.resolve(__dirname, './tsconfig.json'),
        tsconfigRootDir: __dirname,
        sourceType: 'module',
        ecmaFeatures: {
          modules: true,
          jsx: true,
        },
        projectService: {
          defaultProject: path.resolve(__dirname, './tsconfig.json'),
          loadTypeScriptPlugins: !!process.env.VSCODE_PID,
        },
      },
    },
    settings: {
      '@typescript-eslint/parser': ['.ts', '.tsx'],
      'import-x/extensions': ['.ts', '.tsx'],

      'import-x/resolver': {
        typescript: true,
        node: true,
      },

      'import/resolver': {
        alias: {
          map: [['@', 'src']],
        },
      },

      'import-x/docstyle': ['jsdoc', 'tomdoc'],
    },

    rules: {
      'no-unused-vars': 'off',
      'sort-imports': [
        'warn',
        {
          ignoreCase: false,
          ignoreDeclarationSort: true,
          ignoreMemberSort: false,
          memberSyntaxSortOrder: ['none', 'all', 'multiple', 'single'],
          allowSeparatedGroups: true,
        },
      ],

      'unused-imports/no-unused-imports': ['warn'],
      '@typescript-eslint/no-floating-promises': 'off',
      '@typescript-eslint/unbound-method': 'off',
      '@typescript-eslint/no-unsafe-assignment': 'off',
      '@typescript-eslint/prefer-promise-reject-errors': 'off',
      '@typescript-eslint/no-unsafe-member-access': 'off',
      '@typescript-eslint/no-unsafe-call': 'off',
      '@typescript-eslint/require-await': 'off',
      '@typescript-eslint/no-unsafe-return': 'off',
      '@typescript-eslint/await-thenable': 'off',
    },
  },
  {
    name: 'generated-api-config',
    files: ['src/api/generated/**/*.{ts,tsx}'],
    plugins: {
      'typescript-eslint': tsPlugin,
      'unused-imports': unusedImports,
    },
    languageOptions: {
      parser: tsParser,
      ecmaVersion: 'latest',
      sourceType: 'module',

      parserOptions: {
        parser: tsParser,
        project: path.resolve(__dirname, './tsconfig.json'),
        tsconfigRootDir: __dirname,
        sourceType: 'module',
      },
    },
    rules: {
      'no-unused-vars': 'off',
      'unused-imports/no-unused-imports': ['warn'],
      'unused-imports/no-unused-imports-ts': ['warn'],
    },
  },
  {
    name: 'vue-config',
    files: ['src/**/*.vue'],
    plugins: {
      'vue-eslint': pluginVue,
      '@typescript-eslint': tseslint.plugin,
      'unused-imports': unusedImports,
    },
    languageOptions: {
      parser: vueParser,
      ecmaVersion: 'latest',
      sourceType: 'module',

      parserOptions: {
        parser: tsParser,
        extraFileExtensions: ['.vue'],
        // project: path.resolve(__dirname, './tsconfig.json'),
        projectService: {
          defaultProject: path.resolve(__dirname, './tsconfig.json'),
          loadTypeScriptPlugins: !!process.env.VSCODE_PID,
        },
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
    settings: {
      '@typescript-eslint/parser': ['.vue'],
      'vue-eslint/parser': ['.vue'],
      'import-x/extensions': ['.vue'],
    },
  },
  // {
  //   files: ['**/*.{ts,tsx}'],
  //   extends: [tseslint.configs.disableTypeChecked],
  // },
  // keep as last item to override conflicting rules
  eslintConfigPrettier,
);
