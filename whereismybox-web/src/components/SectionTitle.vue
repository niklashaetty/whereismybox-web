<script setup lang="ts">

import { computed} from 'vue'

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
  <div class="title">
    <p>// {{ title }}</p>
  </div>
  <div class="right-content">
    <slot class="right-slot" name="right"></slot>
  </div>
  
  
</div>
</template>

<style scoped lang="scss">
.titlecontainer{
  padding-left: 5px;
  color: white;
  align-items: center;
  display: flex;
  height: 30px;
  font-size: 20px;
  width:100%;
  @media (min-width: 500px) {
    height: 40px;
  }
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
