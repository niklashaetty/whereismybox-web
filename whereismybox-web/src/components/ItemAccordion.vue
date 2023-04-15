<script setup lang="ts">

import { defineProps, computed} from 'vue'

import router from '@/router';
import Button from 'primevue/button';
import Menu from 'primevue/menu';
import BoxService from '@/services/boxservice';
import { ref } from 'vue';
import { useConfirm } from "primevue/useconfirm";

const props = defineProps({
  item:  {
      type: Object,  // TODO why do we get a warning if type is Item?
      required: true
  },
  boxId: {
    type: String,
    required: true
  }
}
);

const confirm = useConfirm();
const userId = router.currentRoute.value.params.userId as string;
const item = computed(() => props.item);
const boxId = computed(() => props.boxId);

const menu: any = ref(null);
const menuItems = ref([
    {
        label: 'Options',
        items: [{label: 'Remove item from box', icon: 'pi pi-times-circle',
        command: () => {
              removeSelectedItemFromBox(userId, boxId.value, item.value.itemId);
            }
        },
        {label: 'Delete item', icon: 'pi pi-trash',
            command: () => {
              confirmHardDeleteItem(userId, boxId.value, item.value.itemId)
            }
        }
    ]}
]);

function confirmHardDeleteItem(userId: string, boxId: string, itemId: string) {

  confirm.require({
        message: `Are you sure you want to delete ${item.value.name}? This action cannot be undone`,
        header: `Deleting ${item.value.name}`,
        icon: 'pi pi-info-circle',
        acceptClass: 'p-button-danger',
        accept: () => deleteItem(userId, boxId, itemId, true),
        reject: () => {}
    });
}

function removeSelectedItemFromBox(userId: string, boxId: string, itemId: string) {
  deleteItem(userId, boxId, itemId, false)
}

function deleteItem(userId: string, boxId: string, itemId: string, hardDelete: boolean) {
  BoxService.deleteItem(userId, boxId, itemId, hardDelete);
}

function toggleItemMenu(event: MouseEvent)  { 
  menu.value?.toggle(event);
}

</script>

<template>
<div class="itemaccordion-container">
  <div class="itemaccordion-name">
    <slot name="name"></slot>
  </div>
  <div class="itemaccordion-description">
    <slot name="description"></slot>
  </div>
  <div class="itemaccordion-options">
    <Button style="color: #718355; height: 30px;" icon="pi pi-ellipsis-h" class="p-button-rounded p-button-text p-button-sm" @click="toggleItemMenu($event)" aria-haspopup="true" aria-controls="overlay_menu" />
    <Menu id="overlay_menu" ref="menu" :model="menuItems" :popup="true" />
  </div>
</div>
</template>

<style scoped>


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
  font-size: 12px;
  padding: 2px;
  padding-left: 10px;
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
</style>
