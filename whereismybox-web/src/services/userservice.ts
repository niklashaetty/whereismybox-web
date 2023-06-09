import User from '@/models/User';
import axios from 'axios';
import { useLoggedInUserStore } from '@/stores/loggedinuser'

export class UsernameExistsError extends Error {
  constructor(msg: string) {
      super(msg);

      // Set the prototype explicitly.
      Object.setPrototypeOf(this, UsernameExistsError.prototype);
  }

  sayHello() {
      return "hello " + this.message;
  }
}

export default new class UserService {

  async createUser(username:string){
    const createUserRequest = { username: username };
    console.log("creating user!")
    try {
      var res = await axios.post('/api/users', createUserRequest)
      let user = new User(res.data.userId, res.data.username, res.data.primaryCollectionId);
      return user;
    } catch(e){
        console.log("Error creating user" + e);
        throw new Error("FailedToCreateUser");
    }
  }

  

  async getUser(userId:string){
    let path = `/api/users/${userId}`
    return axios.get(path);
  }

  async getUserByCollectionId(collectionId:string){
    let path = `/api/users?primaryCollectionId=${collectionId}`
    return axios.get(path);
  }

  async isUserAuthenticated(){
    let path = `/.auth/me`
    var res = await axios.get(path);
    return res.data.clientPrincipal != null;
  }

  async getRegisteredUser(){
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
