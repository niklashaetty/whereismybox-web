<script setup lang="ts">
import router from '@/router';
import UserService from '@/services/userservice';
import Header from '@/components/Header.vue';
import { onMounted, ref } from 'vue';
import ProgressSpinner from 'primevue/progressspinner';

const isLoadingSlow = ref(false);
const isLoadingSuperSlow = ref(false);

async function getCurrentUser(){
  var isUserAuthenticated = await UserService.isUserAuthenticated();
  if(isUserAuthenticated){
  await UserService.getRegisteredUser()
  .then((user) => pushToDashboard())
  .catch(pushToRegistration)
  } else {
    pushToLogin();
  }
}
function pushToDashboard(){
  router.push({path: `/dashboard`});
}

function pushToLogin(){
  router.push("/login")
}

function pushToRegistration(){
  router.push("/register")
}

onMounted(async () => {
  setTimeout(() => isLoadingSlow.value = true, 1000)
  setTimeout(() => isLoadingSuperSlow.value = true, 2000)
  getCurrentUser();
});

</script>
<template>
  <Header />
  <div v-show="isLoadingSlow" class="loading-container">
    <ProgressSpinner style="width: 150px; height: 150px;" strokeWidth="1" 
    animationDuration="2s" aria-label="Loading.." />
    <h2 v-show="isLoadingSuperSlow" class="text">Hold on! Loading your boxes is taking longer than expected...</h2>
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