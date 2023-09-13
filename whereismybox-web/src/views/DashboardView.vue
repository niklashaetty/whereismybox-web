<script setup lang="ts">
import router from '@/router';
import { computed, onMounted, ref } from 'vue';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import DataView from 'primevue/dataview'
import Card from 'primevue/card'
import Header from '@/components/Header.vue'
import SectionTitle from '@/components/SectionTitle.vue'
import UserService from '@/services/userservice';
import { useLoggedInUserStore } from '@/stores/loggedinuser'
import type Contributor from '@/models/Contributor';

const loggedInUserStore = useLoggedInUserStore()

let sharedCollectionIds = ref([]);
let sharedCollections = ref(new Map());
let sharedCollectionsLoaded = ref(false);

// Share collection dialog
const disableAddContributor = ref(false);
const displayManageCollectionAccessDialog = ref(false)
let contributors = ref<Contributor[]>([]);
let contributorsLoaded = ref(false);
const newContributorUsername = ref("");

function openManageCollectionAccessDialog() {
  getContributors()
  .then(() => displayManageCollectionAccessDialog.value = true)
}

async function getContributors() {
  UserService.getCollectionContributors(loggedInUserStore.primaryCollectionId)
  .then((c) => contributors.value = c.data)
  .then(() => contributorsLoaded.value = true)
}

function removeContributor(userId: string, collectionId: string) {
  UserService.deleteCollectionContributor(userId, collectionId)
  .then(getContributors)
}

function addContributor(username: string, collectionId: string) {
  disableAddContributor.value = true;
  UserService.addCollectionContributor(username, collectionId)
  .then(getContributors)
  .then(() => {
    disableAddContributor.value = false;
    newContributorUsername.value = "";
  })
}

function closeManageCollectionAccessDialog() {
  contributorsLoaded.value = false;
  contributors.value = [];
  displayManageCollectionAccessDialog.value = false;
  newContributorUsername.value = "";
  disableAddContributor.value = false;
}

onMounted(async () => {
  await getCurrentUserInformation();
  sharedCollectionIds.value = loggedInUserStore.sharedCollectionIds;
  getCurrentSharedCollections(sharedCollectionIds.value);
});

async function getCurrentUserInformation() {
  if(!loggedInUserStore.username){
    await UserService.getRegisteredUser();
  }
}

async function getCurrentSharedCollections(sharedCollectionIds: string[]) {
  
  for (const sharedCollectionId of sharedCollectionIds) {
    const collectionOwnerResponse = await UserService.getCollectionOwner(sharedCollectionId);
    sharedCollections.value.set(sharedCollectionId, collectionOwnerResponse.data.username)
  }
  sharedCollectionsLoaded.value = true;
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
            <template #title> <p style="font-size: 18px;">Collection with id {{loggedInUserStore.primaryCollectionId}}</p> </template>
            <template #subtitle> <p>Created by you</p> </template>
          
            <template #footer>
                <Button icon="pi pi-check" label="Open" @click="pushToCollection(loggedInUserStore.primaryCollectionId)"/>
                <Button icon="pi pi-share-alt" label="Share" severity="secondary" style="margin-left: 0.5em" @click="openManageCollectionAccessDialog()"/>
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
          <Card v-show="sharedCollectionsLoaded" class="shared-collection-card" v-for="collectionId in loggedInUserStore.sharedCollectionIds">
            <template #title> <p style="font-size: 18px;">Collection with id {{collectionId}}</p> </template>
            <template #subtitle><p>Shared by {{ sharedCollections.get(collectionId) }}</p></template>
            <template #footer>
                <Button icon="pi pi-check" label="Open" @click="pushToCollection(collectionId)"/>
            </template>
          </Card>
        </div>
      </div>
    </div>
  </div>
</div>


<Dialog v-model:visible="displayManageCollectionAccessDialog" :style="{ width: '650px' }" header="Share collection" :modal="true">
  <div class="card">
    <p style="margin-bottom: 10px;"> Once a collection is shared with someone, they can access and edit the collection until you revoke their access here.</p>
    
    <DataView dataKey="userId" v-show="contributorsLoaded" :value="contributors">
            <template #header>
              Collection shared with
            </template>
            <template #list="slotProps">
                <div class="col-12">
                    <div class="flex flex-column xl:flex-row xl:align-items-start p-2 gap-1">
                        <div class="flex flex-column sm:flex-row justify-content-between align-items-center xl:align-items-start flex-1 gap-1">
                            <div class="flex flex-column align-items-center sm:align-items-start gap-1">
                                <div class="text-l text-1000">{{ slotProps.data.username }}</div>
                            </div>
                            <div class="flex sm:flex-column align-items-center sm:align-items-end gap-1 sm:gap-1">
                                <Button @click="removeContributor(slotProps.data.userId, loggedInUserStore.primaryCollectionId)" p-button-sm icon="pi pi-times" rounded outlined  severity="danger"></Button>
                            </div>
                        </div>
                    </div>
                </div>
            </template>
        </DataView>
  </div>
  <div style="margin-top: 30px;display: flex;">   
        <span class="p-float-label">
          <InputText v-show="!disableAddContributor" id="username" v-model="newContributorUsername" />
          <InputText disabled v-show="disableAddContributor" id="username" v-model="newContributorUsername" />
          <label v-show="!disableAddContributor" for="username">Username</label>
        </span>
        <Button style="width:110px;" v-show="!disableAddContributor" class="youtube p-0" @click="addContributor(newContributorUsername, loggedInUserStore.primaryCollectionId)" outlined>
          <i class="pi pi-share-alt px-2"></i>
          <span class="px-3">Share</span>
        </Button>

        <Button style="width:110px;" disabled v-show="disableAddContributor" class="youtube p-0" outlined>
          <i class="pi pi-spin pi-spinner px-2"></i>
          <span class="px-3">Share</span>
        </Button>

        
      </div> 
  <template #footer>
    <Button label="Close" icon="pi pi-times" class="p-button-text" @click="closeManageCollectionAccessDialog" />
  </template>
</Dialog>
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
  width: 20em;

  font-family: 'Roboto', sans-serif;
  @media (min-width: 1000px) {
    width: 40em;
  }
}

.c-bc-boxes::v-deep .p-card .p-card-content {
  padding: 0 !important;
}

.c-bc-boxes::v-deep .p-card .p-card-body {
  padding: 10px !important;
}

.c-ui-unattacheditems::v-deep .p-card .p-card-content {
  padding: 0 !important;
}

.c-ui-unattacheditems::v-deep .p-card .p-card-body {
  padding: 10px !important;
}



.p-card-content {
  padding: 0px;
}

.shared-collection-card {
  width: 20em;
  margin-bottom: 10px;
  font-family: 'Roboto', sans-serif;
  @media (min-width: 1000px) {
    width: 40em;
  }
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
