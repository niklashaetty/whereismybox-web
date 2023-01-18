<script setup lang="ts">
import axios from 'axios';
import { ref } from 'vue';
import TheWelcome from '../components/TheWelcome.vue'

const username = ref("");
const newUser = ref("");
let userId = ref("")


function createUser(){
  const createUserRequest = { username: username.value };
  axios.post('/api/users', createUserRequest)
      .then(response => userId.value = response.data.userId);
}

</script>

<template>
  <main>
    <h1>Titel</h1>
    <p>Start by creating your own user</p>
    <div> 
       {{ newUser }}
      <input v-model="username" type="text" placeholder="Username"/>
      <button @click="createUser">Create</button>
    </div>
    <div v-if="userId">
      
       <p>Perfect {{username}}! The homepage for your user can be found here</p>
      
       <button @click="$router.push({path: `/users/${userId}/boxes`})">Go to your new homepage!</button>

       <p>Hint! Bookmark this page to be able to not lose access to your boxes:  </p>
      </div>
  
  </main>
</template>

<style scoped>

</style>