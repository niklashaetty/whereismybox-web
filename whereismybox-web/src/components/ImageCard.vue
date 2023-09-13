<script setup lang="ts">
import { computed, onMounted, watch} from 'vue'

import router from '@/router';
import Button from 'primevue/button';
import Menu from 'primevue/menu';
import BoxService from '@/services/boxservice';
import { ref } from 'vue';
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";
import ItemAccordion from '@/components/ItemAccordion.vue'
import type Item from '@/models/Item';
import Box from '@/models/Box';
import ItemAccordionDescription from '@/components/ItemAccordionDescription.vue'
import Sticker from '@/components/Sticker.vue'
import Skeleton from 'primevue/skeleton';
import Dialog from 'primevue/dialog'

const props = defineProps(
  {
    imgpath:  {
        type: String,
        required: true
    },
    title: {
      type: String,
      required: true
    },
    text: {
      type: String,
      required: true,
      default: false
    }
  }
);

const imagePath = computed(() => props.imgpath);
const title = computed(() => props.title);
const text = computed(() => props.text);
</script>

<template>
  <div class="imagecard-container"> 
    <div class="ic-image">
      <img class="ic-image-image" src="../assets/screenshots/collection_full_size.png" > 
    </div>
    <div class="ic-content">
      <p class="ic-content-title">{{ title }}</p>
      <p class="ic-content-text">{{ text }}</p>
    </div>
  </div>
 
</template>

<style scoped lang="scss">
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300&display=swap');

.ic-image {
  margin-right: auto;
  margin-left: auto;
  width: 261px;
  height: px;
}

.ic-image-image {
  max-width: 100%;
  max-height: 100%;
  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
}

.imagecard-container{
  padding: 10px;
  margin-top: 100px;
  width: 285px;
  height: 400px;
  background-color: #f7faf8;
  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
  border-radius: 3px;
}

.ic-content {
  padding-left: 5px;
  margin-top: 10px;
}

.ic-content-title {
  font-weight: bold;
  font-size: 14px;
  color:  #606060;
}
.ic-content-text {

  font-size: 12px;
}
</style>
