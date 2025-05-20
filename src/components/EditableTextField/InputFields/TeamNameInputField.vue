<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, reactive, toRaw } from 'vue';
  import InputField from './InputField.vue';
  import { useTeamStore } from '@/store';
  import useNotification from 'ant-design-vue/es/notification/useNotification';
  import type { TeamModel } from '@/models/Team';

  const props = defineProps({
    teamId: {
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
    teamName: string;
  };

  const teamStore = useTeamStore();

  const dynamicValidateForm = reactive<FormType>({
    teamName: props.default,
  });

  const isUniqueTeamName = (_rule: Rule, name: string) => {
    const teams: TeamModel[] = teamStore.getTeams;
    const currentTeam: TeamModel = teamStore.getTeam;

    if (
      teams?.every(
        (team) => team.teamName !== name || name === currentTeam.teamName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(new Error('This team name is already in use.'));
  };

  const rulesRef = reactive<RulesObject<FormType>>({
    teamName: [
      {
        required: true,
        message: 'Please insert an unique team name.',
        validator: isUniqueTeamName,
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  const [notificationApi] = useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    const newTeamName = {
      teamName: toRaw(fields).teamName,
    };
    teamStore
      .update(props.teamId, newTeamName)
      .then(() => {
        notificationApi.success({
          message: 'Team Name updated',
        });
      })
      .catch((error) => {
        notificationApi.error({
          message: 'An error occurred. The team name could not be updated',
        });
        console.error('Error updating team:', error);
      });
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setRules(rulesRef);
  props.formStore.setOnSubmit(onSubmit);

  const formRef = ref();
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm">
    <a-form-item
      :rules="rulesRef.teamName"
      name="teamName"
      class="formItem teamName"
      has-feedback
    >
      <InputField
        v-model:value="dynamicValidateForm.teamName"
        :placeholder="props.placeholder"
        :default="props.default"
        :rules="rulesRef.teamName"
      />
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
