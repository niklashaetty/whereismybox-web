<script setup lang="ts">
import router from '@/router';
import axios from 'axios';
import { onMounted, ref} from 'vue';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';

let boxes = ref();
const boxName = ref("");
const boxNumber = ref(0);
async function getBoxes(){
  var userId = router.currentRoute.value.params.userId;
  let boxesPath = `/api/users/${userId}/boxes`
  await axios.get(boxesPath)
        .then((response) => boxes.value = response.data)
}

async function postBox(number: any, name: any, ){
  var userId = router.currentRoute.value.params.userId;
  const postBoxRequest = { Number: number,  Name: name};
  let boxesPath = `/api/users/${userId}/boxes`
  await axios.post(boxesPath, postBoxRequest)
      .then((response) => console.log(response));
}

onMounted(async () => {
  await getBoxes();
  console.log("this is the boxes: " + boxes.value)
});

</script>
<template>
  <div>
    <h1>Boxes</h1>
    <div class="boxwrapper"> 
      <Card v-for="box in boxes">
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
                <Button type="submit" label="Submit" class="mt-2" />
            </div>
  </div>
</template>

<style scoped>
.boxwrapper {
  /* We first create a flex layout context */
  display: flex;

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
