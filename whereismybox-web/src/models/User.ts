export default class User{
    userId: string;
    username: string;
    isRegistered: boolean;

    constructor(userId: string, username:string, isRegistered: boolean){
        this.userId = userId;
        this.username = username;
        this.isRegistered = isRegistered;
    }
}