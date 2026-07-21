<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { EditGlobalPluginForm } from './';
  import { useFormStore } from '@/components/Form';
  import { useRouter } from 'vue-router';
  import { useGlobalPluginStore } from '@/store';
  import { message } from 'ant-design-vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { ResourceActions } from '@/models/utils';

  const formStore = useFormStore('editPluginForm');
  const globalPluginsStore = useGlobalPluginStore();
  const router = useRouter();
  const route = useRoute();
  const isDialogOpen = ref(false);

  const numericPluginId = computed(() => {
    const id = route.query.pluginId;
    if (typeof id !== 'string') return null;
    const parsed = Number.parseInt(id, 10);
    return Number.isNaN(parsed) ? null : parsed;
  });
  const canEdit = ref(false);
  watchEffect(async () => {
    if (numericPluginId.value === null) {
      canEdit.value = false;
      return;
    }

    try {
      const plugin = await globalPluginsStore.findPlugin(numericPluginId.value);
      canEdit.value =
        plugin?.permissions?.includes(ResourceActions.Edit) ?? false;
    } catch (error) {
      console.error('Failed to fetch plugin permissions:', error);
      canEdit.value = false;
    }
  });

  const onClose = () => {
    router.push('/settings/global-plugins');
  };

  const showDialog = () => {
    isDialogOpen.value = true;
  };

  const handleConfirm = async () => {
    if (numericPluginId.value) {
      await globalPluginsStore?.archive(numericPluginId.value);
      message.success('The plugin has been archived', 2);
    } else {
      message.error('The plugin could not be archived', 2);
    }
    isDialogOpen.value = false;

    onClose();
  };

  const handleCancel = () => {
    isDialogOpen.value = false;
  };
</script>

<template>
  <FormModal title="Edit Plugin" :form-store="formStore" @close="onClose">
    <EditGlobalPluginForm :form-store="formStore" />

    <template #footer="{ handleOk, resetModal }">
      <div style="display: flex; justify-content: space-between; width: 100%">
        <a-button danger :disabled="!canEdit" @click="showDialog"
          >Archive</a-button
        >

        <div style="display: flex; gap: 8px">
          <a-button @click="resetModal">Cancel</a-button>
          <a-button :disabled="!canEdit" type="primary" @click="handleOk"
            >OK</a-button
          >
        </div>
      </div>
    </template>
  </FormModal>
  <ConfirmationDialog
    :is-open="isDialogOpen"
    title="Archive"
    message="Are you sure you want to archive this plugin?"
    @confirm="handleConfirm"
    @cancel="handleCancel"
    @update:is-open="isDialogOpen = $event"
  />
</template>
