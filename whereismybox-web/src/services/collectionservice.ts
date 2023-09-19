
import axios from 'axios';
import EventService from '@/services/eventservice';
import type CollectionMetadata from '@/models/CollectionMetadata';
import type Contributor from '@/models/Contributor';

export default new class CollectionService {

  async createCollection(name:string){
    const requestBody = { Name: name};
    let path = `/api/collections`
    return axios
        .post(path, requestBody);
        //.then(() => EventService.BoxItemsChanged(boxId));
  }

  async getCollection(collectionId:string){
    let path = `/api/collections/${collectionId}`
    return axios
        .get(path);
        //.then(() => EventService.BoxItemsChanged(boxId));
  }

  async getOwnedCollections(userId: string){
    let path = `/api/users/${userId}/collections?filter=owner`
    try {
      const res = await axios.get<CollectionMetadata[]>(path);
      return res;
    }
    catch(e){
      throw new Error("Failed to get the collections a user owns");
    }
  }

  async getCollectionOwnerInfo(collectionId: string){
    let path = `/api/collections/${collectionId}/owner`
    try {
      const res = await axios.get<Contributor[]>(path);
      return res;
    }
    catch(e){
      throw new Error("Failed to get the owner of a collection");
    }
  }

  async getContributedCollections(userId: string){
    let path = `/api/users/${userId}/collections?filter=contributor`
    try {
      const res = await axios.get<CollectionMetadata[]>(path);
      return res;
    }
    catch(e){
      throw new Error("Failed to get collections user contributes to");
    }
  }
}
