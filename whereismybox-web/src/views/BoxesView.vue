<script setup lang="ts">
import router from '@/router';
import axios from 'axios';
import { onMounted, ref} from 'vue';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Button from 'primevue/button';

let boxes = ref();
const boxName = ref("");
const boxNumber = ref(0);
const userId = ref(router.currentRoute.value.params.userId);
const boxId = ref("");

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
  await getBoxes();
});

</script>
<template>
  <div>
 
    <h1>Boxes</h1>
    <div class="boxwrapper"> 
      <Card v-for="box in boxes" @click="$router.push({path: `/users/${userId}/boxes/${box.boxId}`})">
        <template #title>
            <div>{{box.number}} -  {{box.name}}</div>
            </template>
            <template #content>
              <Message class="message">This box has {{ box.items.length }} items in it.</Message>
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
.boxwrapper {
  /* We first create a flex layout context */
  display: flex;
  padding: 10px;

  /* Then we define the flow direction 
     and if we allow the items to wrap 
   * Remember this is the same as:
   * flex-direction: row;
   * flex-wrap: wrap;
   */
  flex-flow: row wrap;

  /* Then we define how is distributed the remaining space */
  justify-content: space-around;
}

@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    align-items: center;
  }
}
</style>
