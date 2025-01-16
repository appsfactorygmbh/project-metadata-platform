import { useDark, useToggle } from '@vueuse/core';
import { theme } from 'ant-design-vue';
import { ref } from 'vue';

export const themeConfig = ref({
  algorithm: theme.defaultAlgorithm,
});

export function useTheme() {
  const isDark = useDark({
    selector: 'body',
    attribute: 'color-scheme',
    valueDark: 'dark',
    valueLight: 'light',
    onChanged: (isDark) => {
      themeConfig.value.algorithm = isDark
        ? theme.darkAlgorithm
        : theme.defaultAlgorithm;
    },
  });

  const toggleDark = useToggle(isDark);

  return {
    isDark,
    toggleDark,
    themeConfig,
  };
}
