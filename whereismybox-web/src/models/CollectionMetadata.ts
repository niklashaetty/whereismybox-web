export default class CollectionMetadata{
    collectionId: string;
    name: string;

    constructor(collectionId: string, name:string){
        this.collectionId = collectionId;
        this.name = name;
    }
}