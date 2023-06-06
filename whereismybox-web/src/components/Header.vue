<script setup lang="ts">

import { ref, onMounted} from 'vue'
import router from '@/router';

import UserService from '@/services/userservice';
import Avatar from 'primevue/avatar';
import { useLoggedInUserStore } from '@/stores/loggedinuser'

const collectionId = router.currentRoute.value.params.collectionId as string;
const loggedInUserStore = useLoggedInUserStore()
const avatarLetter = ref("");

onMounted(async () => {
  await getCurrentUserInformation();
});

async function getCurrentUserInformation() {
  if(!loggedInUserStore.username){
    await UserService.getLoggedInUser();
  }
  avatarLetter.value = getAvatarLetter(loggedInUserStore.username);
}

function getAvatarLetter(username: string){
  return Array.from(username)[0].toUpperCase();
}

function pushToIndex(){
  router.push({ path: `/`});
}

</script>

<template>
<div class="headercontainer">
  <div class="logo" @click="pushToIndex()">
    <i class="fa-solid fa-box-open"></i>
    <i class="pi pi-box boxlogo" ></i>
    <h2 style="margin-left: 5px; "> Boxie</h2>
  </div>
  <div class="filler"></div>
  <div class="username"  @click="pushToIndex()">
    <Avatar v-if="avatarLetter" :label="avatarLetter"  style="background-color: #f7faf8" class="mr-2"  shape="circle" />
    <p style="margin-left: 10px">{{ loggedInUserStore.username }} </p>
  </div>
</div>
</template>

<style scoped>
.headercontainer {
  display: grid; 
  grid-template-columns: 0.7fr 1.9fr 0.8fr; 
  grid-template-rows: 0.2fr 1.8fr 1fr; 
  gap: 0px 0px; 
  grid-template-areas: 
    "logo filler username"
    ". . ."
    ". . ."; 
  width: 100%;
  height:60px;
}

.logo { 
  display: flex;
  grid-area: logo; 
  justify-content: center;
  justify-content: center;
  align-items: center;
  cursor: pointer;
}

.boxlogo {
  color: #f7faf8;
  font-size: 1rem;
  padding-right: 5px;
}

.username { 
  grid-area: username; 
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  height: 60px;
}
.filler { grid-area: filler; }

</style>
