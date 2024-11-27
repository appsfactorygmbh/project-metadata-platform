<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, defineProps, inject, reactive, toRaw } from 'vue';

  const props = defineProps({
    userId: {
      type: Number,
      required: true,
    },
    formStore: {
      type: Object as PropType<FormStore>,
      required: true,
    },
    placeholder: {
      type: String,
      required: true,
    },
    default: {
      type: String,
      required: true,
    },
  });

  type FormType = {
    name: string;
  };

  const formRef = ref();

  const userStore = inject(userStoreSymbol)!;

  const validateName = (_rule: Rule, value: string) => {
    if (value === '') {
      return Promise.reject('Please enter a name.');
    } else {
      return Promise.resolve();
    }
  };

  const dynamicValidateForm = reactive<FormType>({
    name: props.default,
  });

  const rulesRef = reactive<RulesObject<FormType>>({
    name: [
      {
        required: true,
        validator: validateName,
        message: 'Please enter a name',
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  const onSubmit: FormSubmitType = async (fields) => {
    const newName = {
      name: toRaw(fields).name,
    };
    await userStore.patchUser(props.userId, newName);
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setRules(rulesRef);
  props.formStore.setOnSubmit(onSubmit);
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm">
    <a-form-item :rules="rulesRef.name" name="name" class="formItem">
      <a-input
        v-model:value="dynamicValidateForm.name"
        :placeholder="props.placeholder"
        :rules="rulesRef.name"
      ></a-input>
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
