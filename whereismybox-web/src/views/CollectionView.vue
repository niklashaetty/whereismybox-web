<script setup lang="ts">
import router from '@/router';
import { computed, onMounted, ref } from 'vue';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import DataView from 'primevue/dataview'
import Menu from 'primevue/menu';
import type Box from '@/models/Box';
import type UnattachedItem from '@/models/UnattachedItem';
import { BoxEvents } from '@/services/eventservice';
import Header from '@/components/Header.vue'
import SectionTitle from '@/components/SectionTitle.vue'
import BoxAccordion from '@/components/BoxAccordion.vue';
import QrStickersView from '@/views/QrStickersView.vue';
import UnattachedItemAccordion from '@/components/UnattachedItemAccordion.vue';
import EventBus from '@/services/eventbus';
import BoxService from '@/services/boxservice';
import UserService from '@/services/userservice';
import CollectionService from '@/services/collectionservice';
import type Contributor from '@/models/Contributor';
import { useLoggedInUserStore } from '@/stores/loggedinuser'
import { usePaperizer } from 'paperizer'
import { useToast } from "primevue/usetoast";
import {  AxiosError } from 'axios'

const boxes = ref<Box[]>([]);
const contributors = ref<Contributor[]>([]);
const contributorsLoaded = ref(false);
const collectionMetadataLoaded = ref(false);
const unattachedItems = ref<UnattachedItem[]>([]);
const filteredBoxes = ref();
const boxName = ref("");
const newContributorUsername = ref("");
const currentCollectionName = ref("");
const boxNumber = ref(99);
const loadingBoxes = ref(false);
const loadingUnattachedItems = ref(false);
const displayCreateBoxDialog = ref(false)
const displayManageCollectionAccessDialog = ref(false)
const displayPrintableQrStickersDialog = ref(false)
const searchQuery = ref("");
const currentCollectionId = ref("");
const currentCollectionOwner = ref("");
const disableAddContributor = ref(false);
const unauthorizedAccess = ref(false);

const { paperize } = usePaperizer('qr-stickers-view', {
  styles: [
    '/testx.css',
  ]
})
const print = () => {
  paperize()
}

const toast = useToast();
const loggedInUserStore = useLoggedInUserStore();

const menu: any = ref(null);
const menuItemsForOwner = ref([
    {
        label: 'Create box',
        icon: 'pi pi-plus',
        command: () => openDisplayCreateBoxDialog()      
    },
    {
        label: 'Share collection',
        icon: 'pi pi-share-alt',
        command: () => openManageCollectionAccessDialog()      
    },
    {
        label: 'QR sticker',
        icon: 'pi pi-qrcode',
        command: () => openPrintableQrStickersDialog()      
    },
]);

const menuItemsForContributor = ref([
    {
        label: 'Create box',
        icon: 'pi pi-plus',
        command: () => openDisplayCreateBoxDialog()      
    },
    {
        label: 'QR sticker',
        icon: 'pi pi-qrcode',
        command: () => openPrintableQrStickersDialog()      
    },
]);

function toggleBoxMenu(event: MouseEvent)  { 
  menu.value?.toggle(event);
}

async function getCurrentUserInformation() {
  if(!loggedInUserStore.username){
    await UserService.getRegisteredUser();
  }
}

async function getBoxes(showLoading: boolean) {
  if(showLoading){
    loadingBoxes.value = true;
  }

  BoxService.getBoxCollection(currentCollectionId.value)
    .then((response) => {
      boxes.value = response.data.boxes
      filteredBoxes.value = response.data.boxes;
      loadingBoxes.value = false;
    })
    .catch((error: AxiosError) => {
      if(error.status === 403) {
        unauthorizedAccess.value = true;
        loadingBoxes.value = false;
        loadingUnattachedItems.value = false;
        toast.add({ severity: 'error', summary: 'Unauthorized', detail: `You do not have access to this collection`, life: 5000 });
      }
    });
}

async function getUnattachedItems(showLoading: boolean) {
  if(showLoading){
    loadingUnattachedItems.value = true;
  }
  BoxService.getUnattachedItems(currentCollectionId.value)
  .then((response) => unattachedItems.value = response.data.unattachedItems)
  .then(() => loadingUnattachedItems.value = false)
}

async function getCollectionMetadata(collectionId : string) {

  CollectionService.getCollection(collectionId)
  .then((response) => {
    currentCollectionName.value = response.data.name
    currentCollectionOwner.value = response.data.owner
  })
  .then(() => collectionMetadataLoaded.value = true);
}

async function createNewBox() {
  BoxService.createBox(currentCollectionId.value, boxNumber.value, boxName.value)
    .then(closeDisplayCreateBoxDialog)
    .then(() => toast.add({ severity: 'success', summary: 'Created', detail: `Box ${boxName.value} created`, life: 3000 }))
}

onMounted(async () => {
  currentCollectionId.value = router.currentRoute.value.params.collectionId as string;
  getBoxes(true);
  getCollectionMetadata(currentCollectionId.value);
  getUnattachedItems(true);
  getCurrentUserInformation();
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

function isMyCollection(){
  return loggedInUserStore.userId === currentCollectionOwner.value;
}

function isSharedCollection(){
  return loggedInUserStore.userId === currentCollectionOwner.value;
}


function closeDisplayCreateBoxDialog() {
  displayCreateBoxDialog.value = false;
}

function openDisplayCreateBoxDialog() {
  displayCreateBoxDialog.value = true;
  boxName.value = "";
  boxNumber.value = getlowestFreeBoxNumber();
}

function openPrintableQrStickersDialog(){
  displayPrintableQrStickersDialog.value = true;
}

function closePrintableQrStickersDialog(){
  displayPrintableQrStickersDialog.value = false;
}

function openManageCollectionAccessDialog() {
  getContributors();
  displayManageCollectionAccessDialog.value = true;
}

function closeManageCollectionAccessDialog() {
  contributorsLoaded.value = false;
  contributors.value = [];
  displayManageCollectionAccessDialog.value = false;
  newContributorUsername.value = "";
  disableAddContributor.value = false;
}

function getContributors() {
  UserService.getCollectionContributors(currentCollectionId.value)
  .then((c) => contributors.value = c.data)
  .then(() => contributorsLoaded.value = true)
}

function removeContributor(userId: string, collectionId: string) {
  UserService.deleteCollectionContributor(userId, collectionId)
  .then(getContributors)
}

function addContributor(username: string, collectionId: string) {
  disableAddContributor.value = true;
  UserService.addCollectionContributor(username, collectionId)
  .then(getContributors)
  .then(() => {
    disableAddContributor.value = false;
    newContributorUsername.value = "";
  })
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

function pushToDashboard(){
  router.push({path: `/dashboard`});
}

</script>
<template>
<Header />
<div class="wrapper">
  <div class="container">
    <div v-show="unauthorizedAccess" class="unauthorized-info">
      <Button style="background: white; margin: auto;" severity="success"  text raised icon="pi pi-home" label="Go to dashboard" @click="pushToDashboard()"/>
    </div>
    
    <div v-if="!unauthorizedAccess" class="searchbar">
    <span class="p-input-icon-right p-input-icon-left searchinput">
        <InputText class="searchinput" type="text" v-model="searchQuery" placeholder="Type something to start filtering items" />
        <i v-if=searchQuery class="pi pi-times boxie-icon clickable" @click="clearFilter()" />
        <i v-else class="pi pi-search boxie-icon" />
      </span>
    
    </div>
    <div v-if="!unauthorizedAccess" class="content">
      <div class="c-boxcollection">
        <div class="sectiontitle">
          <SectionTitle v-show="!unauthorizedAccess" v-if="collectionMetadataLoaded && isMyCollection()" :title="`My collection ` + currentCollectionName "  >
            <template #right>
              <i style="margin-right: 10px;" class="pi pi-cog boxie-icon clickable" @click="toggleBoxMenu($event)" />
              <Menu id="overlay_menu" ref="menu" :model="menuItemsForOwner" :popup="true" />
            </template>
          </SectionTitle>
          <SectionTitle v-show="!unauthorizedAccess" v-if="collectionMetadataLoaded && !isMyCollection()" :title="`Shared collection ` + currentCollectionName " >
            <template #right>
              <i style="margin-right: 10px;" class="pi pi-cog boxie-icon clickable" @click="toggleBoxMenu($event)" />
              <Menu id="overlay_menu" ref="menu" :model="menuItemsForContributor" :popup="true" />
            </template>
          </SectionTitle>
          <SectionTitle v-show="!unauthorizedAccess && !collectionMetadataLoaded" title="" />
        </div>
        <div class="c-bc-boxes">
          <BoxAccordion v-if="loadingBoxes" :box="Object()" v-for="box in Array(4)" :isLoading="loadingBoxes"/>
          <BoxAccordion :searchQuery="searchQuery" :box="box" v-for="box in filterBoxes"  :alwaysExpandedItems="false"/>
          <div v-show="!unauthorizedAccess && !loadingBoxes && boxes.length === 0" class="c-bc-boxes-empty"> 
            <h3 class="c-bc-boxes-empty-text">This is your collection of boxes. </h3>
            <h3 class="c-bc-boxes-empty-text"> When you create your first box, it'll show up here!</h3>
          </div>
        </div>
      </div>
      <div class="c-unattacheditems">
        <div class="sectiontitle">
          <SectionTitle v-if="!unauthorizedAccess" title="Unattached items" >
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
    
  </div>
  <template #footer>
    <Button severity="success" text icon="pi pi-plus" label="Create new box" type="submit"  @click="createNewBox" />
    <Button severity="secondary" text icon="pi pi-times" label="Close" @click="closeDisplayCreateBoxDialog" />
  </template>
</Dialog>
<Dialog v-model:visible="displayPrintableQrStickersDialog" :style="{ width: '650px' }" header="Printable QR stickers" :modal="true">
  
  <div id="qr-stickers-view">
    <QrStickersView :collectionId="currentCollectionId" :boxes="boxes"/>'
  </div>
  <template #footer>
    <Button severity="success" label="Print" icon="pi pi-print" class="p-button-text" @click="print()" />
    <Button severity="secondary" label="Close" icon="pi pi-times" class="p-button-text" @click="closePrintableQrStickersDialog" />
  </template>
</Dialog>

<Dialog v-model:visible="displayManageCollectionAccessDialog" :style="{ width: '650px' }" header="Share collection" :modal="true">
  <div class="card">
    <p style="margin-bottom: 10px;"> Once a collection is shared with someone, they can access and edit the collection until you revoke their access here.</p>
    
    <DataView dataKey="userId" v-show="contributorsLoaded" :value="contributors">
            <template #header>
              Collection shared with
            </template>
            <template #list="slotProps">
                <div class="col-12">
                    <div class="flex flex-column xl:flex-row xl:align-items-start p-2 gap-1">
                        <div class="flex flex-column sm:flex-row justify-content-between align-items-center xl:align-items-start flex-1 gap-1">
                            <div class="flex flex-column align-items-center sm:align-items-start gap-1">
                                <div class="text-l text-1000">{{ slotProps.data.username }}</div>
                            </div>
                            <div class="flex sm:flex-column align-items-center sm:align-items-end gap-1 sm:gap-1">
                                <Button @click="removeContributor(slotProps.data.userId, currentCollectionId)" p-button-sm icon="pi pi-times" rounded outlined  severity="danger"></Button>
                            </div>
                        </div>
                    </div>
                </div>
            </template>
        </DataView>
  </div>
  <div style="margin-top: 30px;display: flex;">   
        <span class="p-float-label">
          <InputText v-show="!disableAddContributor" id="username" v-model="newContributorUsername" />
          <InputText disabled v-show="disableAddContributor" id="username" v-model="newContributorUsername" />
          <label v-show="!disableAddContributor" for="username">Username</label>
        </span>
        <Button severity="plain" text raised icon="pi pi-share-alt" label="Share" style="margin-left: 0.5em" v-show="!disableAddContributor" @click="addContributor(newContributorUsername, currentCollectionId)"/>
        <Button severity="plain" text raised icon="pi pi-spin pi-spinner" label="Share" style="margin-left: 0.5em" disabled v-show="disableAddContributor"/>

      </div> 
  <template #footer>
    <Button label="Close" icon="pi pi-times" class="p-button-text" @click="closeManageCollectionAccessDialog" />
  </template>
</Dialog>
</template>

<style scoped lang="scss">
@import 'primeflex/primeflex.scss';
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300&display=swap');

.unauthorized-info{
  width: 100%;
  margin: auto;
  margin-top: 100px;
}

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

.collection-sharee{
  display: flex;
  height: 30px;
  line-height: 30px;
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
