<script setup lang="ts">

import {PrimeIcons} from 'primevue/api';
import { defineProps, computed, ref, onMounted} from 'vue'
import router from '@/router';

import UserService from '@/services/userservice';
import Avatar from 'primevue/avatar';

const userId = router.currentRoute.value.params.userId as string;
const userName = ref(" ");
const avatarLetter = ref(" ");

function getCurrentUser() {
  UserService.getUser(userId)
  .then((response => {
    userName.value = response.data.userName;
    avatarLetter.value = Array.from(userName.value)[0];
  }))
}

onMounted(async () => {
  getCurrentUser();
});
</script>

<template>
<div class="headercontainer">
  <div class="logo">
    <i class="fa-solid fa-box-open"></i>
    <i class="pi pi-box boxlogo" ></i>
    <h1 style="margin-left: 10px"> Where is my box?</h1>
  </div>
  <div class="filler"></div>
  <div class="username"  @click="$router.push({ path: `/users/${userId}`})">
    <Avatar :label="avatarLetter" class="mr-2" size="large" shape="circle" />
    <p style="margin-left: 10px">{{ userName }} </p>
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
  background-color: aliceblue;

  border-bottom: 1px solid;
  border-color: white;
}
.logo { 
  display: flex;
  grid-area: logo; 
  justify-content: center;
  justify-content: center;
  align-items: center;
}

.boxlogo {
  font-size: 2rem;
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
