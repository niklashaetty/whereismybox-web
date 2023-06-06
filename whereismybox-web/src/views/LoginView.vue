<script setup lang="ts">
import router from '@/router';
import axios from 'axios';
import { ref } from 'vue';
import UserService from '@/services/userservice';
import Header from '@/components/Header.vue'
import Button from 'primevue/button'

const username = ref("");
const newUser = ref("");
let userId = ref("")


function gitHubLogin(){
  router.push({path: `.auth/login/github`});
}

</script>
<template>
  <Header />
  <div class="logincontainer" >
    <div class="loginsection" @click="gitHubLogin()">
      <div class="logo"> 
        <img src="../assets/github-logo.svg" width="50px" height="50px"> 
      </div>
      <div class="text" >
        <p class="logintext" > Login with Github</p>
        
      </div>
    </div>
  </div>
</template>

<style scoped>
.logincontainer {
  margin-top: 10px;
  width: 400px;
  height: 500px;
  padding: 10px;
  border-radius: 3px;
  align-items: center;
  user-select: none; /* Non-prefixed version, currently */
  -webkit-user-select: none;
  display: block;
  margin-left: auto;
  margin-right: auto;
}

.loginsection{
  margin-bottom: 5px;
  height: 80px;
  border-radius: 3px;
  background-color: aqua;
  display: flex;
  text-align: center;
  align-items: center;

  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
}
.logo{
  width: 100px;
}

.loginbutton{
  margin-top: auto;
  text-align: center;
  display: flex;
  justify-content: center;
}



</style>