<script setup lang="ts">
import ConfirmDialog from 'primevue/confirmdialog';
import Toast from 'primevue/toast'
import { RouterView } from 'vue-router';
</script>

<template>
  <Toast />
  <RouterView />
  <ConfirmDialog />
</template>

<style scoped>
</style>
