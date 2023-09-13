<script setup lang="ts">
import router from '@/router';
import axios from 'axios';
import { onMounted, ref } from 'vue';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import type Box from '@/models/Box';
import type Item from '@/models/Item';
import type UnattachedItem from '@/models/UnattachedItem';
import { BoxEvents } from '@/services/eventservice';
import ConfirmPopup from 'primevue/confirmpopup';
import Toast from 'primevue/toast'
import Header from '@/components/Header.vue'
import SectionTitle from '@/components/SectionTitle.vue'
import BoxAccordion from '@/components/BoxAccordion.vue';
import EventBus from '@/services/eventbus';
import BoxService from '@/services/boxservice';

let box = ref<Box>(Object())
const boxName = ref("");
const collectionId = router.currentRoute.value.params.collectionId as string;
const boxId = router.currentRoute.value.params.boxId as string;
const loadingBox = ref(true);

let filteredItems = ref<Item[]>([]);
const filter = ref("");

async function getBox(showLoading: boolean) {
  if(showLoading){
    loadingBox.value = true;
  }
  BoxService.getBox(collectionId, boxId)
    .then((response) => {
      box.value = response.data
      filteredItems.value = response.data.items;
      boxName.value = response.data.boxName;
    })
    .then(() => {
      loadingBox.value = false});
}


onMounted(async () => {
  getBox(true);
});

/* Events */

EventBus.on(BoxEvents.ITEM_CHANGED,  () => { 
      getBox(false);
});


const filterBoxes = () => box.value?.items.filter((item) => !filter.value || item.name.toLowerCase().includes(filter.value.toLowerCase()))

function clearFilter() {
  filter.value = "";
}


function trimString(maxLength: number, text: string) {
  return text.length > maxLength ? text.substring(0, maxLength - 3) + "..." : text;
}

</script>
<template>
<Header />
<div class="container">

  <Toast/>
  <div class="searchbar">
    <SectionTitle title="Search" />
    
    <span class="p-input-icon-right p-input-icon-left testt">
        <InputText class="searchinput" type="text" v-model="filter" placeholder="Type something to start filter" />
        <i v-if=filter style="width: 10px;" class="pi pi-times" @click="clearFilter()" />
        <i v-else class="pi pi-search" style="width: 10px;" />
    </span>

  </div>
  <div class="boxes">
    
    <div class="accordioncontainer">
      <BoxAccordion v-if="loadingBox" :box="Object()" v-for="box in Array(1)" :isLoading="loadingBox"/>
      <BoxAccordion v-else :box="box" :alwaysExpandedItems="true" />
    </div>
  </div>
</div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300&display=swap');


.myboxestitle{
  display: flex;
}


.container {  
  margin: auto;
  min-height: 1000px;
  max-width: 800px;;
  
}

.boxes { 
  margin-top: 50px;
  grid-area: boxes;

}

.testt{
  width: 100%;
  height: 50px;
  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
  background-color: white;
  border-radius: 3px;
}

.searchinput {
  width: 100%;
  height: 50px;
  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
  background-color: white;
  border-radius: 3px;
}

</style>
