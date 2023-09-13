import { defineStore } from 'pinia'

export const useLoggedInUserStore = defineStore('loggedInUser', {
    state: () => {
      return { 
        username: "",
        userId: "",
        primaryCollectionId: "",
        sharedCollectionIds: []
     }
    },
    actions: {
      setLoggedInUser(userId: string, username: string, primaryCollectionId: string, sharedCollectionId: []) {
        this.userId = userId;
        this.username = username;
        this.primaryCollectionId = primaryCollectionId;
        this.sharedCollectionIds = sharedCollectionId;
      },
    },
  })