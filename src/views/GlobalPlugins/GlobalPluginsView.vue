<template>
  <a-list
    item-layout="horizontal"
    :data-source="data.filter((item) => !item.archieved)"
  >
    <template #renderItem="{ item }">
      <a-list-item>
        <a-list-item-meta>
          <template #title>
            <div class="list">
              <div class="title">
                {{ item.name }}
              </div>
              <div class="buttons">
                <a-button style="margin-right: 1em">
                  <EditOutlined />
                </a-button>
                <a-button>
                  <DeleteOutlined />
                </a-button>
              </div>
            </div>
          </template>
        </a-list-item-meta>
      </a-list-item>
    </template>
  </a-list>
  <FloatingButton :button="button" />
</template>
<script lang="ts" setup>
  import {
    EditOutlined,
    DeleteOutlined,
    PlusOutlined,
  } from '@ant-design/icons-vue';
  import type { GlobalPluginModel } from '@/models/Plugin';
  import type { FloatButtonModel } from '@/components/Button';

  const data: GlobalPluginModel[] = [
    {
      id: 0,
      name: 'Plugin 1',
      archieved: false,
    },
    {
      id: 1,
      name: 'Plugin 2',
      archieved: false,
    },
    {
      id: 2,
      name: 'Plugin 3',
      archieved: false,
    },
    {
      id: 3,
      name: 'Plugin 4',
      archieved: false,
    },
  ];

  const button: FloatButtonModel = {
    name: 'CreatePluginButton',
    onClick: () => {},
    icon: PlusOutlined,
    status: 'activated',
    tooltip: 'Click here to create a new project',
  };
</script>

<style scoped>
  .list {
    display: flex;
  }

  .buttons {
    margin-left: auto;
  }

  .title {
    align-items: center;
  }
</style>
