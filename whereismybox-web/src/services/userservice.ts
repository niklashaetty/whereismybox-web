import axios from 'axios';

export default new class UserService {

  async getUser(userId:string){
    let path = `/api/users/${userId}`
    return axios.get(path);
  }
}

