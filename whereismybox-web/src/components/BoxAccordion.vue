<script setup lang="ts">
import { defineProps, computed, onMounted, watch} from 'vue'

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
import Dialog from 'primevue/dialog'

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
const userId = router.currentRoute.value.params.userId as string;
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
const linkToBox = ref("");
const isClicked = ref(false)

const expanded = computed(() => {
  return alwaysExpandedItems.value ||  isClicked.value
})

watch(searchQuery, (newValue, oldValue) => {
  console.log(newValue, oldValue);
  if (newValue) {
    isClicked.value = true
  } else {
    isClicked.value = false
  }
});

const menu: any = ref(null);
const menuItems = ref([
    {
        label: '',
        items: [
        {label: 'Go to box', icon: 'pi pi-qrcode',
        command: () => {
            router.push({path: `/users/${userId}/boxes/${box.value.boxId}`})
          }
        },
        {label: 'Show printable sticker', icon: 'pi pi-qrcode',
        command: () => {
            openStickerDialog();
          }
        },
        {label: 'Edit (not working currently)', icon: 'pi pi-file-edit',
        command: () => {
              console.log("Not working currently!")
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

function confirmDeleteBox(){
   
   confirm.require({
       message: `Are you sure you want to delete ${box.value.name}? This action cannot be undone, and all ${box.value.items.length} item(s) will be lost.`,
       header: `Deleting box ${box.value.number}`,
       icon: 'pi pi-info-circle',
       acceptClass: 'p-button-danger',
       accept: () => deleteBox(userId, box.value.boxId),
       reject: () => {}
   });
};

function deleteBox(userId:string, boxId:string) {
  BoxService.deleteBox(userId, boxId)
  .then((response => {
     toast.add({ severity: 'success', summary: 'Confirmed', detail: `Box ${box.value.name} deleted`, life: 3000 });
  }))
}

function expandBox(){
  isClicked.value = !isClicked.value
}

function toggleBoxMenu(event: MouseEvent)  { 
  menu.value?.toggle(event);
}

function filter()  { 
  if(box.value.items){
    console.log("Items found: "  + box.value.items.length + "\n query: " + searchQuery.value);
    return box.value.items.filter((item: Item) => item.name.toLowerCase().includes(searchQuery.value.toLowerCase()));
  }
  else return Array(0);
}

function trimString(maxLength: number, text: string) {
  return text.length > maxLength ? text.substring(0, maxLength - 3) + "..." : text;
}

onMounted(async () => {
  linkToBox.value = window.location.origin + router.currentRoute.value.path + "/boxes/" + box.value.boxId + "/";
});

</script>

<template>
    <Dialog v-model:visible="displayStickerDialog" :style="{ width: '450px' }" header="Printable QR code sticker" :modal="true">
         <Sticker :qrCodeLink="linkToBox" :boxNumber="box.number" />
        <template #footer>
          <Button label="Close" icon="pi pi-times" class="p-button-text" @click="closeStickerDialog" />
        </template>
      </Dialog>
<div v-if="isFullyLoaded" class="accordion-container">
  <div class="accordion-icon" @click="expandBox">
    <i v-if="!alwaysExpandedItems && expanded" class="pi pi-angle-down" style="color: slateblue; padding-right: 5px;"></i>
    <i v-else-if="!alwaysExpandedItems && !expanded" class="pi pi-angle-right" style="color: slateblue; padding-right: 5px;"></i>
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
    <Button style="color: #718355; " icon="pi pi-ellipsis-h" class="p-button-rounded p-button-text" @click="toggleBoxMenu($event)" aria-haspopup="true" aria-controls="overlay_menu" />
    <Menu id="overlay_menu" ref="menu" :model="menuItems" :popup="true" />
  </div>
</div>

<div v-else class="accordion-container">
  <div class="accordion-icon">
    <i class="pi pi-angle-right" style="color: slateblue; padding-right: 5px;"></i>
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

<style scoped>
.accordion-container {  
  /* grid props */
  cursor: pointer;
  display: grid;
  grid-template-columns: 0.4fr 0.4fr 2.9fr 1.1fr 0.4fr;
  grid-template-rows: 0.1fr;
  gap: 0px 10px;
  grid-auto-flow: row;
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
  margin: 5px;
  align-items: center;
}

.accordion-container:hover {  
  background-color: #f7faf8
}


.accordion-content {  
  margin-left: 5px;
  margin-right: 5px;
  margin-top: -5px;
  border-top: 0px;
  border-radius: 3px;
  background-color: #f7faf8;
  
}

.accordion-icon { 
  grid-area: icon; 
  margin: 0;
  left: 25%;
}

.accordion-number { 

  grid-area: number;
  align-items: center;
 }

.accordion-name { 
  grid-area: name; 
  align-items: center;
}

.accordion-itemcount {
   grid-area: itemcount;
   align-items: center; 
  }

.accordion-options { 
  height: 40px;
  grid-area: options; 
}

</style>
