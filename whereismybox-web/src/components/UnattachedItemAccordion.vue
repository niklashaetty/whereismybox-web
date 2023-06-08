<script setup lang="ts">

import { defineProps, computed} from 'vue'

import router from '@/router';
import Menu from 'primevue/menu';
import BoxService from '@/services/boxservice';
import { ref } from 'vue';
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";

const props = defineProps({
  unattachedItem:  {
      type: Object,  // TODO why do we get a warning if type is UnattachedItem?
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
const menuItems = ref([
    {
        label: 'Options',
        items: [
          {
            label: 'Add item back to the previous box', icon: 'pi pi-history',
            command: () => addBackUnattachedItem(unattachedItem.value.itemId)   
          },
          {
            label: 'Delete item', icon: 'pi pi-trash',
            command: () => deleteUnattachedItem(unattachedItem.value.itemId)
        }
    ]}
]);


function addBackUnattachedItem(itemId:string) {
  BoxService.addBackUnattachedItem(collectionId, unattachedItem.value.previousBoxId, itemId)
    .then(() => showSuccess(`Item ${trimString(30, unattachedItem.value.name)} added back to box ${unattachedItem.value.previousBoxNumber}`, 3000))
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
</script>

<template>
<div class="unattacheditemaccordion-container">
  <div class="unattacheditemaccordion-name">
    <slot name="name"></slot>
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
  grid-template-columns: 2.4fr 0.3fr 0.3fr;
  grid-template-rows: 0.1fr;
  grid-auto-columns: 1fr;
  gap: 0px 10px;
  grid-auto-flow: row;
  grid-template-areas:
    "name description options";

  height: 30px;
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
  
}

.unattacheditemaccordion-container:hover {  
  background-color: #f2f2f2;
}

.unattacheditemaccordion-name { 
  grid-area: name;
  display: flex;
  line-height: 30px;
  height: 30px;
}

.unattacheditemaccordion-previousbox { 
  grid-area: description; 
  line-height: 30px;
  height: 30px;
}

.unattacheditemaccordion-options { 
  grid-area: options;
  height: 30px;
  line-height: 30px;
  cursor: pointer;
  }

.unattacheditemaccordion-options-icon {
  height: 30px;
  line-height: 30px;
}
</style>
