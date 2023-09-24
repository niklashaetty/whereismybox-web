<script setup lang="ts">
import router from '@/router';
import UserService, { UsernameExistsError } from '@/services/userservice';
import Header from '@/components/Header.vue';
import { onMounted, ref } from 'vue';
import { useLoggedInUserStore } from '@/stores/loggedinuser';
import ProgressSpinner from 'primevue/progressspinner';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import { useField, useForm } from 'vee-validate';
import { useToast } from "primevue/usetoast";
import User from '@/models/User';

const loggedInUser = useLoggedInUserStore()
const userExists = ref(true);
const toast = useToast();
const value = ref("");

async function registerUser(){
  UserService.registerUser(value.value)
  .then(UserService.getRegisteredUser)
  .then(() => pushToDashboard())
  .catch(handleCreateUserError);
}

function pushToDashboard(){
  router.push({path: `dashboard`});
}

function handleCreateUserError(error: Error){
}

</script>
<template>
  <Header />
  <div class="createuser-container">
    <div class="create-user-title">
      <div class="create-user-text">
        <h2 class="welcome-title">Welcome to Boxie!</h2>
        <p class="welcome-text">It looks like it's your first time here. 
          Choose a unique username and get started in a second </p>
        </div>
      <div class="create-user-form">

        <span class="p-float-label">
          <InputText id="username" v-model="value" />
          <label for="username">Username</label>
        </span>
        <Button @click="registerUser" style="margin-top: 20px; margin-left: auto;margin-right: auto;" type="submit" label="Start using Boxie!" text raised /> 
    </div>
    </div>
  </div>
  <Toast />
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300&display=swap');
.createuser-container{
  width: 300px;
  height: 300px;
  padding: 10px;
  margin: auto;
  background-color: #f7faf8;
  border-radius: 3px;
  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
}

.create-user-form {
  margin-top: 30px;
  margin-left: auto;
  margin-right: auto;
}

.welcome-text {
  text-align: center;
}

.welcome-title {
  text-align: center;
}


</style>
