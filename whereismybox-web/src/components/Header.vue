<script lang="ts">
export default {
  props: ['userId']
}
</script>

<script setup lang="ts">

import {PrimeIcons} from 'primevue/api';
</script>

<template>
<div class="headercontainer">
  <div class="logo">
    <i class="fa-solid fa-box-open"></i>
    <i class="pi pi-box boxlogo" ></i>
    <h1> Where is my box?</h1>
  </div>
  <div class="filler"></div>
  <div class="username">{{ userId }}</div>
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

.username { grid-area: username; }
.filler { grid-area: filler; }

</style>
