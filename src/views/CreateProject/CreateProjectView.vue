<script setup lang="ts">
  import { computed, defineProps, inject, ref, watch } from 'vue';
  import {
    BankOutlined,
    FontColorsOutlined,
    ShoppingOutlined,
    TeamOutlined,
    UserOutlined,
  } from '@ant-design/icons-vue';
  import { reactive } from 'vue';
  import type { UnwrapRef } from 'vue';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import type { CreateProjectModel } from '@/models/Project';

  // Props definieren
  const props = defineProps({
    open: {
      type: Boolean,
      default: false,
    },
  });

  // Modal-Status von externem Prop steuern
  const isOpen = ref(props.open);
  watch(
    () => props.open,
    (newVal) => {
      isOpen.value = newVal;
    },
  );

  // Formular- und Zustandskonfiguration
  const formRef = ref();
  const labelCol = { style: { width: '150px' } };
  const wrapperCol = { span: 14 };
  const cancelFetch = ref<boolean>();

  // TableStore für das Aktualisieren der Tabelle nach Hinzufügen eines Projekts
  const projectsStore = inject(projectsStoreSymbol);
  const isAdding = computed(() => projectsStore?.getIsLoadingAdd);
  const fetchError = ref<boolean>(false);

  const formState: UnwrapRef<CreateProjectModel> = reactive({
    projectName: '',
    businessUnit: '',
    teamNumber: undefined,
    department: '',
    clientName: '',
  });

  const validateMessages = {
    required: 'Please enter valid input.',
    types: {
      number: 'Team number is not a valid number!',
    },
    number: {
      range: 'Team number must be positive number.',
    },
  };

  // Modal zurücksetzen
  const resetModal = () => {
    formRef.value.resetFields();
    fetchError.value = false;
    isOpen.value = false;
  };

  // Eingaben validieren und dann absenden
  const handleOk = () => {
    cancelFetch.value = false;
    formRef.value
      .validate()
      .then(() => {
        submit();
      })
      .catch((error: unknown) => {
        console.log('error', error);
      });
  };

  // Projekt anlegen und nach erfolgreicher Validierung Modal schließen
  const submit = async () => {
    watch(isAdding, (newVal) => {
      if (newVal === false) {
        if (projectsStore?.getAddedSuccessfully) {
          projectsStore.fetchProjects();
          fetchError.value = false;
          resetModal();
        } else {
          fetchError.value = true;
        }
      }
    });

    const projectData: CreateProjectModel = {
      projectName: formState.projectName,
      businessUnit: formState.businessUnit,
      teamNumber: formState.teamNumber,
      department: formState.department,
      clientName: formState.clientName,
    };

    await projectsStore?.addProject(projectData);
  };
</script>

<template>
  <!-- Modal für die Projekterstellung -->
  <a-modal
    v-model:open="isOpen"
    width="400px"
    title="Create Project"
    :ok-button-props="{ disabled: isAdding }"
    @ok="handleOk"
    @cancel="resetModal"
  >
    <a-form
      ref="formRef"
      :model="formState"
      :validate-messages="validateMessages"
      :label-col="labelCol"
      :wrapper-col="wrapperCol"
    >
      <a-form-item
        name="projectName"
        :rules="[{ required: true, whitespace: true }]"
        class="column"
        :no-style="true"
      >
        <a-input
          v-model:value="formState.projectName"
          class="inputField"
          placeholder="Project Name"
        >
          <template #prefix>
            <FontColorsOutlined />
          </template>
        </a-input>
      </a-form-item>
      <a-form-item
        name="businessUnit"
        :rules="[{ required: true, whitespace: true }]"
        :no-style="true"
      >
        <a-input
          v-model:value="formState.businessUnit"
          class="inputField"
          placeholder="Business Unit"
        >
          <template #prefix>
            <ShoppingOutlined />
          </template>
        </a-input>
      </a-form-item>
      <a-form-item
        name="teamNumber"
        :rules="[{ required: true }, { type: 'number', min: 0 }]"
        :no-style="true"
      >
        <a-input-number
          v-model:value="formState.teamNumber"
          class="inputField"
          placeholder="Team Number"
        >
          <template #prefix>
            <TeamOutlined />
          </template>
        </a-input-number>
      </a-form-item>
      <a-form-item
        name="department"
        :rules="[{ required: true, whitespace: true }]"
        :no-style="true"
      >
        <a-input
          v-model:value="formState.department"
          class="inputField"
          placeholder="Department"
        >
          <template #prefix>
            <BankOutlined />
          </template>
        </a-input>
      </a-form-item>
      <a-form-item
        name="clientName"
        :rules="[{ required: true, whitespace: true }]"
        :no-style="true"
      >
        <a-input
          v-model:value="formState.clientName"
          class="inputField"
          placeholder="Client Name"
        >
          <template #prefix>
            <UserOutlined />
          </template>
        </a-input>
      </a-form-item>
      <!-- Error-Meldung bei Fehlschlag -->
      <a-alert
        v-if="fetchError"
        message="Failed to create Project"
        type="error"
        show-icon
      ></a-alert>
    </a-form>
  </a-modal>
</template>

<style scoped lang="scss">
  .formItem {
    max-width: none !important;
  }

  .inputField {
    width: 100%;
    margin: 10px 0 10px 0;
  }
</style>
