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

const filterBoxes = computed(() => boxes.value.filter((box) => !searchQuery.value || 
box.items.some((item: any) => 
item.name.toLowerCase().includes(searchQuery.value.toLowerCase())
|| item.description.toLowerCase().includes(searchQuery.value.toLowerCase()))));

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
        <i v-if=searchQuery class="pi pi-times boxie-icon clickable" @click="clearFilter()" />
        <i v-else class="pi pi-search boxie-icon" />
      </span>
    
    </div>
    <div class="content">
      <div class="c-boxcollection">
        <div class="sectiontitle">
          <SectionTitle title="My collection" >
            <template #right>
              <i class="pi pi-plus-circle boxie-icon clickable" @click="openDisplayCreateBoxDialog" />
            </template>
          </SectionTitle>
        </div>
        <div class="c-bc-boxes">
          <BoxAccordion v-if="loadingBoxes" :box="Object()" v-for="box in Array(4)" :isLoading="loadingBoxes"/>
          <BoxAccordion v-else :searchQuery="searchQuery" :box="box" v-for="box in filterBoxes"  :alwaysExpandedItems="false"/>
          <div v-show="!loadingBoxes && boxes.length === 0" class="c-bc-boxes-empty"> 
            <h3 class="c-bc-boxes-empty-text">This is your collection of boxes. </h3>
            <h3 class="c-bc-boxes-empty-text"> When you create your first box, it'll show up here!</h3>
          </div>
        </div>
      </div>
      <div class="c-unattacheditems">
        <div class="sectiontitle">
          <SectionTitle title="Unattached items" >
            <template #right>
              <i class="pi pi-info-circle boxie-icon" 
              v-tooltip.top="{ value: `<p>Here is a list 
                of all the items that you have taken out (removed) from a box. You can easily add any one of them item back 
                into the box it was previously placed in!</p>`, escape: true, class: 'custom-error' }"/>
            </template>
          </SectionTitle>
        </div>
        <div class="c-ui-unattacheditems">
          <UnattachedItemAccordion v-if="loadingUnattachedItems" v-for="i in Array(4)"  :unattachedItem="Object()" :isLoading=loadingUnattachedItems >
          <template #name> <Skeleton style="margin:auto;" height="12px"></Skeleton></template>
          </UnattachedItemAccordion>
    
          <!-- Unattached items -->
          <UnattachedItemAccordion v-else v-for="unattachedItem in unattachedItems"  :unattachedItem="unattachedItem">
            <template #name> <p :title="unattachedItem.name"> {{ trimString(40, unattachedItem.name) }}</p></template>
            <template #previousbox> <p> {{ unattachedItem.previousBoxNumber }}</p></template>
          </UnattachedItemAccordion>
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
  width: 95%;
  max-width: 1250px;  
  min-width: 350px;
  min-height: 500px;
  margin: auto;
  padding: 10px;
  @media (min-width: 1000px) {
    width: 80%;
  }
  @media (min-width: 500px) {
    width:90%;
  }
}

.searchbar {
  display: flex;
  width: 100%;
  margin-left: auto;
  margin-right: auto;
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

.searchinput{
  width: 100%;
  height: 100%;
}

.add-new-icon {
  color: white;
}

.content{
  width: 100%;
  height: 100%;
  @media (min-width: 1000px) {
    display: flex;
  }
}

.c-boxcollection {
  display: flex;
  flex-direction: column;
  width: 100%;
  margin-top: 15px;
  margin-left: auto;
  margin-right: auto;
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

.c-bc-boxes-empty{
  width: 100%;
}
.c-bc-boxes-empty-text{
  margin-top: 20px;
  text-align: center;
  color:white;
}

.clickable {
  cursor: pointer;
}

.c-unattacheditems {
  width: 100%;
  margin-top: 15px;
  margin-left: auto;
  margin-right: auto;
  @media (min-width: 1000px) {
    width: 35%;
  }
  @media (min-width: 1400px) {
    width: 30%;
  }
}

.boxie-icon {
  width: 20px;
  height: 20px;
  font-size: 16px;
  text-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
  color: white;
  @media (min-width: 500px) {
    font-size: 20px;
  }
}


</style>
