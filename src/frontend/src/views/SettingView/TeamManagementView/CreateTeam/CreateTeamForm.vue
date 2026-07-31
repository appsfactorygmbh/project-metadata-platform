<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { App } from 'ant-design-vue';
  import { reactive, toRaw } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateTeamFormData } from './CreateTeamFormData.ts';
  import { CreateIsUniqueTeamName } from '@/utils/form/userValidation.ts';
  import type { CreateTeamModel } from '@/models/Team/CreateTeamModel.ts';
  import type { TeamStore } from '@/store/TeamStore.ts';
  import { useBusinessUnitStore } from '@/store/BusinessUnitStore.ts';
  import { storeToRefs } from 'pinia';

  const { formStore, initialValues, teamStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateTeamFormData;
    teamStore: TeamStore;
  }>();
  const { notification } = App.useApp();
  const buStore = useBusinessUnitStore();

  const { getBusinessUnits } = storeToRefs(buStore);

  interface SelectOption {
    value?: string | number | null;
    name?: string;
    [key: string]: unknown;
  }

  const filterOption = (input: string, option?: SelectOption): boolean => {
    if (!option?.name) return false;
    return option.name.toLowerCase().includes(input.toLowerCase());
  };

  const emit = defineEmits<(e: 'newId', id: number) => void>();
  const onSubmit: FormSubmitType = async (fields) => {
    try {
      const teamDef: CreateTeamModel = {
        ptl: toRaw(fields).ptl,
        businessUnitId: toRaw(fields).businessUnit,
        teamName: toRaw(fields).teamName,
      };
      const id = await teamStore.create(teamDef);
      await teamStore.fetchAll();
      notification.success({
        message: 'Success!',
        description: 'Team created successfully.',
      });
      emit('newId', id);
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
      console.error('Error creating team:', error);

      throw error;
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
        type: 'number',
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
      <a-select
        id="inputCreateTeamBusinessUnit"
        v-model:value="dynamicValidateForm.businessUnit"
        placeholder="Business Unit"
        :disabled="dynamicValidateForm.inputsDisabled"
        show-search
        :filter-option="filterOption"
      >
        <a-select-option
          v-for="bu in getBusinessUnits"
          :key="bu.id"
          :value="bu.id"
          :name="bu.businessUnitName"
        >
          {{ bu.businessUnitName }}
        </a-select-option>
      </a-select>
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
</template>

<style scoped>
  .column {
    margin: 0;
  }
</style>
