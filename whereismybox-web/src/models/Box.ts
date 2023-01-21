import Item from '@/models/Item';

export default class Box{
    boxId: string;
    number: number;
    name: string;
    items: Item[];

    constructor(boxId: string, number:number, name:string, items:Item[]){
        this.boxId = boxId;
        this.number = number;
        this.name = name;
        this.items = items;
    }

    addItem(item:Item) {
        this.items.push(item);
    }

    deleteItem(item:Item) {
        this.items = this.items.filter(obj => obj.itemId !== item.itemId);
    }

    editItem(item:Item) {
        let index =this.items.findIndex((obj => obj.itemId == item.itemId));
        this.items[index] = new Item(item.itemId, item.name, item.description)
    }
}