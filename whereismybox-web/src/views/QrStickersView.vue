<script setup lang="ts">
import router from '@/router';
import axios from 'axios';
import { onMounted, ref } from 'vue';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import type Box from '@/models/Box';
import type Item from '@/models/Item';
import type UnattachedItem from '@/models/UnattachedItem';
import { BoxEvents } from '@/services/eventservice';
import ConfirmPopup from 'primevue/confirmpopup';
import Toast from 'primevue/toast'
import Header from '@/components/Header.vue'
import SectionTitle from '@/components/SectionTitle.vue'
import BoxAccordion from '@/components/BoxAccordion.vue';
import UnattachedItemAccordion from '@/components/UnattachedItemAccordion.vue';
import EventBus from '@/services/eventbus';
import BoxService from '@/services/boxservice';
import Vue3Html2pdf from 'vue3-html2pdf';

let box = ref<Box>(Object())
const boxName = ref("");
const collectionId = router.currentRoute.value.params.collectionId as string;
const boxId = router.currentRoute.value.params.boxId as string;
const loadingBox = ref(true);

let filteredItems = ref<Item[]>([]);
const filter = ref("");

async function getBox(showLoading: boolean) {
  if(showLoading){
    loadingBox.value = true;
  }
  BoxService.getBox(collectionId, boxId)
    .then((response) => {
      box.value = response.data
      filteredItems.value = response.data.items;
      boxName.value = response.data.boxName;
    })
    .then(() => {
      loadingBox.value = false});
}

onMounted(async () => {
  console.log("M08jted!")
  getBox(true);
});

/* Events */

EventBus.on(BoxEvents.ITEM_CHANGED,  () => { 
      getBox(false);
});


const filterBoxes = () => box.value?.items.filter((item) => !filter.value || item.name.toLowerCase().includes(filter.value.toLowerCase()))

function clearFilter() {
  filter.value = "";
}


function trimString(maxLength: number, text: string) {
  return text.length > maxLength ? text.substring(0, maxLength - 3) + "..." : text;
}

</script>
    <template  v-slot:pdf-content>
      <div class="container">
        <div class="boxes">
          <Sticker qrCodeLink="www.google.com" boxNumber="1" />
          <Sticker qrCodeLink="www.google.com" boxNumber="2" />
          <Sticker qrCodeLink="www.google.com" boxNumber="3" />
        </div>
      </div>
    </template>


<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300&display=swap');

.container {  
  margin: auto;
  min-height: 1000px;
  max-width: 800px;;
}

</style>
