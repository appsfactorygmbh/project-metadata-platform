<template>
  <div>
    <a-button type="primary" @click="showDialog">Test Button</a-button>

    <ConfirmationDialog
      :is-open="isDialogOpen"
      title="Delete confirm"
      message="Are you sure you want to delete the plugin?"
      @confirm="handleConfirm"
      @cancel="handleCancel"
      @update:is-open="isDialogOpen = $event"
    />
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import { message } from 'ant-design-vue';
  import ConfirmationDialog from '@/components/ConfirmAction/ConfirmationDialog.vue';
  import { useRouter } from 'vue-router';

  const isDialogOpen = ref(false);
  const router = useRouter();

  const showDialog = () => {
    isDialogOpen.value = true;
  };

  const handleConfirm = () => {
    message.success('The Plugin deleted', 2, () => {
      router.push('/plugin-settings');
    });
    isDialogOpen.value = false;
  };

  // Kommentar für Reviewer, habe keine andere lösung gefunden, bei drücken des no buttons, kommt zwei mal die error Meldung
  const handleCancel = () => {
    if (isDialogOpen.value) {
      message.error("Deleting the plugin didn't work", 2, () => {
        router.push('/plugin-settings');
      });
      isDialogOpen.value = false;
    }
  };
</script>

<style scoped>
  /* Fügen Sie bei Bedarf weitere Styles hinzu */
</style>
