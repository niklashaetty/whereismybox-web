export default class UnattachedItem{
    itemId: string = '';
    name: string = '';
    description: string = '';
    previousBoxId: string = '';
    
    constructor(itemId: string, name:string, description:string, previousBoxId:string){
        this.itemId = itemId;
        this.name = name;
        this.description = description;
        this.previousBoxId = previousBoxId;
    }
}