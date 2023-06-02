import axios from 'axios';

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
} 