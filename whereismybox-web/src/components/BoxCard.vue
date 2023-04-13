<script setup lang="ts">
import { defineProps, computed} from 'vue'

import ConfirmDialog from 'primevue/confirmdialog';
import router from '@/router';
import Button from 'primevue/button';
import Menu from 'primevue/menu';
import BoxService from '@/services/boxservice';
import {PrimeIcons} from 'primevue/api';
import { onMounted, ref } from 'vue';
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";


const props = defineProps({
  boxId: {
      type: String,
      required: true
    },
    boxNumber: {
      type: Number,
      required: true
    },
    boxName: {
      type: String,
      required: true
    },
    boxItemCount: {
      type: Number,
      required: true
    }
});
const userId = router.currentRoute.value.params.userId as string;
const boxId = computed(() => props.boxId);
const boxName = computed(() => props.boxName);
const boxItemCount = computed(() => props.boxItemCount);
const boxNumber = computed(() => props.boxNumber);

const toast = useToast();
const confirm = useConfirm();
const menu: any = ref(null);
const menuItems = ref([
    {
        label: 'Options',
        items: [
        {label: 'Show printable sticker', icon: 'pi pi-qrcode',
        command: () => {
            console.log("Not working currently!")
          }
        },
        {label: 'Edit (not working currently)', icon: 'pi-file-edit',
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


function confirmDeleteBox(){
   
    confirm.require({
        message: `Are you sure you want to delete ${boxName.value}? This action cannot be undone, and all ${boxItemCount.value} items will be lost.`,
        header: `Deleting box ${boxNumber.value}`,
        icon: 'pi pi-info-circle',
        acceptClass: 'p-button-danger',
        accept: () => deleteBox(userId, boxId.value),
        reject: () => {}
    });
};

function toggleBoxMenu(event: MouseEvent)  { 
  menu.value?.toggle(event);
}

function deleteBox(userId:string, boxId:string) {
  BoxService.deleteBox(userId, boxId)
  .then((response => {
     toast.add({ severity: 'success', summary: 'Confirmed', detail: `Box ${boxName.value} deleted`, life: 3000 });
  }))
}

</script>

<template>
<div class="boxcardcontainer">
  <div class="boxnumber" @click="$router.push({ path: `/users/${userId}/boxes/${boxId}`})">
    <p>{{ boxNumber }}</p>
  </div>
  <div class="boxname" @click="$router.push({ path: `/users/${userId}/boxes/${boxId}`})">
    <p>{{ boxName }}</p>
    <div class="boxitemcount"  @click="$router.push({ path: `/users/${userId}/boxes/${boxId}`})">
      <p>{{boxItemCount}} items</p>
    </div>
  </div>
  <div class="boxoptions">
    <Button style="color: #718355; " icon="pi pi-ellipsis-h" class="p-button-rounded p-button-text" @click="toggleBoxMenu($event)" aria-haspopup="true" aria-controls="overlay_menu" />
    <Menu id="overlay_menu" ref="menu" :model="menuItems" :popup="true" />
  </div>
  <div class="footer" @click="$router.push({ path: `/users/${userId}/boxes/${boxId}`})"> </div>
  <div class="boxcontent" @click="$router.push({ path: `/users/${userId}/boxes/${boxId}`})"></div>
</div>
</template>

<style scoped>
  @import url('https://fonts.googleapis.com/css2?family=Roboto&display=swap');

.boxcardcontainer {  
  display: grid;
  grid-template-columns: 0.7fr 2.4fr 0.3fr; 
  grid-template-rows: 0.7fr 0.1fr 2.9fr 0.3fr; 
  gap: 0px 0px; 
  grid-template-areas: 
    "boxnumber boxname boxoptions"
    "boxcontent boxcontent boxcontent"
    "boxcontent boxcontent boxcontent"
    "footer footer footer"; 

  padding: 10px;
  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
}

.boxnumber { 
  grid-area: boxnumber; 
  text-align: center;
  font-size: 20px;
  border-bottom: 1px solid;
  border-color: grey;
  font-family: 'Roboto', sans-serif;

}

.boxname {
  grid-area: boxname;
  text-align: center;
  font-family: 'Roboto', sans-serif;
}

.boxitemcount { 
  color: grey;
  font-size: 10px;
 }

.boxoptions { 
  grid-area: boxoptions; 
  font-family: 'Roboto', sans-serif;
  font-size: 20px;
}

.footer { grid-area: footer; }

.boxcontent { 
  grid-area: boxcontent; 
  padding: 10px;
}



</style>
