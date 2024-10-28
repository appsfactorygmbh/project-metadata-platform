// @ts-check

import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';
import pluginVue from 'eslint-plugin-vue';
import eslintConfigPrettier from 'eslint-config-prettier';
import unusedImports from 'eslint-plugin-unused-imports';
import * as tsParser from '@typescript-eslint/parser';
import * as vueParser from 'vue-eslint-parser';
import globals from 'globals';

import path from 'path';
import { fileURLToPath } from 'url';
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const parser = vueParser; //tsParser;
const parserOptionParser = tsParser;

export default tseslint.config(
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
  eslint.configs.recommended,
  ...tseslint.configs.recommended,
  ...pluginVue.configs['flat/base'],
  ...pluginVue.configs['flat/recommended'],
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
      parser: parser,
      ecmaVersion: 'latest',
      sourceType: 'module',

      parserOptions: {
        parser: parserOptionParser,
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
      'unused-imports': unusedImports,
    },
    languageOptions: {
      parser: parser,
      ecmaVersion: 'latest',
      sourceType: 'module',

      parserOptions: {
        parser: parserOptionParser,
        extraFileExtensions: ['.vue'],
        // project: path.resolve(__dirname, './tsconfig.app.json'),
        tsconfigRootDir: __dirname,
        sourceType: 'module',
        ecmaFeatures: {
          modules: true,
          jsx: true,
        },
        projectService: {
          defaultProject: path.resolve(__dirname, './tsconfig.app.json'),
          loadTypeScriptPlugins: !!process.env.VSCODE_PID,
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
      '@typescript-eslint/parser': ['.ts', '.tsx', '.vue'],
      'import-x/extensions': ['.ts', '.tsx', '.vue'],

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
    },
  },
  {
    name: 'vitest-config',
    files: ['src/**/__tests__/**/*.{ts,tsx}'],
    languageOptions: {
      parser: parser,
      ecmaVersion: 'latest',
      sourceType: 'module',

      parserOptions: {
        parser: parserOptionParser,
        project: path.resolve(__dirname, './tsconfig.vitest.json'),
        tsconfigRootDir: __dirname,
        sourceType: 'module',
      },
    },
  },
  // keep as last item to override conflicting rules
  eslintConfigPrettier,
);
