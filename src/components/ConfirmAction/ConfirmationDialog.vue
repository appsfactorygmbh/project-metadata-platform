<template>
  <!-- reusable component -->
  <a-modal
    v-model:visible="localIsOpen"
    :title="title"
    :style="{ top: '20px' }"
    ok-text="Yes"
    cancel-text="No"
    @ok="confirm"
    @cancel="cancel"
  >
    <p>{{ message }}</p>
  </a-modal>
</template>

<script>
  export default {
    name: 'ConfirmationDialog',
    props: {
      isOpen: {
        type: Boolean,
        required: true,
      },
      title: {
        type: String,
        required: true,
      },
      message: {
        type: String,
        required: true,
      },
    },
    data() {
      return {
        localIsOpen: this.isOpen, // Local copy of the isOpen prop to manage the modal's visibility
      };
    },
    watch: {
      isOpen(val) {
        this.localIsOpen = val; // Watch for changes in the isOpen prop and update localIsOpen accordingly
      },
      localIsOpen(val) {
        if (!val) {
          this.$emit('update:isOpen', val);
        }
      },
    },
    methods: {
      confirm() {
        this.$emit('confirm');
        this.localIsOpen = false;
      },
      cancel() {
        this.localIsOpen = false;
        this.$emit('cancel');
      },
    },
  };
</script>

<style scoped></style>
