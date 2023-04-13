import type Item from '@/models/Item';
import axios from 'axios';
import EventService from '@/services/eventservice';

export default new class BoxService {

  async addItemToBox(userId:string, boxId:string, name:string, description:string){
    const requestBody = { Name: name,  Description: description};
    let path = `/api/users/${userId}/boxes/${boxId}/items`
    return axios
        .post(path, requestBody)
        .then(() => EventService.BoxItemsChanged(boxId));
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
    .then(() => EventService.BoxItemsChanged(boxId))
  }

  async deleteItem(userId:string, boxId:string, itemId: string, hardDelete: boolean){
    let path = `/api/users/${userId}/boxes/${boxId}/items/${itemId}?hardDelete=${hardDelete}`
    axios.delete(path)
    .then(() => {
      console.log("Just deletedItem. Hard? " + hardDelete);
      EventService.BoxItemsChanged(boxId);
      if(hardDelete == false){
        EventService.UnattachedItemsChanged();
      }
    })
  }

  async deleteUnattachedItem(userId:string, itemId: string){
    let path = `/api/users/${userId}/items/${itemId}`
    axios.delete(path)
    .then(() => EventService.UnattachedItemsChanged());
  }

  async addBackUnattachedItem(userId:string, boxId:string, itemId: string){
    let path = `/api/users/${userId}/boxes/${boxId}/items/${itemId}`
    axios.post(path)
    .then(() =>{
      EventService.BoxItemsChanged(boxId)
    })
    .then(() => EventService.UnattachedItemsChanged());
  }

  async deleteBox(userId:string, boxId:string){
    let path = `/api/users/${userId}/boxes/${boxId}`
    axios.delete(path)
    .then(() => EventService.BoxDeleted(boxId));
  }

  async createBox(userId:string, boxNumber: number, boxName: string){
    const postBoxRequest = { Number: boxNumber, Name: boxName };
    let boxesPath = `/api/users/${userId}/boxes`
    await axios.post(boxesPath, postBoxRequest)
    .then((response) => EventService.BoxAdded(response.data.boxId))
  }
}

