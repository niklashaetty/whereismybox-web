<script setup lang="ts">

import {computed} from 'vue'
import QrcodeVue from 'qrcode.vue'

const props = defineProps({
  boxNumber:  {
      type: Number,
      required: true
  },
  qrCodeLink: {
    type: String,
    required: true
  },
  title: {
    type: String,
    required: true
  },
}
);
const boxNumber = computed(() => props.boxNumber);
const qrCodeLink = computed(() => props.qrCodeLink);
const title = computed(() => props.title);

</script>

<template>
<div class="sticker-container">
    <h1 class="number"> {{ boxNumber }} </h1>
    <h1 class="title"> {{ title }} </h1>
    <qrcode-vue class="qrcode" style="margin: auto" :size="125" :value="qrCodeLink" render-as="svg"></qrcode-vue>
</div>
</template>

<style scoped>
.sticker-container{
  width: 300px;
  height: 300px;
  background: white;
  border: 1px solid;
  border-color: black;
  border: 5px solid;
  padding: 20px;
  background-color: #a0ac95;
  margin: auto;
}

.number {
  display:table; 
  line-height: 1;
  margin: auto;
  color: #f2f2f2;
  font-size: 80px;
}
.title {
  display:table; 
  margin: auto;
  color: #f2f2f2;
  font-size: 30px;
}

.qrcode {
  display: flex;
  margin: auto;
  border: 2px solid;
  border-color: white;
  padding: 5px;
  border-radius: 3px;

  background-color: white;
}

</style>
