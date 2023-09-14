<script setup lang="ts">

import { ref, onMounted} from 'vue'
import router from '@/router';

import UserService from '@/services/userservice';
import Avatar from 'primevue/avatar';
import Menu from 'primevue/menu';
import { useLoggedInUserStore } from '@/stores/loggedinuser'

const collectionId = router.currentRoute.value.params.collectionId as string;
const loggedInUserStore = useLoggedInUserStore()
const avatarLetter = ref("");

const menu: any = ref(null);
const menuItems = ref([
    {
        label: 'Dashboard',
        icon: 'pi pi-home',
        command: () => pushToDashboard()
            
    },
    { separator: true }
]);

onMounted(async () => {
  await getCurrentUserInformation();
});

async function getCurrentUserInformation() {
  if(loggedInUserStore.username){
    avatarLetter.value = getAvatarLetter(loggedInUserStore.username);
  }
  else {
    await UserService.getRegisteredUser();
    avatarLetter.value = getAvatarLetter(loggedInUserStore.username);
  }
}

function getAvatarLetter(username: string){
  return Array.from(username)[0].toUpperCase();
}

function pushToIndex(){
  router.push({ path: `/`});
}

function pushToDashboard(){
  router.push({ path: `/dashboard`});
}

function toggleBoxMenu(event: MouseEvent)  { 
  menu.value?.toggle(event);
}

</script>

<template>
<div class="headercontainer">
  <div class="logo" @click="pushToIndex()">
    <i class="fa-solid fa-box-open"></i>
    <i class="pi pi-box boxlogo"></i>
    <h2 class="logo-text"> Boxie</h2>
  </div>
  <div class="filler"></div>
  <div class="username" >
    <Avatar v-if="avatarLetter" :label="avatarLetter"  style="background-color: #f7faf8" class="mr-2"  shape="circle" @click="toggleBoxMenu($event)"/>
    <Menu id="overlay_menu" :model="menuItems" ref="menu"  :popup="true">
            <template #end>
                <button style="height: 30px;padding-left: 10px; align-items: center; display: flex;" class="p-link">
                    <i class="pi pi-sign-out" />
                    <a href="/.auth/logout?post_logout_redirect_uri=/" style="padding-left: 10px;">Log out</a>
                </button>
            </template>
    </Menu>
    <p class="username-text">{{ loggedInUserStore.username }} </p>
  </div>
</div>
</template>

<style scoped lang="scss">
.headercontainer {
  display: grid; 
  grid-template-columns: 0.7fr 1.9fr 0.01fr 0.8fr; 
  grid-template-rows: 0.2fr; 
  gap: 0px 0px; 
  grid-template-areas: 
    "logo filler logout username";
  width: 100%;
  height: 60px;
}

.logo { 
  display: flex;
  grid-area: logo; 
  justify-content: center;
  justify-content: center;
  align-items: center;
  cursor: pointer;
}

.logo-text {
  font-size: 30px;
  margin-left: 5px; 
  color: white;
}

.boxlogo {
  color: #f7faf8;
  padding-right: 5px;
  font-size: 20px;
}

.username { 
  grid-area: username; 
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  height: 60px;
}

.logout { 
  grid-area: logout; 
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  height: 60px;
}

.username-text{
  color: white;
  margin-left: 10px;
  display: none;
  @media (min-width: 1000px) {
    display: inline;
  }
}

.filler { grid-area: filler; }

</style>
