<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { App } from 'ant-design-vue';
  import { useGlobalPluginStore } from '@/store';
  import { onMounted, reactive } from 'vue';
  import { useRoute } from 'vue-router';
  import GlobalPluginForm from '../GlobalPluginForm/GlobalPluginForm.vue';
  import type { GlobalPluginFormData } from '../GlobalPluginForm';
  import type { PatchGlobalPluginRequest } from '@/api/generated';

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();
  const { notification } = App.useApp();
  const globalPluginStore = useGlobalPluginStore();

  const onSubmit: FormSubmitType = (fields: PatchGlobalPluginRequest) => {
    if (!pluginIdRef.value) {
      return;
    }

    globalPluginStore
      .update(pluginIdRef.value, { ...fields })
      .then(() =>
        notification.success({
          message: 'Success!',
          description: 'Plugin updated successfully.',
        }),
      )
      .catch((error) => {
        notification.error({
          message: 'Error!',
          description: (error as Error).message ?? 'An error occurred.',
        });
        throw error;
      });
  };

  const pluginIdRef = ref<number | null>(null);

  const initialValues = reactive<GlobalPluginFormData>({
    pluginName: '',
    keys: [],
    baseUrl: '',
  });

  onMounted(async () => {
    const route = useRoute();
    const { pluginId } = route.query;
    if (typeof pluginId === 'string') {
      const numericPluginId = parseInt(pluginId, 10);
      if (!isNaN(numericPluginId)) {
        pluginIdRef.value = numericPluginId;
        const globalPluginData =
          await globalPluginStore?.fetch(numericPluginId);
        if (!globalPluginData) {
          return;
        }
        initialValues.pluginName = globalPluginData.pluginName;
        initialValues.keys =
          globalPluginData.keys?.map((keyObj, index) => ({
            // TODO: adapt when feature to archive keys is implemented
            key: index, //keyObj.key,
            value: keyObj, //keyObj.value,
            archived: false, //keyObj.archived,
          })) ?? [];
        initialValues.baseUrl = globalPluginData.baseUrl ?? '';
      }
    }
  });

  formStore.setOnSubmit(onSubmit);
</script>

<template>
  <GlobalPluginForm :form-store="formStore" :initial-values="initialValues" />
</template>
