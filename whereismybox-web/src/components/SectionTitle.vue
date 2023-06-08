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
  <div class="icon">
    <i class="pi pi-angle-right" style="color: slateblue; padding-right: 5px;"></i>
  </div>
  <div class="title">
    <p> {{ title }}</p>
  </div>
  <div class="right-content">
    <slot class="right-slot" name="right"></slot>
  </div>
  
  
</div>
</template>

<style scoped lang="scss">
.titlecontainer{
  align-items: center;
  display: flex;
  height: 30px;
  font-size: 14px;
  padding-left: 10px;
  width:100%;
  @media (min-width: 500px) {
    font-size: 18px;
    height: 40px;
  }
}

.icon {
  width:20px;
}


.right-content {
  max-width: 100px;
  margin-right: 5px;
  margin-left: auto;
  margin-top: auto;
  margin-bottom: auto;
}

.right-slot {
  height: 30px;
  line-height: 30px;


}
</style>
