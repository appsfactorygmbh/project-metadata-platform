<script lang="ts" setup>
import { ref } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
const open = ref<boolean>(false);

const businessUnit = ref<string>('');
const teamNumber = ref<string>('');
const department = ref<string>('');
const clientName = ref<string>('');

const showModal = () => {
  open.value = true;
};

const handleOk = (e: MouseEvent) => {
  if(businessUnit.value && teamNumber.value && department.value && clientName.value){


  open.value = false;
  }else{
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
        <a-input class="inputField" v-model:value="businessUnit" placeholder="Business Unit"/>
        <a-input class="inputField" v-model:value="teamNumber" placeholder="Team Number" />
        <a-input class="inputField" v-model:value="department" placeholder="Department" />
        <a-input class="inputField" v-model:value="clientName" placeholder="Client Name" />
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
