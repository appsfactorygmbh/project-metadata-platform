<script setup lang="ts">
  import { type FormSubmitType } from '@/components/Form';
  import { notification } from 'ant-design-vue';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { inject, reactive, onMounted } from 'vue';
  //import type { CreatePluginModel } from '@/models/Plugin';
  import { type FormStore } from '@/components/Form';
  import { useRoute } from 'vue-router';
  import { pluginService } from '@/services';
  import GlobalPluginForm from '../GlobalPluginForm/GlobalPluginForm.vue';
  import type { GlobalPluginFormData } from '../GlobalPluginForm';

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();

  const pluginStore = inject(pluginStoreSymbol);

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      console.log(fields);
      pluginStore?.createPlugin(fields);
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
          await pluginService.fetchGlobalPluginData(numericPluginId);
        initialValues.pluginName = globalPluginData.pluginName;
        const keysArray = Object.entries(globalPluginData.keys).map(
          ([, value], index) => ({
            key: index,
            value: value as string,
          }),
        );
        initialValues.keys = keysArray;
      }
    }
  });

  formStore.setOnSubmit(onSubmit);
</script>

<template>
  <GlobalPluginForm :form-store="formStore" :initial-values="initialValues" />
  <contextHolder></contextHolder>
</template>
