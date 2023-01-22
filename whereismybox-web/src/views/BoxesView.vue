<script setup lang="ts">
import router from '@/router';
import axios from 'axios';
import { onMounted, ref} from 'vue';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Button from 'primevue/button';
import Skeleton from 'primevue/skeleton'
import ProgressSpinner from 'primevue/progressspinner';

let boxes = ref();
const boxName = ref("");
const boxNumber = ref(0);
const userId = ref(router.currentRoute.value.params.userId);
const loadingBoxes = ref(false);

async function getBoxes(){
  let boxesPath = `/api/users/${userId.value}/boxes`
  await axios
  .get(boxesPath)
  .then((response) => boxes.value = response.data)
}

async function createNewBox(){
  const postBoxRequest = { Number: boxNumber.value,  Name: boxName.value};
  let boxesPath = `/api/users/${userId.value}/boxes`
  await axios.post(boxesPath, postBoxRequest)
      .then(getBoxes);
}

onMounted(async () => {
  loadingBoxes.value = true;
  await new Promise(r => setTimeout(r, 3000));
  await getBoxes();
  loadingBoxes.value = false;
});

</script>
<template>
  <div class="wrapper">
    <h1>Boxes</h1>
    <div v-if="loadingBoxes" class="boxescontainer"> 
      <Card v-for="box in new Array(8)" class="boxcard">
            <template #title>
              <Skeleton width="10rem" class="mb-2"></Skeleton>
            </template>
            <template #subtitle>
              <Skeleton width="10rem" class="mb-2"></Skeleton>
            </template>
      </Card>
    </div>
    <div v-else class="boxescontainer"> 
      <Card v-for="box in boxes" @click="$router.push({path: `/users/${userId}/boxes/${box.boxId}`})" class="boxcard">
            <template #title>
              <p class="boxtitle">{{box.number}} -  {{box.name}}</p>
            </template>
            <template #subtitle>
              {{ box.items.length }} items
            </template>
            <template #footer style="align-items: center;">
              <div class="footer-button"><Button icon="pi pi-file-edit" /></div>
              <div class="footer-button"><Button icon="pi pi-qrcode"/></div>
               
            </template>
      </Card>
      
    </div>
    <div v-focustrap class="card">
                <div class="field">
                    <InputText v-model="boxName" type="text" placeholder="Name" autofocus />
                </div>
                <div class="field">
                    <InputNumber v-model="boxNumber" :min="0" :max="100" placeholder="Name" autofocus />
                </div>
                <Button @click="createNewBox" type="submit" label="Create new box" class="mt-2" />
      </div>
  </div>
</template>

<style scoped>

.boxtitle {
  font-size: medium;
}
.boxescontainer {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
}

.wrapper {
  align-items: center;
}

.footer-button {
  display: inline-block;
  justify-content: space-between;
  padding:5px;
  align-items: center;
}

.boxcard {
  background-color: white;
  border-radius: 25px;
  width: 250px;
  height: 200px;
  padding-bottom: 10%; /* 32:18, i.e. 16:9 */
  margin-bottom: 2%; /* (100-32*3)/2 */
}


.boxcard:hover {
  background-color: #f3faff;
  cursor: pointer;
}
</style>
