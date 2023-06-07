<script setup lang="ts">
import router from '@/router';
import { computed, onMounted, ref } from 'vue';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import type Box from '@/models/Box';
import type UnattachedItem from '@/models/UnattachedItem';
import { BoxEvents } from '@/services/eventservice';
import ConfirmPopup from 'primevue/confirmpopup';
import Toast from 'primevue/toast'
import Header from '@/components/Header.vue'
import SectionTitle from '@/components/SectionTitle.vue'
import BoxAccordion from '@/components/BoxAccordion.vue';
import UnattachedItemAccordion from '@/components/UnattachedItemAccordion.vue';
import EventBus from '@/services/eventbus';
import BoxService from '@/services/boxservice';

let boxes = ref<Box[]>([]);
let unattachedItems = ref<UnattachedItem[]>([]);
let filteredBoxes = ref();
const boxName = ref("");
const boxNumber = ref(99);
const loadingBoxes = ref(false);
const loadingUnattachedItems = ref(false);
const displayCreateBoxDialog = ref(false)
const searchQuery = ref("");
const currentCollectionId = ref("");

async function getBoxes(showLoading: boolean) {
  if(showLoading){
    loadingBoxes.value = true;
  }

  BoxService.getBoxCollection(currentCollectionId.value)
    .then((response) => {
      boxes.value = response.data.boxes
      filteredBoxes.value = response.data.boxes;
    })
    .then(() => {
      loadingBoxes.value = false});
}

async function getUnattachedItems(showLoading: boolean) {
  if(showLoading){
    loadingUnattachedItems.value = true;
  }
  BoxService.getUnattachedItems(currentCollectionId.value)
  .then((response) => unattachedItems.value = response.data.unattachedItems)
  .then(() => loadingUnattachedItems.value = false)
}

async function createNewBox() {
  BoxService.createBox(currentCollectionId.value, boxNumber.value, boxName.value)
    .then(closeDisplayCreateBoxDialog)
}

onMounted(async () => {
  currentCollectionId.value = router.currentRoute.value.params.collectionId as string;
  getBoxes(true);
  getUnattachedItems(true);
});

/* Events */
EventBus.on(BoxEvents.ADDED,  () => {  
      getBoxes(false);
});

EventBus.on(BoxEvents.DELETED,  () => { 
      getBoxes(false);
});

EventBus.on(BoxEvents.ITEM_CHANGED,  () => { 
      getBoxes(false);
});

EventBus.on(BoxEvents.UNATTACHED_ITEMS_CHANGED,  () => { 
    getUnattachedItems(false);
    getBoxes(false);
});

function closeDisplayCreateBoxDialog() {
  displayCreateBoxDialog.value = false;
  console.log(displayCreateBoxDialog);
}

function openDisplayCreateBoxDialog() {
  displayCreateBoxDialog.value = true;
  boxName.value = "";
  boxNumber.value = getlowestFreeBoxNumber();

}

function getlowestFreeBoxNumber(){
  let currentNumbers = boxes.value.map(box => box.number);
  let result = 1;
  
  while(true && result < 300){
    if(currentNumbers.find(number => number == result)){
      result++;
    }
    else {
      return result;   
    }
  }
  return 0; // just a faillback
}


const filterBoxes = computed(() => boxes.value.filter((box) => !searchQuery.value || box.items.some((item: any) => item.name.toLowerCase().includes(searchQuery.value.toLowerCase()))));

function clearFilter() {
  searchQuery.value = "";
}

function trimString(maxLength: number, text: string) {
  return text.length > maxLength ? text.substring(0, maxLength - 3) + "..." : text;
}

</script>
<template>
<Header />
<div class="wrapper">
  <div class="container">
    
    <div class="searchbar">
    <span class="p-input-icon-right p-input-icon-left searchinput">
        <InputText class="searchinput" type="text" v-model="searchQuery" placeholder="Type something to start filtering items" />
        <i v-if=searchQuery style="width: 10px;" class="pi pi-times" @click="clearFilter()" />
        <i v-else class="pi pi-search" style="width: 10px;" />
      </span>
    
    </div>
    <div class="content">
      <div class="c-boxcollection">
        <div class="sectiontitle">
          <SectionTitle title="My collection" >
            <template #right>
              <Button size="small" style="font-size: 10px; background-color: white; color: #181F1C;"  
              icon="pi pi-plus" text outlined raised rounded aria-label="Filter" @click="openDisplayCreateBoxDialog"/>
            </template>
          </SectionTitle>
        </div>
        <div class="c-bc-boxes">
          <BoxAccordion v-if="loadingBoxes" :box="Object()" v-for="box in Array(4)" :isLoading="loadingBoxes"/>
          <BoxAccordion v-else :searchQuery="searchQuery" :box="box" v-for="box in filterBoxes"  :alwaysExpandedItems="false"/>
        </div>
      </div>
      <div class="c-unattacheditems">
        <div class="sectiontitle">
          <SectionTitle title="Unattached items" >
            <template #right>
              <i class="pi pi-info-circle" 
              v-tooltip.top="{ value: `<p>Here is a list 
                of all the items that you have taken out (removed) from a box. You can easily add any one of them item back 
                into the box it was previously placed in!</p>`, escape: true, class: 'custom-error' }" 
                style="font-size: 18px; line-height: 40px; color: white;"/>
            </template>
          </SectionTitle>
        </div>
      </div>
    </div>
  </div>
</div>


<Dialog v-model:visible="displayCreateBoxDialog" :style="{ width: '450px' }" header="Create new box" :modal="true">
         <div class="card">
          <p style="font-size:10px; margin-bottom: 10px;"> A box must have a name, for example "Kitchen stuff", and a unique number larger than 0. We've filled in the lowest unoccupied number for you, but feel free to change it! </p>
          <div class="field">
            <InputText v-model="boxName" type="text" placeholder="Name" />
          </div>
          <div class="field">
            <InputNumber v-model="boxNumber" :min="0" :max="100" placeholder="2" />
          </div>
          <Button @click="createNewBox" type="submit" label="Create new box" class="mt-2" />
        </div>
        <template #footer>
          <Button label="Close" icon="pi pi-times" class="p-button-text" @click="closeDisplayCreateBoxDialog" />
        </template>
      </Dialog>
</template>

<style scoped lang="scss">
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300&display=swap');


.container {
  width: 80%;
  max-width: 1250px;  
  min-width: 350px;
  min-height: 500px;
  margin: auto;
  padding: 10px;
  border: 1px solid;
  border-color: aqua;
  
}

.searchbar {
  display: flex;
  width: 100%;
  margin-left: auto;
  margin-right: auto;
  height: 50px;
  margin-bottom: 10px;

  border: 1px solid;
  border-color: chartreuse;
  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
  background-color: white;
  border-radius: 3px;
}

.searchinput{
  width: 100%;
  height: 100%;
}

.content{
  width: 100%;
  height: 100%;
  border: 1px solid;
  border-color: coral;
  @media (min-width: 1000px) {
    display: flex;
  }
}

.c-boxcollection {
  display: flex;
  flex-direction: column;
  width: 100%;
  margin-left: auto;
  margin-right: auto;
  border: 1px solid;
  border-color: gold;
  @media (min-width: 1000px) {
    width: 65%;
  }
  @media (min-width: 1400px) {
    width: 70%;
  }
}

.sectiontitle{
  width: 100%;
}

.c-bc-boxes {
  width: 100%;
}

.c-unattacheditems {
  width: 100%;
  margin-left: auto;
  margin-right: auto;
  border: 2px solid;
  @media (min-width: 1000px) {
    width: 35%;
  }
  @media (min-width: 1400px) {
    width: 30%;
  }
}

</style>
