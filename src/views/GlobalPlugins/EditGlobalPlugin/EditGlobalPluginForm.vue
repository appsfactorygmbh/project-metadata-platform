<script setup lang="ts">
  import { type FormSubmitType } from '@/components/Form';
  import { notification } from 'ant-design-vue';
  import { globalPluginStoreSymbol } from '@/store/injectionSymbols';
  import { inject, reactive, onMounted } from 'vue';
  import { type FormStore } from '@/components/Form';
  import { useRoute } from 'vue-router';
  import type { GlobalPluginKey } from '@/models/Plugin/GlobalPluginModel';
  import GlobalPluginForm from '../GlobalPluginForm/GlobalPluginForm.vue';
  import type { GlobalPluginFormData } from '../GlobalPluginForm';
  import type { GlobalPluginModel } from '@/models/Plugin';

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();

  const globalPluginStore = inject(globalPluginStoreSymbol);

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      console.log(fields);
      const globalPluginData: GlobalPluginModel = {
        id: fields.id,
        name: fields.pluginName,
        keys: fields.GlobalPluginKeys.map((key: GlobalPluginKey) => ({
          key: key.value,
        })),
        archived: false,
      };
      globalPluginStore?.patchGlobalPlugin(globalPluginData);
    } catch {
      notificationApi.error({
        message: 'An error occurred. The plugin could not be created',
      });
      console.log('fehler');
    }
  };

  const initialValues = reactive<GlobalPluginFormData>({
    pluginName: '',
    keys: [],
  });

  onMounted(async () => {
    const route = useRoute();
    const { pluginId } = route.params;
    if (typeof pluginId === 'string') {
      const numericPluginId = parseInt(pluginId, 10);
      if (!isNaN(numericPluginId)) {
        const globalPluginData =
          await globalPluginStore?.getGlobalPluginById(numericPluginId);
        if (globalPluginData) {
          initialValues.pluginName = globalPluginData.name;
          initialValues.keys =
            globalPluginData.keys?.map((keyObj, index) => ({
              key: index, // No need to convert to string, but if needed elsewhere, use String(index)
              value: keyObj.key as unknown as string,
            })) ?? [];
        }
      }
    }
  });

  formStore.setOnSubmit(onSubmit);
</script>

<template>
  <GlobalPluginForm :form-store="formStore" :initial-values="initialValues" />
  <contextHolder></contextHolder>
</template>
