import type Item from '@/models/Item';
import axios from 'axios';

export default new class BoxService {

  async addItemToBox(userId:string, boxId:string, name:string, description:string){
    const requestBody = { Name: name,  Description: description};
    let path = `/api/users/${userId}/boxes/${boxId}/items`
    return axios
        .post(path, requestBody);
  }

  async getBox(userId:string, boxId:string){
    let path = `/api/users/${userId}/boxes/${boxId}`
    return axios.get(path);
  }

  async putItem(userId:string, boxId:string, item:Item){
    console.log("PutItem " + item.itemId)
    const requestBody = { ItemId: item.itemId,  Name: item.name, Description: item.description };
    let path = `/api/users/${userId}/boxes/${boxId}/items/${item.itemId}`
    axios.put(path, requestBody)
  }

  async deleteItem(userId:string, boxId:string, itemId: string, hardDelete: boolean){
    let path = `/api/users/${userId}/boxes/${boxId}/items/${itemId}?hardDelete=${hardDelete}`
    axios.delete(path);
  }

  async deleteUnattachedItem(userId:string, itemId: string){
    let path = `/api/users/${userId}/items/${itemId}`
    axios.delete(path);
  }

  async addBackUnattachedItem(userId:string, boxId:string, itemId: string){
    let path = `/api/users/${userId}/boxes/${boxId}/items/${itemId}`
    axios.post(path);
  }

  async deleteBox(userId:string, boxId:string){
    let path = `/api/users/${userId}/boxes/${boxId}`
    axios.delete(path);
  }
}

