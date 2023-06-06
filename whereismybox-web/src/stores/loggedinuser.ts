import { defineStore } from 'pinia'

export const useLoggedInUserStore = defineStore('loggedInUser', {
    state: () => {
      return { 
        username: "",
        userId: "",
        primaryCollectionId: ""
     }
    },
    actions: {
      setLoggedInUser(userId: string, username: string, primaryCollectionId: string) {
        this.userId = userId;
        this.username = username;
        this.primaryCollectionId = primaryCollectionId;
      },
    },
  })