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
          await globalPluginStore?.fetchGlobalPlugin(numericPluginId);
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
