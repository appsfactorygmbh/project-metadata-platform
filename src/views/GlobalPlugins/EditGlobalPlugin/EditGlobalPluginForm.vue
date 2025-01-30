<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { message } from 'ant-design-vue';
  import { useGlobalPluginsStore } from '@/store';
  import { onMounted, reactive } from 'vue';
  import { useRoute } from 'vue-router';
  import GlobalPluginForm from '../GlobalPluginForm/GlobalPluginForm.vue';
  import type { GlobalPluginFormData } from '../GlobalPluginForm';

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();

  const globalPluginStore = useGlobalPluginsStore();

  const onSubmit: FormSubmitType = async (fields) => {
    if (!pluginIdRef.value) {
      return;
    }

    fields['pluginName'] =
      fields['pluginName'] === oldPluginName ? undefined : fields['pluginName'];

    try {
      await globalPluginStore.update(pluginIdRef.value, { ...fields });
      message.success('Plugin updated successfully.');
    } catch (error) {
      message.error((error as Error).message ?? 'An error occurred');
      return Promise.reject();
    }
  };

  const pluginIdRef = ref<number | null>(null);

  const initialValues = reactive<GlobalPluginFormData>({
    pluginName: '',
    keys: [],
    baseUrl: '',
  });
  let oldPluginName: string = '';

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
        oldPluginName = globalPluginData.pluginName;
        initialValues.keys =
          globalPluginData.keys?.map((keyObj, index) => ({
            // TODO: adapt when feature to archive keys is implemented
            key: index, //keyObj.key,
            value: keyObj, //keyObj.value,
            archived: false, //keyObj.archived,
          })) ?? [];
        initialValues.baseUrl = globalPluginData.baseUrl;
      }
    }
  });

  formStore.setOnSubmit(onSubmit);
</script>

<template>
  <GlobalPluginForm :form-store="formStore" :initial-values="initialValues" />
  <contextHolder />
</template>
