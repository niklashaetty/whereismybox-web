<script setup lang="ts">
import router from '@/router';
import UserService from '@/services/userservice';
import Header from '@/components/Header.vue';
import { onMounted, ref } from 'vue';
import { useLoggedInUserStore } from '@/stores/loggedinuser';
import ProgressSpinner from 'primevue/progressspinner';

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
  <div class="loading-container">
    <ProgressSpinner style="width: 150px; height: 150px;" strokeWidth="1" 
    animationDuration="2s" aria-label="Custom ProgressSpinner" />
    <h2 class="text">Hold on! Loading your boxes as fast as possible</h2>
  </div>
  
</template>

<style scoped>
.loading-container{
  width: 300px;
  height: 300px;
  padding: 10px;
  border-radius: 3px;
  align-items: center;
  margin: auto;
  text-align: center;
  align-items: center;
}

.p-progress-spinner-circle{
  stroke:#f7faf8;

}

.text {
  color:#f7faf8
}

</style>