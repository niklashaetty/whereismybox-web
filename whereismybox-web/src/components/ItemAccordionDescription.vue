<script setup lang="ts">
import router from '@/router';
import Button from 'primevue/button'
import { computed, ref } from 'vue';
import BoxService from '@/services/boxservice';
import { useToast } from "primevue/usetoast";
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'

const props = defineProps(
  {
    boxId:  {
        type: String,
        required: true
    }
  }
);

const toast = useToast();
const userId = router.currentRoute.value.params.userId as string;
const boxId = computed(() => props.boxId);
const displayAddItemDialog = ref(false);
const itemName = ref("");
const itemDescription = ref("");

function closeAddItemDialog() {
  displayAddItemDialog.value = false;
}

function openAddItemDialog() {
  displayAddItemDialog.value = true;
}

function addNewItem() {
  BoxService.addItemToBox(userId, boxId.value, itemName.value, itemDescription.value)
  .then(() => closeAddItemDialog())
  .then((response) => toast.add({severity:'success', summary: `${itemName.value} was added to box`, life: 3000}))
}
</script>

<template>
<div class="itemaccordion-container">
  <div class="itemaccordion-name">
    <p> Name </p>
  </div>
  <div class="itemaccordion-description">
    <p> Description</p>
  </div>
  <div class="itemaccordion-options">
    <Button size="small" style=
      "margin-left: auto; margin-right: 5px; font-size: 10px; color: #181F1C;"  
      icon="pi pi-plus" text outlined raised rounded aria-label="Filter" @click="openAddItemDialog" />

      <Dialog v-model:visible="displayAddItemDialog" :style="{ width: '450px' }" header="Add a new item" :modal="true">
         <div class="card">
          <div class="field">
            <InputText v-model="itemName" type="text" placeholder="Name" />
          </div>
          <div class="field">
            <InputText v-model="itemDescription" type="text" placeholder="Description" />
          </div>
          <Button @click="addNewItem" type="submit" label="Add new item" class="mt-2" />
        </div>
        <template #footer>
          <Button label="Close" icon="pi pi-times" class="p-button-text" @click="closeAddItemDialog" />
        </template>
      </Dialog>
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
  background-color:none;
  height: 30px;
  line-height: 30px;
  border-color: #e9f5db;
  font-weight: 900;
  color: #40513B;
  font-size: 15px;
  padding: 2px;
  padding-left: 10px;
}

.itemaccordion-name { 
  grid-area: name; 
}

.itemaccordion-description { grid-area: description; }

.itemaccordion-options { grid-area: options;
margin-left: auto; 
margin-top: auto;
}
</style>