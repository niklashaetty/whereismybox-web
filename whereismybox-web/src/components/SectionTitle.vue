<script setup lang="ts">

import {PrimeIcons} from 'primevue/api';
import { defineProps, computed, ref, onMounted} from 'vue'
import router from '@/router';


const props = defineProps({
  title: {
      type: String,
      required: true
    }
  }
);
const userId = router.currentRoute.value.params.userId as string;
const title = computed(() => props.title);

</script>

<template>
<div class="titlecontainer">
  <i class="pi pi-angle-right" style="color: slateblue; padding-right: 5px;"></i>
  <p> {{ title }}</p>
</div>
</template>

<style scoped>
.titlecontainer{
  align-items: center;
  display: flex;
  height: 40px;
  font-size: 18px;
  padding-left: 10px;
  min-width: 200px;
}



</style>
