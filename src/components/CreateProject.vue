<script lang="ts" setup>
import { ref } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
const open = ref<boolean>(false);

const businessUnit = ref<string>('');
const teamNumber = ref<string>('');
const department = ref<string>('');
const clientName = ref<string>('');

const businessUnitStatus = ref<string>('');
const teamNumberStatus = ref<string>('');
const departmentStatus = ref<string>('');
const clientNameStatus = ref<string>('');

const showModal = () => {
  open.value = true;
};

const validateField = (fieldValue: string, fieldStatus: { value: string }) => {
  if (!fieldValue) {
    fieldStatus.value = 'error';
  } else {
    fieldStatus.value = '';
  }
}

const handleOk = () => {
  validateField(businessUnit.value, businessUnitStatus);
  validateField(teamNumber.value, teamNumberStatus);
  validateField(department.value, departmentStatus);
  validateField(clientName.value, clientNameStatus);

  if(businessUnit.value && teamNumber.value && department.value && clientName.value){
    open.value = false;
  }

};
</script>

<template>
  <div>
    <a-float-button @click="showModal">
      <template #icon>
        <PlusOutlined />
      </template>
    </a-float-button>
    <a-modal v-model:open="open" width="400px" title="Create Project" @ok="handleOk">
      <a-space direction="vertical" class="space">
        <a-input id="businessUnitField" class="inputField" :status="businessUnitStatus" v-model:value="businessUnit" placeholder="Business Unit"/>
        <a-input id="teamNumberField" class="inputField" :status="teamNumberStatus" v-model:value="teamNumber" placeholder="Team Number" />
        <a-input id="departmentField" class="inputField" :status="departmentStatus" v-model:value="department" placeholder="Department" />
        <a-input id="clientNameField" class="inputField" :status="clientNameStatus" v-model:value="clientName" placeholder="Client Name" />
      </a-space>
    </a-modal>
  </div>
</template>

<style scoped lang="scss">
.space {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  & > * {
    width: 100%;
  }
}

.inputField {
  width: 90%;


}
</style>
