<script setup lang="ts">
import router from '@/router';
import { computed, onMounted, ref } from 'vue';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import Card from 'primevue/card'
import Header from '@/components/Header.vue'
import SectionTitle from '@/components/SectionTitle.vue'
import UserService from '@/services/userservice';
import { useLoggedInUserStore } from '@/stores/loggedinuser'

const loggedInUserStore = useLoggedInUserStore()

onMounted(async () => {
  await getCurrentUserInformation();
});

async function getCurrentUserInformation() {
  if(!loggedInUserStore.username){
    await UserService.getRegisteredUser();
  }
}

async function pushToCollection(collectionId: string) {
  router.push({path: `/collections/${collectionId}`});
}

</script>
<template>
<Header />
<div class="wrapper">
  <div class="container">
    <div class="content">
      <div class="c-boxcollection">
        <div class="sectiontitle">
          <SectionTitle title="My collection" />
        </div>
        <div class="c-bc-boxes">
          <Card class="my-collection-card">
            <template #title> {{loggedInUserStore.primaryCollectionId}} </template>
            <template #subtitle> Created by you </template>
            <template #footer>
                <Button icon="pi pi-check" label="Open" @click="pushToCollection(loggedInUserStore.primaryCollectionId)"/>
                <Button icon="pi pi-share-alt" label="Share" severity="secondary" style="margin-left: 0.5em" />
            </template>
        </Card>
        </div>
      </div>
      <div class="c-unattacheditems">
        <div class="sectiontitle">
          <SectionTitle title="Shared collections" >
            <template #right>
              <i class="pi pi-info-circle boxie-icon" 
              v-tooltip.top="{ value: `<p>When someone shares a collection with you, it will show up here!</p>`, escape: true, class: 'custom-error' }"/>
            </template>
          </SectionTitle>
        </div>
        <div class="c-ui-unattacheditems">
          <Card style="width: 25em; margin-bottom: 10px;" v-for="collectionId in loggedInUserStore.sharedCollectionIds">
            <template #title> {{collectionId}} </template>
            <template #subtitle> Shared by someone</template>
            <template #footer>
                <Button icon="pi pi-check" label="Open" @click="pushToCollection(collectionId)"/>
            </template>
        </Card>
        </div>
      </div>
    </div>
  </div>
</div>
</template>

<style scoped lang="scss">
@import 'primeflex/primeflex.scss';
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300&display=swap');
.container {
  width: 95%;
  max-width: 1250px;  
  min-width: 350px;
  min-height: 500px;
  margin: auto;
  padding: 10px;
  @media (min-width: 1000px) {
    width: 80%;
  }
  @media (min-width: 500px) {
    width:90%;
  }
}

.searchbar {
  display: flex;
  width: 100%;
  margin-left: auto;
  margin-right: auto;
  height: 50px;
  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
  background-color: white;
  border-radius: 3px;
}


.my-collection-card {
  width: 25em;
  font-family: 'Roboto', sans-serif;
  @media (min-width: 1000px) {
    width: 40em;
  }
}

.shared-collection-card {
  width: 25em;
  @media (min-width: 1000px) {
    width: 40em;
  }
}

.searchinput{
  width: 100%;
  height: 100%;
}

.collection-sharee{
  display: flex;
  height: 30px;
  line-height: 30px;
}

.add-new-icon {
  color: white;
}

.content{
  width: 100%;
  height: 100%;
  @media (min-width: 1000px) {
    display: flex;
  }
}

.c-boxcollection {
  display: flex;
  flex-direction: column;
  width: 100%;
  margin-top: 15px;
  margin-left: auto;
  margin-right: auto;
  @media (min-width: 1000px) {
    width: 65%;
  }
  @media (min-width: 1400px) {
    width: 70%;
  }
}

.sectiontitle{
  width: 100%;
}

.c-bc-boxes {
  width: 100%;
}

.c-bc-boxes-empty{
  width: 100%;
}
.c-bc-boxes-empty-text{
  margin-top: 20px;
  color:white;
}

.clickable {
  cursor: pointer;
}

.c-unattacheditems {
  width: 100%;
  margin-top: 15px;
  margin-left: auto;
  margin-right: auto;
  @media (min-width: 1000px) {
    width: 35%;
  }
  @media (min-width: 1400px) {
    width: 30%;
  }
}

.boxie-icon {
  width: 20px;
  height: 20px;
  font-size: 16px;
  text-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
  color: white;
  @media (min-width: 500px) {
    font-size: 20px;
  }
}


</style>
