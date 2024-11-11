<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { notification } from 'ant-design-vue';
  import { useGlobalPluginsStore } from '@/store';
  import { onMounted, reactive } from 'vue';
  import { useRoute } from 'vue-router';
  import GlobalPluginForm from '../GlobalPluginForm/GlobalPluginForm.vue';
  import type { GlobalPluginFormData } from '../GlobalPluginForm';

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();

  const globalPluginStore = useGlobalPluginsStore();

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      console.log(fields);
      globalPluginStore?.update({
        id: pluginIdRef.value,
        ...fields,
      });
    } catch {
      notificationApi.error({
        message: 'The plugin could not be updated.',
      });
      console.log('fehler');
    }
  };

  const pluginIdRef = ref<number | null>(null);

  const initialValues = reactive<GlobalPluginFormData>({
    pluginName: '',
    keys: [],
  });

  onMounted(async () => {
    const route = useRoute();
    const { pluginId } = route.query;
    console.log('pluginId', pluginId);
    if (typeof pluginId === 'string') {
      const numericPluginId = parseInt(pluginId, 10);
      if (!isNaN(numericPluginId)) {
        pluginIdRef.value = numericPluginId;
        const globalPluginData =
          await globalPluginStore?.fetch(numericPluginId);
        if (!globalPluginData) {
          return;
        }
        console.log(globalPluginData);
        initialValues.pluginName = globalPluginData.name;
        initialValues.keys =
          globalPluginData.keys?.map((keyObj) => ({
            key: keyObj.key,
            value: keyObj.value,
            archived: keyObj.archived,
          })) ?? [];
      }
    }
  });

  formStore.setOnSubmit(onSubmit);
</script>

<template>
  <GlobalPluginForm :form-store="formStore" :initial-values="initialValues" />
  <contextHolder></contextHolder>
</template>
