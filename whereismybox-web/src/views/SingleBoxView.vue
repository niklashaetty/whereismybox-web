<script setup lang="ts">
import router from '@/router';
import { onMounted, ref, type Ref} from 'vue';
import Column from 'primevue/column';
import DataTable from 'primevue/datatable';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import Skeleton from 'primevue/skeleton';
import Menu from 'primevue/menu';
import BoxService from '@/services/boxservice';
import Item from '@/models/Item';
import Box from '@/models/Box';
import Toast from 'primevue/toast';
import Header from '@/components/Header.vue'
import { useToast } from "primevue/usetoast";

const toast = useToast();
const currentBox = ref(new Box("", 0, "", []));
const skeletonItems = ref(new Array(5))
const newItemName = ref("");
const newItemDescription = ref("");
const loadingCurrentBox = ref(false);
const deleteItemDialog = ref(false);
const itemIdToBeDeleted = ref("")
const currentUserId = ref("");
const currentBoxId = ref("");

const menu: any = ref(null);
const menuItems = ref([
    {
        label: 'Options',
        items: [{label: 'Remove item', icon: 'pi pi-times-circle',
        command: () => {
              removeSelectedItemFromBox();
            }
        },
        {label: 'Delete', icon: 'pi pi-trash',
            command: () => {
              openHardDeleteItemDialog()
            }
        }
    ]}
]);

function showSuccess(message:string, life:number){
  toast.add({severity:'success', summary: message, life: life});
}

async function updateCurrentBox(){
  loadingCurrentBox.value = true;
  BoxService.getBox(currentUserId.value, currentBoxId.value)
  .then((response) => {
    let tmpItems:Item[] = [];
    response.data.items.forEach((itemDto: { itemId: string; name: string; description: string; }) => {
      tmpItems.push(new Item(itemDto.itemId, itemDto.name, itemDto.description))
    });
    currentBox.value = new Box(response.data.boxId, response.data.number, response.data.name, tmpItems);
  })
  .catch((error) => console.log(error))
  loadingCurrentBox.value = false;
}

onMounted(async () => {
  currentUserId.value = router.currentRoute.value.params.userId as string;
  currentBoxId.value =  router.currentRoute.value.params.boxId as string;
  loadingCurrentBox.value = true;
  await updateCurrentBox();
  loadingCurrentBox.value = false;
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

function openHardDeleteItemDialog() {
  deleteItemDialog.value = true;
}

function closeDeleteItemDialog() {
  itemIdToBeDeleted.value = "";
  console.log("setting item to be deleted to nothing");
  deleteItemDialog.value = false;
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

function hardDeleteSelectedItem() {
  deleteItem(itemIdToBeDeleted.value, true);
}

function removeSelectedItemFromBox() {
  deleteItem(itemIdToBeDeleted.value, false)
}

function deleteItem(itemId:string, hardDelete: boolean) {
  BoxService.deleteItem(currentUserId.value, currentBoxId.value, itemId, hardDelete)
  .then((response => {
    itemIdToBeDeleted.value = "";
    let itemName = currentBox.value.getItem(itemId).name;
    currentBox.value.deleteItem(itemId);
    deleteItemDialog.value = false;
    showSuccess(`Removed ${itemName}`, 3000);
  }))
}

function toggle(event: MouseEvent, itemId: string)  { 
  itemIdToBeDeleted.value = itemId;
  menu.value?.toggle(event);
}
</script>

<template>
  <Header />
  <div class="wrapper">
    <Toast/>
    <h1>{{currentBox.number}} - {{ currentBox.name }}</h1>
  	<div class="card">
      <DataTable v-if="loadingCurrentBox" :value="skeletonItems">
                <Column field="name" header="Name">
                    <template #body>
                        <Skeleton></Skeleton>
                    </template>
                </Column>
                <Column field="description" header="Description">
                    <template #body>
                        <Skeleton></Skeleton>
                    </template>
                </Column>
            </DataTable>

      <DataTable v-else :value="currentBox.items" :rowHover="true" editMode="cell" dataKey="itemId" @cell-edit-complete="onCellEditComplete">
        
        <Column field="name" header="Name" style="width:150px" class="p-pluid">
                    <template #editor="{ data, field }">
                        <InputText v-model="data[field]" autofocus />
                    </template>
                </Column>

                <Column field="description" header="Description" style="width:150px">
                    <template #editor="{ data, field }">
                        <InputText v-model="data[field]" autofocus />
                    </template>
                </Column>
                <Column style="width:30px">
                    <template #body="slotProps">
                        <Button icon="pi pi-ellipsis-h" class="p-button-rounded p-button-text" @click="toggle($event, slotProps.data.itemId)" aria-haspopup="true" aria-controls="overlay_menu" />
                        <Menu id="overlay_menu" ref="menu" :model="menuItems" :popup="true" />
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
                <span v-if="!!itemIdToBeDeleted">Are you sure you want to delete <b>{{currentBox.getItem(itemIdToBeDeleted).name}}</b>?</span>
            </div>
            <template #footer>
                <Button label="No" icon="pi pi-times" class="p-button-text" @click="closeDeleteItemDialog"/>
                <Button label="Yes" icon="pi pi-check" class="p-button-text" @click="hardDeleteSelectedItem" />
            </template>
  </Dialog>
</div>
</template>

<style scoped>



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

</style>
