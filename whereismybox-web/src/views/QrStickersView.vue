<script setup lang="ts">
import router from '@/router';
import axios from 'axios';
import { onMounted, ref } from 'vue';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'

import type Item from '@/models/Item';
import type UnattachedItem from '@/models/UnattachedItem';
import { BoxEvents } from '@/services/eventservice';
import ConfirmPopup from 'primevue/confirmpopup';
import Toast from 'primevue/toast'
import Header from '@/components/Header.vue'
import SectionTitle from '@/components/SectionTitle.vue'
import BoxAccordion from '@/components/BoxAccordion.vue';
import UnattachedItemAccordion from '@/components/UnattachedItemAccordion.vue';
import EventBus from '@/services/eventbus';
import Sticker from '@/components/Sticker.vue'
import BoxService from '@/services/boxservice';
import Box from '@/models/Box';

import {computed} from 'vue'

const props = defineProps({
  boxes:  {
      type: Object,
      required: true
  },
  collectionId: {
      type: String,
      required: true
    }
});
const boxes = computed(() => props.boxes);
const collectionId = computed(() => props.collectionId);

function makeLinkToBox(collectionId: string, boxId: string){
  return window.location.origin + "collections/" + collectionId + "/boxes/" +boxId;
}
</script>
<template>
<div class="sticker-containers">
        
        <div class="stickers-content">
          <Sticker v-for="box in boxes" :qrCodeLink="makeLinkToBox(collectionId, box.boxId)" :boxNumber="box.number" :title="box.name" />
        </div>
      </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300&display=swap');


.sticker-containers {
    display: flex;
    margin-top: 10mm;
    flex-direction: column;
    justify-content: flex-start;
    flex-wrap: wrap;
  }

</style>
