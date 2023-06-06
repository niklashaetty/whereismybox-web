import User from '@/models/User';
import axios from 'axios';
import { useLoggedInUserStore } from '@/stores/loggedinuser'

export default new class UserService {

  async createUser(username:string){
    const createUserRequest = { username: username };
    return axios.post('/api/users', createUserRequest);
  }

  async getUser(userId:string){
    let path = `/api/users/${userId}`
    return axios.get(path);
  }

  async getUserByCollectionId(collectionId:string){
    let path = `/api/users?primaryCollectionId=${collectionId}`
    return axios.get(path);
  }

  async getLoggedInUser(){
    const loggedInUser = useLoggedInUserStore();
    const path = `/api/users/me`
    try {
      const res = await axios.get(path);
      let user = new User(res.data.userId, res.data.username, res.data.primaryCollectionId);
      loggedInUser.setLoggedInUser(user.userId, user.username, user.primaryCollectionId);
      return user;
    }
    catch(e){
      console.log("Error!: " + e);
      throw new Error("UserNotFound");
    }
  }
} 
