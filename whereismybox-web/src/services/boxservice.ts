import type Item from '@/models/Item';
import axios from 'axios';

export default new class BoxService {

  async addItemToBox(userId:string, boxId:string, name:string, description:string){
    const requestBody = { Name: name,  Description: description};
    let path = `/api/users/${userId}/boxes/${boxId}/items`
    return await axios
        .post(path, requestBody);
  }

  async getBox(userId:string, boxId:string){
    let path = `/api/users/${userId}/boxes/${boxId}`
    return await axios.get(path);
  }

  async putItem(userId:string, boxId:string, item:Item){
    console.log("PutItem " + item.itemId)
    const requestBody = { ItemId: item.itemId,  Name: item.name, Description: item.description };
    let path = `/api/users/${userId}/boxes/${boxId}/items/${item.itemId}`
    await axios.put(path, requestBody);
  }

  async deleteItem(userId:string, boxId:string, itemId: string){
    let path = `/api/users/${userId}/boxes/${boxId}/items/${itemId}`
    await axios.delete(path);
  }
}

const response = {
  Ok: 0,
  BadRequest: 1,
  InternalServerError: 2,
};
