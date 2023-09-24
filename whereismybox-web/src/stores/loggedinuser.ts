import { defineStore } from 'pinia'

export const useLoggedInUserStore = defineStore('loggedInUser', {
    state: () => {
      return { 
        username: "",
        userId: ""
     }
    },
    actions: {
      setLoggedInUser(userId: string, username: string) {
        this.userId = userId;
        this.username = username;
      },
    },
  })