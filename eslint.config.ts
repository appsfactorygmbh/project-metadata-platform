import eslint from '@eslint/js';
import type { Linter } from 'eslint';
import tseslint, { type ConfigWithExtends } from 'typescript-eslint';
import tsParser from '@typescript-eslint/parser';
// import tsPlugin from '@typescript-eslint/eslint-plugin';
import vuePlugin from 'eslint-plugin-vue';
import * as vueParser from 'vue-eslint-parser';
import eslintConfigPrettier from 'eslint-config-prettier';
import unusedImports from 'eslint-plugin-unused-imports';
import globals from 'globals';

import path from 'path';
import { fileURLToPath } from 'url';
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

type Parser = NonNullable<Linter.Config['languageOptions']>['parser'];
type Plugin =
  NonNullable<Linter.Config['plugins']> extends Record<string, infer T>
    ? T
    : never;

// const tsParser = tseslint.parser as Parser;
const tsPlugin = tseslint.plugin as Plugin;

const extraFileExtensions = ['.vue'];

const ignoreFiles: Linter.Config = {
  name: 'ignore-files',
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
};
const globalsConfig: Linter.Config = {
  name: 'globals-config',
  languageOptions: {
    globals: {
      ...globals.browser,
      ...globals.node,
    },
  },
};
const linterConfig: Linter.Config = {
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
      extraFileExtensions,
    },
  },
  rules: {
    '@typescript-eslint/ban-ts-comment': 'off',
    '@typescript-eslint/no-unused-vars': 'off',
  },
};
const vitestConfig: Linter.Config = {
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
      extraFileExtensions,
    },
  },
};
const vueConfig: Linter.Config = {
  name: 'vue-config',
  files: ['src/**/*.vue'],
  plugins: {
    'vue-eslint': vuePlugin,
    '@typescript-eslint': tsPlugin,
    'unused-imports': unusedImports,
  },
  languageOptions: {
    parser: vueParser,
    ecmaVersion: 'latest',
    sourceType: 'module',

    parserOptions: {
      parser: tsParser,
      extraFileExtensions,
      // project: path.resolve(__dirname, './tsconfig.json'),
      projectService: {
        defaultProject: path.resolve(__dirname, './tsconfig.app.json'),
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
};
const typescriptConfig: Linter.Config = {
  name: 'typescript-config',
  files: ['src/**/!(generated)/**/*.{ts,tsx}'],
  ignores: [
    'src/**/__tests__/**/*.{ts,tsx}',
    // 'src/api/generated/**/*.{ts,tsx}',
  ],
  plugins: {
    '@typescript-eslint': tsPlugin,
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
        defaultProject: path.resolve(__dirname, './tsconfig.app.json'),
        loadTypeScriptPlugins: !!process.env.VSCODE_PID,
      },
      extraFileExtensions,
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
    '@typescript-eslint/no-unused-vars': 'off',
    'unused-imports/no-unused-imports': ['warn'],

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
  },
};
const generatedApiConfig: Linter.Config = {
  name: 'generated-api-config',
  files: ['src/api/generated/**/*.{ts,tsx}'],
  plugins: {
    'unused-imports': unusedImports,
  },
  languageOptions: {
    parser: tsParser,
    ecmaVersion: 'latest',
    sourceType: 'module',

    parserOptions: {
      projectService: {
        defaultProject: path.resolve(__dirname, './tsconfig.json'),
        loadTypeScriptPlugins: !!process.env.VSCODE_PID,
      },
      tsconfigRootDir: __dirname,
      sourceType: 'module',
      extraFileExtensions,
    },
  },
  rules: {
    'no-unused-vars': 'off',
    '@typescript-eslint/no-unused-vars': 'off',
    'unused-imports/no-unused-imports': ['error'],
    '@typescript-eslint/no-explicit-any': 'off',
    'unused-imports/no-unused-vars': ['off'],
  },
};
const ruleOverrides: Linter.Config = {
  name: 'rule-overrides',
  rules: {
    'no-unused-vars': 'off',
    '@typescript-eslint/no-floating-promises': 'off',
    '@typescript-eslint/unbound-method': 'off',
    '@typescript-eslint/no-unsafe-assignment': 'off',
    '@typescript-eslint/prefer-promise-reject-errors': 'off',
    '@typescript-eslint/no-unsafe-member-access': 'off',
    '@typescript-eslint/no-unsafe-call': 'off',
    '@typescript-eslint/require-await': 'off',
    '@typescript-eslint/no-unsafe-return': 'off',
    '@typescript-eslint/await-thenable': 'off',
    '@typescript-eslint/no-unsafe-argument': 'off',
    '@typescript-eslint/restrict-template-expressions': 'off',
    '@typescript-eslint/restrict-plus-operands': 'off',
    '@typescript-eslint/no-for-in-array': 'off',
    '@typescript-eslint/no-misused-promises': 'off',
  },
};

const disableTypeChecked: ConfigWithExtends = {
  name: 'disable-type-checked',
  files: ['**/*.{ts,tsx}'],
  extends: [tseslint.configs.disableTypeChecked],
};

export default tseslint.config(
  {
    name: 'typescript-base-config',
    files: ['src/**/!(generated)/**/*.{ts,tsx}'],
    extends: [
      eslint.configs.recommended,
      ...tseslint.configs.recommendedTypeChecked,
    ],
  },
  {
    files: ['src/**/*.{vue}'],
    extends: [
      ...vuePlugin.configs['flat/base'],
      ...vuePlugin.configs['flat/recommended'],
    ],
  },
  ignoreFiles,
  globalsConfig,
  linterConfig,
  vitestConfig,
  vueConfig,
  typescriptConfig,
  generatedApiConfig,
  ruleOverrides,
  //disableTypeChecked,
  // keep as last item to override conflicting rules
  // { name: 'prettier', ...eslintConfigPrettier },
);
