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
import Item from '@/models/Item';
import type Box from '@/models/Box';
import type UnattachedItem from '@/models/Box';

let boxes = ref<Box[]>([]);
let unattachedItems = ref<UnattachedItem[]>([]);
let filteredBoxes = ref();
const boxName = ref("");
const boxNumber = ref(0);
const userId = ref(router.currentRoute.value.params.userId);
const loadingBoxes = ref(false);
const loadingUnattachedItems = ref(false);
const displayQrCodeDialog = ref(false)
const qrCodeLink = ref("");
const filter = ref("");

async function getBoxes() {
  loadingBoxes.value = true;
  console.log("Loading boxes")
  let boxesPath = `/api/users/${userId.value}/boxes`
   axios
    .get(boxesPath)
    .then((response) => {
      boxes.value = response.data
      filteredBoxes.value = response.data;
    })
    .then(() => {
      loadingBoxes.value = false;
      console.log("Loading boxes completed. value: " + loadingBoxes.value)
    });
}

async function getUnattachedItems() {
  loadingUnattachedItems.value = true;
  console.log("Loading unattached items")
  let path = `/api/users/${userId.value}/items`
   axios
    .get(path)
    .then((response) => {
      unattachedItems.value = response.data
    })
    .then(() => {
      loadingUnattachedItems.value = false;
      console.log("Loading boxes completed. value: " + loadingUnattachedItems.value)
    });
}

async function createNewBox() {
  const postBoxRequest = { Number: boxNumber.value, Name: boxName.value };
  let boxesPath = `/api/users/${userId.value}/boxes`
  await axios.post(boxesPath, postBoxRequest)
    .then(getBoxes);
}

onMounted(async () => {
  getBoxes();
  getUnattachedItems();
});

function closeQrCodeDialog() {
  displayQrCodeDialog.value = false;
}

function openQrCodeDialog(boxId: string) {
  qrCodeLink.value = window.location.origin + router.currentRoute.value.path + "/boxes/" + boxId;
  displayQrCodeDialog.value = true;
}

const filterBoxes = () => boxes.value.filter((box) => !filter.value || box.items.some((item: any) => item.name.toLowerCase().includes(filter.value.toLowerCase())))

function clearFilter() {
  filter.value = "";
}

function showItemsMatchingFilter(items: any) {
  const maxLength = 8;
  let itemsMatchingFilter = items.filter((item: { name: string; }) => item.name.toLowerCase().includes(filter.value.toLowerCase()))
  if (itemsMatchingFilter.length > maxLength) {
    console.log("Too many matches!")
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
<div class="container">
  <div class="searchbar">
    <h1>Search for items</h1>
    <span class="p-input-icon-right p-input-icon-left ">
        <i class="pi pi-search" />
        <InputText type="text" v-model="filter" placeholder="Search" />
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
      <Card v-for="box in filterBoxes()" @click="$router.push({ path: `/users/${userId}/boxes/${box.boxId}` })"
        class="boxcard">
        <template #title>
          <p class="boxtitle">{{ box.number }} - {{ box.name }}</p>
        </template>
        <template v-if=!filter #subtitle>
          {{ box.items.length }} items
        </template>

        <template v-if=filter #content>
          <div class="matchingitemsbox" v-for="item in showItemsMatchingFilter(box.items)"> {{ trimString(item.name) }}
          </div>
        </template>

        <template v-if=!filter #footer style="align-items: center;">
          <div class="footer-button"><Button @click.stop="openQrCodeDialog(box.boxId)" icon="pi pi-qrcode" /></div>
        </template>
      </Card>
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
    <DataTable v-if="loadingUnattachedItems" :value=" new Array(10)">
                <Column field="name" header="Name">
                    <template #body>
                        <Skeleton></Skeleton>
                    </template>
                </Column>
            </DataTable>

      <DataTable v-else :value="unattachedItems" :rowHover="true" dataKey="itemId">
        <Column field="name" header="Name" style="width:150px" class="p-pluid"> </Column>
        <Column style="width:30px">
            <template #body="slotProps">
                <Button icon="pi pi-trash" class="p-button-rounded p-button-warning" />
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
.boxtitle {
  font-size: medium;
}

.container {  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  grid-template-rows: 0.2fr 1.8fr 1fr;
  gap: 0px 0px;
  grid-auto-flow: row;
  grid-template-areas:
    "searchbar searchbar searchbar"
    "boxes boxes unattacheditems"
    ". . .";
}

.searchbar { grid-area: searchbar; }

.boxes { grid-area: boxes; }

.unattacheditems { grid-area: unattacheditems; }



.boxescontainer {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
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
  border-radius: 25px;
  width: 250px;
  height: 200px;
  padding-bottom: 10%;
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
