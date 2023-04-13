<script setup lang="ts">
import { defineProps, computed} from 'vue'

import ConfirmDialog from 'primevue/confirmdialog';
import router from '@/router';
import Button from 'primevue/button';
import Menu from 'primevue/menu';
import BoxService from '@/services/boxservice';
import Item from '@/models/Item';
import {PrimeIcons} from 'primevue/api';
import { onMounted, ref } from 'vue';
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";


</script>

<template>
<div class="itemaccordion-container-description">
  <div class="itemaccordion-name-description">
    <p> Name </p>
  </div>
  <div class="itemaccordion-description-description">
    <p> Description</p>
  </div>
  <div class="itemaccordion-options-description">
  </div>
</div>
</template>

<style scoped>


.itemaccordion-container-description {  display: grid;
  grid-template-columns: 1fr 1.7fr 0.3fr;
  grid-template-rows: 0.1fr;
  grid-auto-columns: 1fr;
  gap: 0px 10px;
  grid-auto-flow: row;
  grid-template-areas:
    "name description options";
  background-color:none;
  height: 25px;
  border-bottom: 1px solid;
  border-color: #e9f5db;
  font-weight: 900;
  color: #40513B;
  font-size: 15px;
  padding: 2px;
  padding-left: 10px;
}

.itemaccordion-name-description { 
  grid-area: name; 
}

.itemaccordion-description-description { grid-area: description; }

.itemaccordion-options-description { grid-area: options; }
</style>
