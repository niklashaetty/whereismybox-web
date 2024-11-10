export default class UnattachedItem{
    itemId: string = '';
    name: string = '';
    description: string = '';
    previousBoxId: string = '';
    previousBoxNumber: number;
    
    constructor(itemId: string, name:string, description:string, previousBoxId:string, previousBoxNumber:number){
        this.itemId = itemId;
        this.name = name;

        this.description = description;
        this.previousBoxId = previousBoxId;
        this.previousBoxNumber = previousBoxNumber;
    }
}