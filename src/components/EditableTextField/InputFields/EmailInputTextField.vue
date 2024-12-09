<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, inject, reactive, toRaw } from 'vue';
  import type { UserListModel, UserModel } from '@/models/User';

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
    email: string;
  };

  const userStore = inject(userStoreSymbol)!;

  const isUniqueEmail = (email: string) => {
    const users: UserListModel[] = userStore.getUsers;
    const currentUser: UserModel | null = userStore.getUser;

    // checks if email is already in use by another user
    if (
      email === users?.find((user) => user.email === email)?.email &&
      email !== currentUser?.email
    ) {
      return false;
    }
    return true;
  };

  const validateEmail = (_rule: Rule, value: string) => {
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

    if (value && emailRegex.test(value)) {
      if (isUniqueEmail(value)) {
        return Promise.resolve();
      }
      return Promise.reject('This email is already in use.');
    } else {
      return Promise.reject('Please enter a valid email.');
    }
  };

  const dynamicValidateForm = reactive<FormType>({
    email: props.default,
  });

  const rulesRef = reactive<RulesObject<FormType>>({
    email: [
      {
        required: true,
        validator: validateEmail,
        message: 'Please enter a valid and unique E-Mail adress',
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  const onSubmit: FormSubmitType = (fields) => {
    const newEmail = {
      email: toRaw(fields).email,
    };
    userStore.update(props.userId, newEmail);
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setRules(rulesRef);
  props.formStore.setOnSubmit(onSubmit);

  const formRef = ref();
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm">
    <a-form-item
      :rules="rulesRef.email"
      name="email"
      class="formItem email"
      has-feedback
    >
      <a-input
        v-model:value="dynamicValidateForm.email"
        :placeholder="props.placeholder"
        :rules="rulesRef.email"
      ></a-input>
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
