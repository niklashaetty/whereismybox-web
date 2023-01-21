<script setup lang="ts">
import router from '@/router';
import axios from 'axios';
import { onMounted, ref} from 'vue';
import Column from 'primevue/column';
import DataTable from 'primevue/datatable';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import BoxService from '@/services/boxservice';
import Item from '@/models/Item';
import Box from '@/models/Box';
import Toast from 'primevue/toast';
import { useToast } from "primevue/usetoast";

const toast = useToast();
let currentBox = ref(new Box("", 0, "", []));
let newItemName = ref("");
let newItemDescription = ref("");
const deleteItemDialog = ref(false);
const itemToBeDeleted = ref();
let currentUserId = ref("");
let currentBoxId = ref("");

async function updateCurrentBox(){
  BoxService.getBox(currentUserId.value, currentBoxId.value)
  .then((response) => {
    let tmpItems:Item[] = [];
    response.data.items.forEach((itemDto: { itemId: string; name: string; description: string; }) => {
      tmpItems.push(new Item(itemDto.itemId, itemDto.name, itemDto.description))
    });
    currentBox.value = new Box(response.data.boxId, response.data.number, response.data.name, tmpItems);
  });
}

onMounted(async () => {
  currentUserId.value = router.currentRoute.value.params.userId as string;
  currentBoxId.value =  router.currentRoute.value.params.boxId as string;
  await updateCurrentBox();
});

const onCellEditComplete = (event: { data: any; newValue: any; field: any; }) => {
    let { data, newValue, field } = event;
    let oldValue = data[field];
    if(oldValue !== newValue){
      data[field] = newValue;
      let updatedItem = new Item(data.itemId, data.name, data.description);
      updateItem(updatedItem);
    }
};

function openDeleteItemDialog(item:Item) {
  itemToBeDeleted.value = item;
  deleteItemDialog.value = true;
}

function addItemToBox(name:string, description:string) {
  BoxService.addItemToBox(currentUserId.value, currentBoxId.value, name, description)
  .then((response) => {
    currentBox.value.addItem(new Item(response.data.itemId, response.data.name, response.data.description));
    showSuccess(`Added new item ${response.data.name}`, 3000)
    newItemName.value = "";
    newItemDescription.value = "";
  });
}

function updateItem(updatedItem:Item){
    BoxService.putItem(currentUserId.value, currentBoxId.value, updatedItem)
    .then(() => currentBox.value.editItem(updatedItem))
}

function deleteItem(item:Item) {
  BoxService.deleteItem(currentUserId.value, currentBoxId.value, item.itemId)
  .then((response => {
    currentBox.value.deleteItem(item);
    deleteItemDialog.value = false;
    showSuccess(`Removed ${item.name}`, 3000);
  }))
}

function showSuccess(message:string, life:number){
  toast.add({severity:'success', summary: message, life: life});
}
</script>

<template>
  <div>
    <Toast />
    <h1>{{currentBox.number}} - {{ currentBox.name }}</h1>
  	<div class="card">
      <DataTable :value="currentBox.items" editMode="cell" dataKey="itemId" @cell-edit-complete="onCellEditComplete" >
        <Column field="name" header="Name" style="width:50%" class="p-pluid">
                    <template #editor="{ data, field }">
                        <InputText v-model="data[field]" autofocus />
                    </template>
                </Column>

                <Column field="description" header="Description" style="width:50%">
                    <template #editor="{ data, field }">
                        <InputText v-model="data[field]" autofocus />
                    </template>
                </Column>
                <Column style="min-width:8rem">
                    <template #body="slotProps">
                        <Button icon="pi pi-trash" class="p-button-rounded p-button-warning" @click="openDeleteItemDialog(new Item(slotProps.data.itemId,slotProps.data.name, slotProps.data.description))" />
                    </template>
                </Column>
      </DataTable>
      <br/>
      <div>
        <span class="p-float-label inline-block-child">
            <InputText id="newItemNameId" type="text" v-model="newItemName" />
            <label for="newItemNameId">Name</label>
          </span>
          <span class="p-float-label inline-block-child">
            <InputText id="newItemDescriptionId" type="text" v-model="newItemDescription" />
            <label for="newItemDescriptionId">Description</label>
        </span>
        <Button class="inline-block-child" @click="addItemToBox(newItemName, newItemDescription)" type="submit" label="Add item"/>
      </div>
     
	</div>

  <Dialog v-model:visible="deleteItemDialog" :style="{width: '450px'}" header="Confirm" :modal="true">
            <div class="confirmation-content">
                <i class="pi pi-exclamation-triangle mr-3" style="font-size: 2rem" />
                <span v-if="itemToBeDeleted">Are you sure you want to delete <b>{{itemToBeDeleted.name}}</b>?</span>
            </div>
            <template #footer>
                <Button label="No" icon="pi pi-times" class="p-button-text" @click="deleteItemDialog = false"/>
                <Button label="Yes" icon="pi pi-check" class="p-button-text" @click="deleteItem(itemToBeDeleted)" />
            </template>
        </Dialog>
  </div>
</template>

<style scoped>

.inline-block-child {
  display: inline-block;
  padding: 5px;
}


.boxwrapper {
  /* We first create a flex layout context */
  display: flex;
  padding: 10px;

  /* Then we define the flow direction 
     and if we allow the items to wrap 
   * Remember this is the same as:
   * flex-direction: row;
   * flex-wrap: wrap;
   */
  flex-flow: row wrap;

  /* Then we define how is distributed the remaining space */
  justify-content: space-around;
}

@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    align-items: center;
  }
}
</style>
