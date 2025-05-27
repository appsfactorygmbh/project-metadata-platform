<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { message } from 'ant-design-vue';
  import { reactive, ref, toRaw } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateTeamFormData } from './CreateTeamFormData.ts';
  import { CreateIsUniqueTeamName } from '@/utils/form/userValidation.ts';
  import type { CreateTeamModel } from '@/models/Team/CreateTeamModel.ts';
  import type { TeamStore } from '@/store/TeamStore.ts';

  const { formStore, initialValues, teamStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateTeamFormData;
    teamStore: TeamStore;
  }>();

  const emit = defineEmits<(e: 'newId', id: number) => void>();
  const onSubmit: FormSubmitType = async (fields) => {
    try {
      const teamDef: CreateTeamModel = {
        ptl: toRaw(fields).ptl,
        businessUnit: toRaw(fields).businessUnit,
        teamName: toRaw(fields).teamName,
      };
      const id = await teamStore.create(teamDef);
      await teamStore.fetchAll();
      message.success('Team created', 2);
      emit('newId', id);
    } catch (error) {
      message.error('An error occurred. The team could not be created', 10);
      console.error('Error creating team:', error);
    }
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm = reactive<CreateTeamFormData>(initialValues);

  const rulesRef = reactive<RulesObject<CreateTeamFormData>>({
    teamName: [
      {
        required: true,
        message: 'Please insert an unique team name.',
        validator: CreateIsUniqueTeamName(teamStore),
        trigger: 'change',
        type: 'string',
      },
    ],
    ptl: [
      {
        required: false,
        type: 'string',
      },
    ],
    businessUnit: [
      {
        required: true,
        type: 'string',
      },
    ],
    inputsDisabled: [
      {
        required: false,
      },
    ],
  });

  formStore.setOnSubmit(onSubmit);
  formStore.setModel(dynamicValidateForm);
  formStore.setRules(rulesRef);

  const formRef = ref();
</script>

<template>
  <a-form
    ref="formRef"
    :model="dynamicValidateForm"
    v-bind="formItemLayoutWithOutLabel"
    :wrapper-col="{ span: 24 }"
  >
    <a-form-item
      has-feedback
      name="teamName"
      class="column"
      :whitespace="true"
      :rules="rulesRef.teamName"
    >
      <a-input
        id="inputCreateTeamTeamName"
        v-model:value="dynamicValidateForm.teamName"
        class="inputField"
        placeholder="Team Name"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.teamName"
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="businessUnit"
      class="column"
      :whitespace="false"
      :rules="rulesRef.businessUnit"
    >
      <a-input
        id="inputCreateTeamBusinessUnit"
        v-model:value="dynamicValidateForm.businessUnit"
        class="inputField"
        placeholder="Business Unit"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.businessUnit"
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="ptl"
      class="column"
      :whitespace="true"
      :rules="rulesRef.ptl"
    >
      <a-input
        id="inputCreateTeamPtl"
        v-model:value="dynamicValidateForm.ptl"
        class="inputField"
        placeholder="PTL"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.ptl"
      />
    </a-form-item>
  </a-form>
  <contextHolder />
</template>

<style scoped>
  .column {
    margin: 0;
  }
</style>
