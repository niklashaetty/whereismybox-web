import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', {
    state: () => {
      return { 
        username: "",
        userId: "",
        primaryCollectionId: ""
     }
    },
    actions: {
      setUser(userId: string, username: string, primaryCollectionId: string) {
        this.userId = userId;
        this.username = username;
        this.primaryCollectionId = primaryCollectionId;
      },
    },
  })