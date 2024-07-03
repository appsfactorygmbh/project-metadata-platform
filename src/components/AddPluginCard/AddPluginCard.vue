<script setup lang="ts">
    import { ref } from 'vue';
    import FormModal from '../Modal/FormModal.vue';
    import { usePluginsStore } from '@/store';
    import { PluginModel } from '@/models/Plugin';

    const isModalOpen = ref(false);
    const pluginsStore = usePluginsStore();

    // opens the modal
    const openModal = () => {
        isModalOpen.value = true
    }

    // closes the modal
    const closeModal = () => {
        isModalOpen.value = true
    }

    // sends form
    const handleFormSubmit = (plugin: PluginModel) => {
        pluginsStore.createPlugin(plugin);
        closeModal();
    }

    // creates form-object from PluginModel.ts
    const form = ref({
        modelRef: ref<PluginModel>({
            pluginName: '',
            displayName: '',
            url: '' 
        })
    })
</script>

<template>
    <div>
        <button @click="openModal">+</button>
        <FormModal
            v-if="isModalOpen"
            :onSubmit="handleFormSubmit"
            :form='form.modelRef'
        >
        </FormModal>
    </div>
</template>

<style scoped>
    button {
        font-size: 24px;
        padding: 10px 20px;
        cursor: pointer;
    }
</style>