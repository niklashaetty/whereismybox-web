<script setup lang="ts">
import { computed, onMounted, watch} from 'vue'

import router from '@/router';
import Button from 'primevue/button';
import Menu from 'primevue/menu';
import BoxService from '@/services/boxservice';
import { ref } from 'vue';
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";
import ItemAccordion from '@/components/ItemAccordion.vue'
import type Item from '@/models/Item';
import Box from '@/models/Box';
import ItemAccordionDescription from '@/components/ItemAccordionDescription.vue'
import Sticker from '@/components/Sticker.vue'
import Skeleton from 'primevue/skeleton';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Dialog from 'primevue/dialog'

import { usePaperizer } from 'paperizer'

const { paperize } = usePaperizer('print', {
  styles: [
    '/testx.css',
  ]
})
const print = () => {
  paperize()
}
const props = defineProps(
  {
    box:  {
        type: Box,
        required: true
    },
    isLoading: {
      type: Boolean,
      required: false
    },
    alwaysExpandedItems: {
      type: Boolean,
      required: false,
      default: false
    },
    searchQuery: {
      type: String,
      required: false
    }
  }
);

const toast = useToast();
const confirm = useConfirm();

const isFullyLoaded = computed(() => !props.isLoading);
const collectionId = router.currentRoute.value.params.collectionId as string;
const box = computed(() => props.box);
const alwaysExpandedItems = computed(() => props.alwaysExpandedItems);
const searchQuery = computed(() => {
  if(props.searchQuery){
    return  props.searchQuery;
  }
  else{
    return "";
  }
});
const displayStickerDialog = ref(false);
const displayEditBoxDialog = ref(false);
const linkToBox = ref("");
const isClicked = ref(false)
const updatedBoxName = ref(box.value.name);
const updatedBoxNumber = ref(box.value.number);

const expanded = computed(() => {
  return alwaysExpandedItems.value || isClicked.value
})

watch(searchQuery, (newValue, oldValue) => {
  if (newValue) {
    isClicked.value = true
  } else {
    isClicked.value = false
  }
});

const menu: any = ref(null);
const menuItems = ref([
    {
        items: [
        {label: 'Go to box', icon: 'pi pi-box',
        command: () => {
            router.push({path: `/collections/${collectionId}/boxes/${box.value.boxId}`})
          }
        },
        {label: 'Edit box details', icon: 'pi pi-pencil',
        command: () => {
          openEditBoxDialog();
        }
        },
        {label: 'Show printable sticker', icon: 'pi pi-qrcode',
        command: () => {
            openStickerDialog();
          }
        },
        {label: 'Delete', icon: 'pi pi-trash',
            command: () => {
              confirmDeleteBox()
            }
        }
    ]}
]);

function closeStickerDialog() {
  displayStickerDialog.value = false;
}

function openStickerDialog() {
  displayStickerDialog.value = true;
}


function closeEditBoxDialog() {
  displayEditBoxDialog.value = false;
  updatedBoxName.value = box.value.name;
  updatedBoxNumber.value = box.value.number;
}

function openEditBoxDialog() {
  displayEditBoxDialog.value = true;
}

function confirmDeleteBox(){
   
   confirm.require({
       message: `Are you sure you want to delete ${box.value.name}? This action cannot be undone, and all ${box.value.items.length} item(s) will be lost.`,
       header: `Deleting box ${box.value.number}`,
       icon: 'pi pi-info-circle',
       acceptClass: 'p-button-danger',
       accept: () => deleteBox(collectionId, box.value.boxId),
       reject: () => {}
   });
};

function deleteBox(collectionId:string, boxId:string) {
  BoxService.deleteBox(collectionId, boxId)
  .then(() => toast.add({ severity: 'success', summary: 'Deleted', detail: `Box ${box.value.name} deleted`, life: 3000 }))
}

function expandBox(){
  isClicked.value = !isClicked.value
}

function toggleBoxMenu(event: MouseEvent)  { 
  menu.value?.toggle(event);
}

function filter()  { 
  if(box.value.items){
    return box.value.items.filter((item: Item) => 
    item.name.toLowerCase().includes(searchQuery.value.toLowerCase())
    || item.description.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }
  else return Array(0);
}

function trimString(maxLength: number, text: string) {
  return text.length > maxLength ? text.substring(0, maxLength - 3) + "..." : text;
}

async function updateBoxDetails() {
  await BoxService.updateBoxDetails(collectionId, box.value.boxId, updatedBoxNumber.value, updatedBoxName.value)
    .then(closeEditBoxDialog)
    .then(() => toast.add({ severity: 'success', summary: 'Updated', detail: `Box ${updatedBoxName.value} updated`, life: 3000 }))
    .catch(e => {
      if(e.status == 409){
        toast.add({ severity: 'warn', summary: 'Error', detail: `Sorry, there is already a box with this number in this collection`, life: 3000 })
      }
      else {
        toast.add({ severity: 'error', summary: 'Error', detail: `Oops, something went wrong. Try again later!`, life: 3000 })
      }
    })
  }

onMounted(async () => {
  linkToBox.value = window.location.origin + router.currentRoute.value.path + "/boxes/" + box.value.boxId + "/";
});

</script>

<template>
    <Dialog v-model:visible="displayStickerDialog" :style="{ width: '450px' }" header="QR code sticker" :modal="true">

    <div id="print">
        <Sticker :qrCodeLink="linkToBox" :boxNumber="box.number" :title="box.name" />
    </div>
        <template #footer>
          <Button severity="success" label="Print" icon="pi pi-print" class="p-button-text" @click="print()" />
          <Button severity="secondary" label="Close" icon="pi pi-times" class="p-button-text" @click="closeStickerDialog()" />
        </template>
    </Dialog>
    <Dialog @update:visible="closeEditBoxDialog" v-model:visible="displayEditBoxDialog" :style="{ width: '450px' }" header="Edit box" :modal="true">
      <div class="card">
        <div class="field">
          <InputText v-model="updatedBoxName" type="text" placeholder="Name" />
        </div>
        <div class="field">
          <InputNumber v-model="updatedBoxNumber" :min="0" :max="100" placeholder="2" />
        </div>
        
      </div>
      <template #footer>
        <Button severity="success" text icon="pi pi-plus" label="Update box" type="submit"  @click="updateBoxDetails" />
        <Button severity="secondary" text icon="pi pi-times" label="Close" @click="closeEditBoxDialog" />
      </template>
    </Dialog>
    
    
<div v-if="isFullyLoaded" class="accordion-container">
  <div class="accordion-icon" @click="expandBox">
    <i v-if="!alwaysExpandedItems && expanded" class="pi pi-angle-down accordion-icon-icon" style="color: slateblue; padding-right: 5px;"></i>
    <i v-else-if="!alwaysExpandedItems && !expanded" class="pi pi-angle-right accordion-icon-icon" style="color: slateblue; padding-right: 5px;"></i>
  </div>
  <div class="accordion-number" @click="expandBox">
    <p> {{ box.number }}</p>
  </div>
  <div class="accordion-name" @click="expandBox">
    <p>{{ box.name }}</p>
  </div>
  
  <div class="accordion-itemcount" @click="expandBox">
    <p>{{ box.items.length  }} items</p>
  </div>
  <div class="accordion-options">
    <i class="pi pi-ellipsis-h accordion-options-icon" @click="toggleBoxMenu($event)" />
    <Menu id="overlay_menu" ref="menu" :model="menuItems" :popup="true" />
  </div>
</div>

<div v-else class="accordion-container">
  <div class="accordion-icon">
    <i class="pi pi-angle-right"></i>
  </div>
  <div class="accordion-number">
   <Skeleton />
  </div>
  <div class="accordion-name">
    <Skeleton />
  </div>
  
  <div class="accordion-itemcount">
    <Skeleton />
  </div>
  <div class="accordion-options">
  </div>
</div>

<div v-show="expanded" class="accordion-content">
    <ItemAccordionDescription :boxId="box.boxId" />
    <ItemAccordion :boxId="box.boxId" :item="item" v-for="item in filter()">
      <template #name> <p :title="item.name"> {{trimString(40, item.name)}}</p></template>
      <template #description> <p :title="item.description">{{trimString(50, item.description)}} </p></template>
    </ItemAccordion>
    <br/>
</div>
</template>

<style scoped lang="scss">
.accordion-container {  
  /* grid props */
  cursor: pointer;
  display: grid;
  grid-template-columns: 0.4fr 0.4fr 2.9fr 1.1fr 0.4fr;
  grid-template-rows: 0.1fr;
  gap: 0px 10px;

  grid-template-areas:
    "icon number name itemcount options";

  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);

  background-color: white;
  border-radius: 3px;
  height: 40px;
  line-height: 40px;
  margin: 5px;
  color: black;
}

.accordion-container:hover {  
  background-color: #f7faf8
}


.accordion-content {  
  border-radius: 3px;
  background-color: #f7faf8;
  margin: 5px;
}

.accordion-icon { 
  grid-area: icon; 
  left: 25%;
}

.accordion-icon-icon { 
  height: 40px;
  line-height: 40px;
}

.accordion-number { 
  grid-area: number;
  margin-top: auto;
  margin-bottom: auto;
 }

.accordion-name { 
  grid-area: name; 
  margin-top: auto;
  margin-bottom: auto;
  font-size: 14px;
}

.accordion-itemcount {
  grid-area: itemcount;
  margin-top: auto;
  margin-bottom: auto;
  font-size: 12px;
  @media (min-width: 500px) {
  font-size: 14px;
  }
}

.accordion-options { 
  grid-area: options; 
}

.accordion-options-icon { 
  font-size: 12px;
  @media (min-width: 500px) {
  font-size: 14px;
  }
}

</style>
