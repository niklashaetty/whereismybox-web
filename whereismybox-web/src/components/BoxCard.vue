<script lang="ts">
export default {
  props: ['boxNumber', 'boxName', 'boxItemCount']
}
</script>

<script setup lang="ts">

import {PrimeIcons} from 'primevue/api';
</script>

<template>
<div class="boxcardcontainer">
  <div class="boxnumber">
    <p>{{ boxNumber }}</p>
  </div>
  <div class="boxname">
    <p>{{ boxName }}</p>
    <div class="boxitemcount">
      <p>{{boxItemCount}} items</p>
    </div>
  </div>
  <div class="boxoptions">...</div>
  <div class="footer"></div>
  <div class="boxcontent"></div>
</div>
</template>

<style scoped>
  @import url('https://fonts.googleapis.com/css2?family=Roboto&display=swap');

.boxcardcontainer {  display: grid;
  grid-template-columns: 0.7fr 2.4fr 0.3fr; 
  grid-template-rows: 0.7fr 0.1fr 2.9fr 0.3fr; 
  gap: 0px 0px; 
  grid-template-areas: 
    "boxnumber boxname boxoptions"
    "boxcontent boxcontent boxcontent"
    "boxcontent boxcontent boxcontent"
    "footer footer footer"; 

  padding: 10px;
  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
}

.boxnumber { 
  grid-area: boxnumber; 
  text-align: center;
  font-size: 20px;
  border-bottom: 1px solid;
  border-color: grey;
  font-family: 'Roboto', sans-serif;

}

.boxname {
  grid-area: boxname;
  text-align: center;
  font-family: 'Roboto', sans-serif;
}

.boxitemcount { 
  color: grey;
  font-size: 10px;
 }

.boxoptions { 
  grid-area: boxoptions; 
  font-family: 'Roboto', sans-serif;
  font-size: 20px;
}

.footer { grid-area: footer; }

.boxcontent { 
  grid-area: boxcontent; 
  padding: 10px;
}



</style>
