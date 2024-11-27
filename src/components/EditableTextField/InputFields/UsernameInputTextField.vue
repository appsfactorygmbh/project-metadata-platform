<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, defineProps, inject, reactive, toRaw } from 'vue';
  import type { UserListModel } from '@/models/User';

  const props = defineProps({
    userId: {
      type: String,
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
    username: string;
  };

  const formRef = ref();

  const userStore = inject(userStoreSymbol)!;

  const validateUsername = (_rule: Rule, value: string) => {
    if (value === '') {
      return Promise.reject('Please enter a username.');
    } else {
      const users: UserListModel[] = userStore?.getUsers;

      if (value === users?.find((user) => user.name === value)?.name) {
        return Promise.reject('Username already exists.');
      } else {
        return Promise.resolve();
      }
    }
  };

  const dynamicValidateForm = reactive<FormType>({
    username: props.default,
  });

  const rulesRef = reactive<RulesObject<FormType>>({
    username: [
      {
        required: true,
        validator: validateUsername,
        message: 'Please enter a unique username',
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  const onSubmit: FormSubmitType = async (fields) => {
    const newUsername = {
      username: toRaw(fields).username,
    };
    await userStore.patchUser(props.userId, newUsername);
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setRules(rulesRef);
  props.formStore.setOnSubmit(onSubmit);
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm" class="form">
    <a-form-item :rules="rulesRef.username" name="username" class="formItem">
      <a-input
        v-model:value="dynamicValidateForm.username"
        :placeholder="props.placeholder"
        :rules="rulesRef.username"
      ></a-input>
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
