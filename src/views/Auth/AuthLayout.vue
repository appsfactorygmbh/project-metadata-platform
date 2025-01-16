<script setup lang="ts">
  import { BackgroundContainer } from '@/components/Container';

  import { computed } from 'vue';
  import useBreakpoint from 'ant-design-vue/es/_util/hooks/useBreakpoint';
  import { useToken } from 'ant-design-vue/es/theme/internal';

  // Token and Breakpoints
  const token = useToken()[1];
  const screens = useBreakpoint();

  const bgColor = '#dddddd'; // token.value.colorBgContainer

  // Styles
  const styles = computed(() => ({
    container: {
      margin: '0 auto',
      padding: screens.value.md
        ? `${token.value.paddingXL}px`
        : `${token.value.sizeXXL}px ${token.value.padding}px`,
      width: '380px',
    },
    section: {
      alignItems: 'center',
      backgroundColor: bgColor,
      display: 'flex',
      height: screens.value.sm ? '100vh' : 'auto',
    },
  }));

  const cardStyle = {
    backgroundColor: 'white',
    boxShadow: '0px 0px 20px grey',
    padding: 0,
  };

  const innerCardStyle = {
    backgroundColor: 'white',
    margin: '-24px',
  };
</script>
<template>
  <BackgroundContainer :backgroud-color="bgColor">
    <section :style="styles.section">
      <a-card :bordered="true" :style="cardStyle">
        <a-card :bordered="true" :hoverable="true" :style="innerCardStyle">
          <div :style="styles.container">
            <slot />
          </div>
        </a-card>
      </a-card>
    </section>
  </BackgroundContainer>
</template>

<style scoped>
  section {
    align-items: center;
    background-color: #fcf0eb;
    display: flex;
  }
</style>
