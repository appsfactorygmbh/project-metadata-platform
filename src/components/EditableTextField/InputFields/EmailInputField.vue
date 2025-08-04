<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, reactive, toRaw } from 'vue';
  import type { UserListModel, UserModel } from '@/models/User';
  import InputField from './InputField.vue';
  import { isValidEmail } from '@/utils/form/userValidation.ts';
  import { useUserStore } from '@/store';
  import useNotification from 'ant-design-vue/es/notification/useNotification';

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

  const userStore = useUserStore();

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
    return Promise.reject(new Error('This email is already in use.'));
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

  const [notificationApi] = useNotification();

  const onSubmit: FormSubmitType = async (fields) => {
    const newEmail = {
      email: toRaw(fields).email,
    };
    await userStore
      .update(props.userId, newEmail)
      .then((res) => {
        console.log('err successfull ' + JSON.stringify(res));
        notificationApi.success({
          message: 'E-Mail updated',
        });
      })
      .catch((error) => {
        notificationApi.error({
          message: 'An error occurred. The email could not be updated',
        });
        console.error('Error updating email:', error);
      });
    console.log('test after fetch');
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
      <InputField
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
