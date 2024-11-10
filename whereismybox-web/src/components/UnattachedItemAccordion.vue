<script setup lang="ts">

import { computed} from 'vue'

import router from '@/router';
import Menu from 'primevue/menu';
import Button from 'primevue/button';
import BoxService from '@/services/boxservice';
import InputNumber from 'primevue/inputnumber';
import { ref } from 'vue';
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";
import Dialog from 'primevue/dialog'
import UnattachedItem from '@/models/UnattachedItem';

const props = defineProps({
  unattachedItem:  {
      type: UnattachedItem,  // TODO why do we get a warning if type is UnattachedItem?
      required: true
  },
  isLoading: {
    type: Boolean,
    required: false
  }
}
);

const toast = useToast();
const confirm = useConfirm();
const collectionId = router.currentRoute.value.params.collectionId as string;
const unattachedItem = computed(() => props.unattachedItem);
const isLoading = computed(() => props.isLoading);
const menu: any = ref(null);
const displayMoveItemDialog = ref(false);
const targetBoxNumber = ref(0);

const menuItems = ref([
    {
        label: 'Options',
        items: [
          {
            label: 'Add item back to the previous box', icon: 'pi pi-history',
            command: () => moveUnattachedItem(unattachedItem.value.itemId, unattachedItem.value.previousBoxNumber)   
          },
          {
            label: 'Move item to new box', icon: 'pi pi-history',
            command: () => openMoveItemDialog() 
          },
          {
            label: 'Delete item', icon: 'pi pi-trash',
            command: () => deleteUnattachedItem(unattachedItem.value.itemId)
        }
    ]}
]);

function moveUnattachedItem(itemId:string, boxNumber:number) {
  BoxService.moveUnattachedItem(collectionId, boxNumber, itemId)
    .then(() => showSuccess(`Item ${trimString(30, unattachedItem.value.name)} added back to box ${boxNumber}`, 3000))
    .catch(e => {
      if(e.status == 400){
        toast.add({ severity: 'warn', summary: 'Error', detail: `Sorry, this box does not exist`, life: 3000 })
      }
      else {
        toast.add({ severity: 'error', summary: 'Error', detail: `Oops, something went wrong. Try again later!`, life: 3000 })
      }
    })
}

function deleteUnattachedItem(itemId:string) {
  BoxService.deleteUnattachedItem(collectionId, itemId)
    .then(() => showSuccess(`Item ${unattachedItem.value.name} was deleted`, 3000))
}

function showSuccess(message:string, life:number){
  toast.add({severity:'success', summary: message, life: life});
}

function toggleItemMenu(event: MouseEvent)  { 
  menu.value?.toggle(event);
}

function trimString(maxLength: number, text: string) {
  return text.length > maxLength ? text.substring(0, maxLength - 3) + "..." : text;
}

function openMoveItemDialog() {
  resetTargetBoxNumber();
  displayMoveItemDialog.value = true;
}

function closeMoveItemDialog() {
  resetTargetBoxNumber();
  displayMoveItemDialog.value = false;
}

function resetTargetBoxNumber(){
  targetBoxNumber.value = unattachedItem.value.previousBoxNumber == null ? 0 : unattachedItem.value.previousBoxNumber;
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
    <Button severity="success" text icon="pi pi-plus" label="Move item" type="submit"  @click="moveUnattachedItem(unattachedItem.itemId, targetBoxNumber)" />
    <Button severity="secondary" text icon="pi pi-times" label="Close" @click="closeMoveItemDialog" />
  </template>
  </Dialog>
<div class="unattacheditemaccordion-container">
  <div class="unattacheditemaccordion-name">
    <slot name="name"></slot>
  </div>
  <div class="unattacheditemaccordion-description">
    <slot name="description"></slot>
  </div>
  <div class="unattacheditemaccordion-previousbox">
    <slot name="previousbox"></slot>
  </div>
  <div v-if="!isLoading" class="unattacheditemaccordion-options">
    <i class="pi pi-ellipsis-h unattacheditemaccordion-options-icon" @click="toggleItemMenu($event)" />
    <Menu id="overlay_menu" ref="menu" :model="menuItems" :popup="true" />
  </div>
</div>
</template>

<style scoped>


.unattacheditemaccordion-container {  
  display: grid;
  grid-template-columns: 2.3fr 0.4fr 0.3fr; 
  grid-template-rows: 0.4fr 0.4fr; 
  gap: 0px 10px;
  grid-auto-flow: row;
  grid-template-areas: 
    "name previous-box options"
    "description . options"; 
  height: 50px;
  margin: 5px;
  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
  background-color: white;
  border-radius: 3px;
  font-size: 12px;
  padding-left: 10px;
  background-color: white;
  color: black;
  
}

.unattacheditemaccordion-container:hover {  
  background-color: #f2f2f2;
}

.unattacheditemaccordion-name { 
  grid-area: name;
  display: flex;
  line-height: 25px;
  height: 25px;
}

.unattacheditemaccordion-description { 
  grid-area: description;
  display: flex;
  line-height: 25px;
  height: 25px;
}

.unattacheditemaccordion-previousbox { 
  grid-area: previous-box; 
  line-height: 50px;
  height: 25px;
}

.unattacheditemaccordion-options { 
  grid-area: options;
  height: 25px;
  line-height: 50px;
  cursor: pointer;
  }

.unattacheditemaccordion-options-icon {
  height: 30px;
  line-height: 30px;
}
</style>
