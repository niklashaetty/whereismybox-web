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
const collectionId = router.currentRoute.value.params.collectionId as string;
const boxId = computed(() => props.boxId);
const displayAddItemDialog = ref(false);
const itemName = ref("");
const itemDescription = ref("");

function closeAddItemDialog() {
  displayAddItemDialog.value = false;
  itemName.value = "";
  itemDescription.value = "";
}

function openAddItemDialog() {
  displayAddItemDialog.value = true;
}

function addNewItem() {
  BoxService.addItemToBox(collectionId, boxId.value, itemName.value, itemDescription.value)
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
    <i class="pi pi-plus-circle boxie-icon clickable" @click="openAddItemDialog" />
      <Dialog v-model:visible="displayAddItemDialog" :style="{ width: '450px' }" header="Add a new item" :modal="true">
         <div class="card">
          <p style="font-size:10px; margin-bottom: 10px;"> An item consists of a name, and optionally a description. 
            Currently only the name is searchable.</p>
          <div class="field">
            <InputText v-model="itemName" type="text" placeholder="Name - e.g. Bike helmet" />
          </div>
          <div class="field">
            <InputText v-model="itemDescription" type="text" placeholder="Description - e.g. Ferrari, red with white stripes" />
          </div>
          
        </div>
        <template #footer>
          <Button icon="pi pi-plus" style="margin-top: 10px;" severity="success" text raised @click="addNewItem" type="submit" label="Add new item" class="mt-2" />
          <Button severity="plain" text raised label="Close" icon="pi pi-times" class="p-button-text" @click="closeAddItemDialog" />
        </template>
      </Dialog>
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
  background-color:none;
  height: 30px;
  line-height: 30px;
  border-color: #e9f5db;
  font-weight: 900;
  color: #40513B;
  font-size: 12px;
  padding: 2px;
  padding-left: 10px;
  @media (min-width: 500px) {
    font-size: 14px;
  }
}

.itemaccordion-name { 
  grid-area: name; 
}

.itemaccordion-description { grid-area: description; }

.itemaccordion-options { grid-area: options;
margin-left: auto; 
margin-top: auto;
}

.clickable {
  cursor: pointer;
}

.boxie-icon {
  font-size: 16px;
  text-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
  color: #656c63;
  @media (min-width: 500px) {
    font-size: 20px;
  }
}

</style>
