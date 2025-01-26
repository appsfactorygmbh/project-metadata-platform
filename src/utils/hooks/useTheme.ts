import darkTheme from '@/assets/themes/darkTheme';
import defaultTheme from '@/assets/themes/defaultTheme';
import { useDark, useToggle } from '@vueuse/core';
import { theme } from 'ant-design-vue';
import { ref } from 'vue';

export const themeConfig = ref({
  algorithm: theme.defaultAlgorithm,
  token: defaultTheme.token,
});

export function useTheme() {
  const isDark = useDark({
    valueDark: 'dark',
    valueLight: 'light',
    onChanged: (isDark) => {
      themeConfig.value.algorithm = isDark
        ? theme.darkAlgorithm
        : theme.defaultAlgorithm;
      themeConfig.value.token = isDark ? darkTheme.token : defaultTheme.token;
    },
  });

  const toggleDark = useToggle(isDark);

  return {
    isDark,
    toggleDark,
    themeConfig,
  };
}

export const useThemeToken = () => {
  const { useToken } = theme;
  const { token } = useToken();
  return token;
};
