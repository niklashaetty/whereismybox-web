export default class User{
    userId: string;
    username: string;
    primaryCollectionId: string;
    sharedCollectionIds: [];

    constructor(userId: string, username:string, primaryCollectionId:string, sharedCollectionIds: []){
        this.userId = userId;
        this.username = username;
        this.primaryCollectionId = primaryCollectionId;
        this.sharedCollectionIds = sharedCollectionIds;
    }
}