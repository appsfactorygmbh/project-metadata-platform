/// <reference types="vitest" />
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  resolve: {
    alias: {
      lodash: 'lodash-es'
    }
  },
  plugins: [vue()],
  test: {
    globals: true,
    environment: "happy-dom"
  }
})
