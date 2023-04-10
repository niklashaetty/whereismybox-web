<script setup lang="ts">
import router from '@/router';
import axios from 'axios';
import { onMounted, ref } from 'vue';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Button from 'primevue/button';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Skeleton from 'primevue/skeleton'
import QrcodeVue from 'qrcode.vue'
import Dialog from 'primevue/dialog'
import Menu from 'primevue/menu';
import Item from '@/models/Item';
import type Box from '@/models/Box';
import type User from '@/models/User';
import type UnattachedItem from '@/models/UnattachedItem';
import BoxService from '@/services/boxservice';
import UserService from '@/services/userservice';
import ConfirmPopup from 'primevue/confirmpopup';
import { useToast } from "primevue/usetoast";
import { useConfirm } from "primevue/useconfirm";
import Toast from 'primevue/toast'
import Header from '@/components/Header.vue'
import BoxCard from '@/components/BoxCard.vue'

let boxes = ref<Box[]>([]);
let unattachedItems = ref<UnattachedItem[]>([]);
let filteredBoxes = ref();
const boxName = ref("");
const selectedUnattachedItem = ref("");
const boxNumber = ref(0);
const loadingBoxes = ref(false);
const loadingUnattachedItems = ref(false);
const displayQrCodeDialog = ref(false)
const qrCodeLink = ref("");
const filter = ref("");
const currentUserId = ref("");
const currentUser = ref()

const menu: any = ref(null);
const menuItems = ref([
    {
        label: 'Options',
        items: [{label: 'Add item back to the previous box', icon: 'pi pi-history',
        command: () => {
              addBackUnattachedItem(selectedUnattachedItem.value);
          }
        },
        {label: 'Put item in box', icon: 'pi pi-trash',
            command: () => {
              deleteUnattachedItem(selectedUnattachedItem.value);
            }
        },
        {label: 'Delete item', icon: 'pi pi-trash',
            command: () => {
              deleteUnattachedItem(selectedUnattachedItem.value);
            }
        }
    ]}
]);

const toast = useToast();

async function getBoxes() {
  loadingBoxes.value = true;
  let boxesPath = `/api/users/${currentUserId.value}/boxes`
   axios
    .get(boxesPath)
    .then((response) => {
      boxes.value = response.data
      filteredBoxes.value = response.data;
    })
    .then(() => loadingBoxes.value = false);
}

async function getUnattachedItems() {
  loadingUnattachedItems.value = true;
  let path = `/api/users/${currentUserId.value}/items`;
   axios
    .get(path)
    .then((response) => {
      unattachedItems.value = response.data
    })
    .then(() => {
      loadingUnattachedItems.value = false;
    });
}

async function createNewBox() {
  const postBoxRequest = { Number: boxNumber.value, Name: boxName.value };
  let boxesPath = `/api/users/${currentUserId.value}/boxes`
  await axios.post(boxesPath, postBoxRequest)
    .then(getBoxes);
}

onMounted(async () => {
  currentUserId.value = router.currentRoute.value.params.userId as string;
  getBoxes();
  getUnattachedItems();
});

function closeQrCodeDialog() {
  displayQrCodeDialog.value = false;
}

function removeUnattachedItemFromList(itemId:string){
  unattachedItems.value = unattachedItems.value.filter(obj => obj.itemId !== itemId);
}

function deleteUnattachedItem(itemId:string) {
  BoxService.deleteUnattachedItem(currentUserId.value, itemId)
  .then((response => {
    removeUnattachedItemFromList(itemId)
  }))
}

function showSuccess(message:string, life:number){
  toast.add({severity:'success', summary: message, life: life});
}

function addBackUnattachedItem(itemId:string) {
  let unattachedItem = unattachedItems.value.find(e => e.itemId === itemId);
  var box = boxes.value.find(b => b.boxId === unattachedItem!.previousBoxId);
  BoxService.addBackUnattachedItem(currentUserId.value, unattachedItem!.previousBoxId, itemId)
  .then((response => {
    removeUnattachedItemFromList(itemId)
  }))
  .then(() => showSuccess(`Item ${unattachedItem?.name} added back to box ${box?.number}`, 3000))
  .then(getBoxes)
}


function openQrCodeDialog(boxId: string) {
  qrCodeLink.value = window.location.origin + router.currentRoute.value.path + "/boxes/" + boxId;
  displayQrCodeDialog.value = true;
}

const filterBoxes = () => boxes.value.filter((box) => !filter.value || box.items.some((item: any) => item.name.toLowerCase().includes(filter.value.toLowerCase())))

function clearFilter() {
  filter.value = "";
}

function toggle(event: MouseEvent, itemId: string)  { 
  selectedUnattachedItem.value = itemId;
  menu.value?.toggle(event);
}


function showItemsMatchingFilter(items: any) {
  const maxLength = 8;
  let itemsMatchingFilter = items.filter((item: { name: string; }) => item.name.toLowerCase().includes(filter.value.toLowerCase()))
  if (itemsMatchingFilter.length > maxLength) {
    let overflowItems = itemsMatchingFilter.length - maxLength;
    const slicedArray = itemsMatchingFilter.slice(0, maxLength - 1);
    slicedArray.push(new Item("no-id", "  ", "..."));
    slicedArray.push(new Item("no-id", `${overflowItems} items not shown...`, "..."));
    return slicedArray;
  }
  return itemsMatchingFilter;
}

function trimString(text: string) {
  const maxLength = 30;
  return text.length > maxLength ? text.substring(0, maxLength - 3) + "..." : text;
}

</script>
<template>
<Header />
<div class="container">

  <Toast/>
  <div class="searchbar">
    <h1>Search for items</h1>
    <span class="p-input-icon-right p-input-icon-left ">
        <i class="pi pi-search" />
        <InputText class="searchinput" type="text" v-model="filter" placeholder="Search" />
        <i v-if=filter class="pi pi-times" @click="clearFilter()" />
      </span>
  </div>
  <div class="boxes">
    <h1>Boxes</h1>
    <div v-if="loadingBoxes" class="boxescontainer">
      <Card v-for="box in new Array(8)" class="boxcard">
        <template #title>
          <Skeleton width="10rem" class="mb-2"></Skeleton>
        </template>
        <template #subtitle>
          <Skeleton width="10rem" class="mb-2"></Skeleton>
        </template>
      </Card>
    </div>
    <div v-else class="boxescontainer">
      <BoxCard :userId="currentUserId" :boxId="box.boxId" :boxNumber="box.number" :boxName="box.name" :boxItemCount="box.items.length" v-for="box in filterBoxes()" class="boxcard">
        <template v-if=filter #content>
          <div class="matchingitemsbox" v-for="item in showItemsMatchingFilter(box.items)"> {{ trimString(item.name) }}
          </div>
        </template>
      </BoxCard>
      <Dialog v-model:visible="displayQrCodeDialog" :style="{ width: '450px' }" header="Confirm" :modal="true">
        <div class="confirmation-content">
          <qrcode-vue :value="qrCodeLink"></qrcode-vue>
        </div>
        <template #footer>
          <Button label="Close" icon="pi pi-times" class="p-button-text" @click="closeQrCodeDialog" />
        </template>
      </Dialog>
    </div>
  </div>
  <div class="unattacheditems">
    <h1>Unattached items</h1>
    <ConfirmPopup></ConfirmPopup>
        <ConfirmPopup group="demo">
            <template #message="slotProps">
                <div class="flex p-4">
                    <i :class="slotProps.message.icon" style="font-size: 1.5rem"></i>
                    <p class="pl-2">{{slotProps.message.message}}</p>
                </div>
            </template>
        </ConfirmPopup>
        <Toast />
    <DataTable v-if="loadingUnattachedItems" :value=" new Array(10)">
                <Column field="name" header="Name">
                    <template #body>
                        <Skeleton></Skeleton>
                    </template>
                </Column>
            
            </DataTable>

      <DataTable v-else :value="unattachedItems" :rowHover="true" dataKey="itemId">
        
        <Column field="name" header="Name" style="width:150px" class="p-pluid"> </Column>
        <Column field="previousBoxNumber" header="Previous box"> </Column>
        <Column style="width:30px">
            <template #body="slotProps">
              <Button icon="pi pi-ellipsis-h" class="p-button-rounded p-button-text" @click="toggle($event, slotProps.data.itemId)" aria-haspopup="true" aria-controls="overlay_menu" />
              <Menu id="overlay_menu" ref="menu" :model="menuItems" :popup="true" />
            </template>
        </Column>
        
      </DataTable>
  </div>
</div>
  <div class="card">
    <div class="field">
      <InputText v-model="boxName" type="text" placeholder="Name" />
    </div>
    <div class="field">
      <InputNumber v-model="boxNumber" :min="0" :max="100" placeholder="Name" />
    </div>
    <Button @click="createNewBox" type="submit" label="Create new box" class="mt-2" />
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300&display=swap');
.boxtitle {
  font-size: medium;
}
.h1{
  font-family: 'Roboto', sans-serif;
}

.container {  
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  grid-template-rows: 0.2fr 1.8fr 1fr;
  gap: 30px 30px;
  grid-auto-flow: row;
  grid-template-areas:
    "searchbar searchbar searchbar"
    "boxes boxes unattacheditems"
    ". . .";
  width: 1200px;
  margin: auto;
  min-height: 1000px;
  
}

.searchbar { 
  margin-top: 30px;
  grid-area: searchbar; 
}

.boxes { 
  grid-area: boxes;

}

.unattacheditems { grid-area: unattacheditems; }

.boxescontainer {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  padding: 5px;
}

.searchinput {
  width: 700px;
  margin: auto;
}

.searchfield {
  display: flex;
  align-items: center;
  width: 700px;
}

.wrapper {
  align-items: center;
}

.footer-button {
  display: inline-block;
  justify-content: space-between;
  padding: 5px;
  align-items: center;
}

.boxcard {
  background-color: white;
  border-radius: 10px;
  width: 250px;
  height: 200px;
  /* 32:18, i.e. 16:9 */
  margin-bottom: 2%;
  /* (100-32*3)/2 */
}

.matchingitemsbox {
  font-size: x-small;
}

.boxcard:hover {
  background-color: #f3faff;
  cursor: pointer;
}
</style>
