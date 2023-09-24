import type Item from '@/models/Item';
import axios from 'axios';
import EventService from '@/services/eventservice';
import type Box from '@/models/Box';

export default new class BoxService {

  async addItemToBox(collectionId:string, boxId:string, name:string, description:string){
    const requestBody = { Name: name,  Description: description};
    let path = `/api/collections/${collectionId}/boxes/${boxId}/items`
    return axios
        .post(path, requestBody)
        .then(() => EventService.BoxItemsChanged(boxId));
  }

  async getBoxCollection(collectionId:string){
    let path = `/api/collections/${collectionId}/boxes`
    return axios.get(path);
  }

  async getUnattachedItems(collectionId:string){
    let path = `/api/collections/${collectionId}/unattached-items`
    return axios.get(path);
  }

  async getBox(collectionId:string, boxId:string){
    let path = `/api/collections/${collectionId}/boxes/${boxId}`
    return axios.get(path);
  }

  // TODO: NOT WORKING
  async putItem(userId:string, boxId:string, item:Item){
    const requestBody = { ItemId: item.itemId,  Name: item.name, Description: item.description };
    let path = `/api/users/${userId}/boxes/${boxId}/items/${item.itemId}`
    axios.put(path, requestBody)
    .then(() => EventService.BoxItemsChanged(boxId))
  }

  async deleteItem(collectionId:string, boxId:string, itemId: string, hardDelete: boolean){
    let path = `/api/collections/${collectionId}/boxes/${boxId}/items/${itemId}?hardDelete=${hardDelete}`
    return axios.delete(path)
    .then(() => {
      EventService.BoxItemsChanged(boxId);
      if(hardDelete == false){
        EventService.UnattachedItemsChanged();
      }
    })
  }

  async deleteUnattachedItem(collectionId:string, itemId: string){
    let path = `/api/collections/${collectionId}/unattached-items/${itemId}`
    return axios.delete(path)
    .then(() => EventService.UnattachedItemsChanged());
  }

  async addBackUnattachedItem(collectionId:string, boxId:string, itemId: string){
    let path = `/api/collections/${collectionId}/unattached-items/${itemId}`
    const requestBody = { BoxId: boxId };
    return axios.post(path, requestBody)
    .then(() => EventService.BoxItemsChanged(boxId))
    .then(() => EventService.UnattachedItemsChanged());
  }

  async deleteBox(collectionId:string, boxId:string){
    let path = `/api/collections/${collectionId}/boxes/${boxId}`
    return axios.delete(path)
    .then(() => EventService.BoxDeleted(boxId));
  }

  async createBox(collectionId:string, boxNumber: number, boxName: string){
    const postBoxRequest = { Number: boxNumber, Name: boxName };
    let boxesPath = `/api/collections/${collectionId}/boxes`
    return axios.post(boxesPath, postBoxRequest)
    .then((response) => EventService.BoxAdded(response.data.boxId))
  }
}

