export default class User{
    userId: string;
    username: string;
    primaryCollectionId: string;

    constructor(userId: string, username:string, primaryCollectionId:string){
        this.userId = userId;
        this.username = username;
        this.primaryCollectionId = primaryCollectionId;
    }
}