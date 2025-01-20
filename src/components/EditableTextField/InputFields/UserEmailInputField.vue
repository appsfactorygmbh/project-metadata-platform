<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, inject, reactive, toRaw } from 'vue';
  import type { UserListModel, UserModel } from '@/models/User';
  import EmailInputTextField from './EmailInputTextField.vue';
  import { isValidEmail } from '@/utils/form/userValidation.ts';

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

  const dynamicValidateForm = reactive<FormType>({
    email: props.default,
  });

  const isUniqueEmail = (_rule: Rule, email: string) => {
    const users: UserListModel[] = userStore.getUsers;
    const currentUser: UserModel | null = userStore.getUser;

    // checks if email is already in use by another user
    if (
      users?.every(
        (user) => user.email !== email || user.email === currentUser?.email,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject('This email is already in use.');
  };

  const rulesRef = reactive<RulesObject<FormType>>({
    email: [
      {
        required: true,
        message: 'Please insert a valid email.',
        validator: isValidEmail,
        trigger: 'change',
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert an unique email.',
        validator: isUniqueEmail,
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
