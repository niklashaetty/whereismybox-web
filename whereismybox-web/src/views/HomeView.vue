<script setup lang="ts">
import router from '@/router';
import UserService from '@/services/userservice';
import Header from '@/components/Header.vue';
import { onMounted, ref } from 'vue';
import { useLoggedInUserStore } from '@/stores/loggedinuser';

const loggedInUser = useLoggedInUserStore()

function getCurrentUser(){
  UserService.getLoggedInUser()
  .then((user) => pushToCollection(user.primaryCollectionId))
  .catch(handleUserNotLoggedIn)
}

function pushToCollection(collectionId:string){
  router.push({path: `collections/${collectionId}`});
}

function handleUserNotLoggedIn(){
  console.log("Error!. Redirecting to login!:  ")
  pushToLoginPage();
}

function pushToLoginPage(){
  router.push("/login")
}

onMounted(async () => {
  getCurrentUser();
});

</script>
<template>
  <Header />
  Logging in the current user!
</template>

<style scoped>
</style>