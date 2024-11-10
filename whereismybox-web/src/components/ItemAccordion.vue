<script setup lang="ts">

import { computed} from 'vue'

import router from '@/router';
import Button from 'primevue/button';
import Menu from 'primevue/menu';
import InputNumber from 'primevue/inputnumber';
import BoxService from '@/services/boxservice';
import { ref } from 'vue';
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";
import Dialog from 'primevue/dialog'

const props = defineProps({
  item:  {
      type: Object,  // TODO why do we get a warning if type is Item?
      required: true
  },
  boxId: {
    type: String,
    required: true
  },
  boxNumber: {
    type: Number,
    required: true
  }
}
);

const toast = useToast();
const confirm = useConfirm();
const collectionId = router.currentRoute.value.params.collectionId as string;
const item = computed(() => props.item);
const boxId = computed(() => props.boxId);
const boxNumber = computed(() => props.boxNumber);
const displayMoveItemDialog = ref(false);
const targetBoxNumber = ref(0);

const menu: any = ref(null);
const menuItems = ref([
    {
        label: 'Options',
        items: [
        {
          label: 'Move item', 
          icon: 'pi pi-arrow-circle-right',
            command: () => {
              openMoveItemDialog();
            }
        },
        {label: 'Remove item from box', icon: 'pi pi-times-circle',
        command: () => {
              removeSelectedItemFromBox(collectionId, boxId.value, item.value.itemId);
            }
        },
        {
          label: 'Delete item', 
          icon: 'pi pi-trash',
            command: () => {
              confirmHardDeleteItem(collectionId, boxId.value, item.value.itemId)
            }
        },
        
    ]}
]);

function confirmHardDeleteItem(collectionId: string, boxId: string, itemId: string) {

  confirm.require({
        message: `Are you sure you want to delete ${item.value.name}? This action cannot be undone`,
        header: `Deleting ${item.value.name}`,
        icon: 'pi pi-info-circle',
        acceptClass: 'p-button-danger',
        accept: () => deleteItem(collectionId, boxId, itemId, true),
        reject: () => {}
    });
}

function removeSelectedItemFromBox(collectionId: string, boxId: string, itemId: string) {
  deleteItem(collectionId, boxId, itemId, false)
}

function deleteItem(collectionId: string, boxId: string, itemId: string, hardDelete: boolean) {
  BoxService.deleteItem(collectionId, boxId, itemId, hardDelete);
}

function toggleItemMenu(event: MouseEvent)  { 
  menu.value?.toggle(event);
}

function openMoveItemDialog() {
  targetBoxNumber.value = boxNumber.value;
  displayMoveItemDialog.value = true;
}

function closeMoveItemDialog() {
  targetBoxNumber.value = boxNumber.value;
  displayMoveItemDialog.value = false;
}

async function moveItem() {
 await BoxService.moveItem(collectionId, boxId.value, item.value.itemId, targetBoxNumber.value)
    .then(closeMoveItemDialog)
    .then(() => toast.add({ severity: 'success', summary: 'Moved item', detail: `Item ${item.value.name} moved to box ${targetBoxNumber.value}`, life: 3000 }))
    .catch(e => {
      if(e.status == 400){
        toast.add({ severity: 'warn', summary: 'Target box not found', detail: `No box with number ${targetBoxNumber.value} was found`, life: 3000 })
      }
      else {
        toast.add({ severity: 'error', summary: 'Error', detail: `Oops, something went wrong. Try again later!`, life: 3000 })
      }
    })
  }

</script>

<template>
  <Dialog @update:visible="closeMoveItemDialog" v-model:visible="displayMoveItemDialog" :style="{ width: '450px' }" header="Move item" :modal="true">
  <div class="card">
    <div class="field">
      <InputNumber v-model="targetBoxNumber" :min="0" :max="200" placeholder="0" />
    </div>
    
  </div>
  <template #footer>
    <Button severity="success" text icon="pi pi-plus" label="Move item" type="submit"  @click="moveItem" />
    <Button severity="secondary" text icon="pi pi-times" label="Close" @click="closeMoveItemDialog" />
  </template>
  </Dialog>
<div class="itemaccordion-container">
  <div class="itemaccordion-name">
    <slot name="name"></slot>
  </div>
  <div class="itemaccordion-description">
    <slot name="description"></slot>
  </div>
  <div class="itemaccordion-options">
    <i class="pi pi-ellipsis-h itemaccordion-options-icon" @click="toggleItemMenu($event)" />
    <Menu id="overlay_menu" ref="menu" :model="menuItems" :popup="true" />
  </div>
</div>
</template>

<style scoped lang="scss">

.itemaccordion-container {  display: grid;
  grid-template-columns: 1fr 1.7fr 0.3fr;
  grid-template-rows: 0.1fr;
  grid-auto-columns: 1fr;
  gap: 0px 10px;
  grid-auto-flow: row;
  grid-template-areas:
    "name description options";
  background-color: white;
  height: 30px;
  border-bottom: 1px solid;
  border-color: #e9f5db;
  border-radius: 2px;
  margin: 1px;
  padding: 2px;
  padding-left: 10px;
  font-size: 12px;
  color: black;
}

.itemaccordion-container:hover {  
  background-color: #f2f2f2;
}

.itemaccordion-name { 
  grid-area: name; 
  height: 30px;
  line-height: 30px;
}

.itemaccordion-description { 
  grid-area: description; 
  height: 30px;
  line-height: 30px;
}

.itemaccordion-options { 
  grid-area: options;
  height: 30px;
  line-height: 30px;
  }

.itemaccordion-options-icon{
  font-size: 12px;
  @media (min-width: 500px) {
  font-size: 14px;
  }
}

</style>
