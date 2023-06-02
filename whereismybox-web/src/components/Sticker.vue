<script setup lang="ts">

import { ref, onMounted} from 'vue'
import router from '@/router';

import { defineProps, computed} from 'vue'

import UserService from '@/services/userservice';
import Avatar from 'primevue/avatar';
import { useConfirm } from "primevue/useconfirm";
import QrcodeVue from 'qrcode.vue'

const collectionId = router.currentRoute.value.params.collectionId as string;

const props = defineProps({
  boxNumber:  {
      type: Number,
      required: true
  },
  qrCodeLink: {
    type: String,
    required: true
  },
}
);
const boxNumber = computed(() => props.boxNumber);
const qrCodeLink = computed(() => props.qrCodeLink);

</script>

<template>
<div class="sticker-container">
    <h1 class="number"> {{ boxNumber }} </h1>
    <qrcode-vue class="qrcode" style="margin: auto" :size="100" :value="qrCodeLink"></qrcode-vue>
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
  margin: auto;
  color: #f2f2f2;
  font-size: 80px;

}

.qrcode {
  display: flex;
  margin: auto;
  border: 2px solid;
  border-color: white;
  padding: 5px;
  border-radius: 3px;
  width: 115px;
  height: 115px;
  background-color: white;
}

</style>
