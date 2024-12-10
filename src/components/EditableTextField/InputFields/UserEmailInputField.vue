<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, inject, reactive, toRaw } from 'vue';
  import type { UserListModel, UserModel } from '@/models/User';
  import EmailInputTextField from './EmailInputTextField.vue';

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

  const isUniqueEmail = (_rule: Rule, email: string) => {
    const users: UserListModel[] = userStore.getUsers;
    const currentUser: UserModel | null = userStore.getUser;

    // checks if email is already in use by another user
    if (
      users?.every((user) => user.email !== email) &&
      email !== currentUser?.email
    ) {
      return Promise.reject('This email is already in use.');
    }
    return Promise.resolve();
  };

  const dynamicValidateForm = reactive<FormType>({
    email: props.default,
  });

  const rulesRef = reactive<RulesObject<FormType>>({
    email: [
      {
        required: true,
        validator: isUniqueEmail,
        message: 'Please enter a valid and unique E-Mail adress',
        trigger: 'change',
        type: 'email',
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
      <EmailInputTextField
        v-model:value="dynamicValidateForm.email"
        :placeholder="props.placeholder"
        :default="props.default"
        :rules="rulesRef.email"
      />
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
