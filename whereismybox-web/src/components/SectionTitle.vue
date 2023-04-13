<script setup lang="ts">

import { defineProps, computed} from 'vue'

const props = defineProps({
  title: {
      type: String,
      required: true
    }
  }
);
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
