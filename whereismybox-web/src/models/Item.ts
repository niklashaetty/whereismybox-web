export default class Item{
    itemId: string = '';
    name: string = '';
    description: string = '';
    constructor(itemId: string, name:string, description:string){
        this.itemId = itemId;
        this.name = name;
        this.description = description;
    }
}