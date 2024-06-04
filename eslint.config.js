// eslint.config.js

import js from "eslint-plugin-vue";
import { FlatCompat } from "@eslint/eslintrc";
import tseslint from 'typescript-eslint';
import path from "path";
import { fileURLToPath } from "url";

// mimic CommonJS variables -- not needed if using CommonJS
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const compat = new FlatCompat({
  baseDirectory: __dirname
});

export default [
    ...tseslint.configs.recommended,
    ...compat.extends("plugin:vue/vue3-recommended"),
];