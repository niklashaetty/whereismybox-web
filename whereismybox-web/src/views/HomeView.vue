<script setup lang="ts">
import router from '@/router';
import axios from 'axios';
import { ref } from 'vue';

const username = ref("");
const newUser = ref("");
let userId = ref("")


function createUser(){
  const createUserRequest = { username: username.value };
  
  axios.post('/api/users', createUserRequest)
      .then(response => router.push({path: `users/${response.data.userId}`}))

}

</script>

<template>
  <main>
    <h1>Where is my box?</h1>
    <p>Start by creating your own user</p>
    <div> 
       {{ newUser }}
      <input v-model="username" type="text" placeholder="Username"/>
      <button @click="createUser">Create</button>
    </div>
    <div v-if="userId">
      
       <p>Welcome {{username}}!</p>
       <button @click="$router.push({path: `/users/${userId}/boxes`})"> here </button>
       <p>Hint! Bookmark this page to be able to not lose access to your boxes:  </p>
      </div>
  
  </main>
</template>

<style scoped>

</style>