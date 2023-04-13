import EventBus from '@/services/eventbus';

export enum BoxEvents {
  ADDED = "BoxAdded",
  ITEM_CHANGED = "BoxItemsChanged",
  DELETED = "BoxDeleted",
  UNATTACHED_ITEMS_CHANGED = "UnattachedItemsChanged"
}

export default new class EventService {

BoxDeleted(boxId: string) {
    EventBus.emit(BoxEvents.DELETED, boxId);
}

BoxAdded(boxId: string) {
  EventBus.emit(BoxEvents.ADDED, boxId);

}

BoxItemsChanged(boxId: string) {
  EventBus.emit(BoxEvents.ITEM_CHANGED, boxId);
}

UnattachedItemsChanged() {
  EventBus.emit(BoxEvents.UNATTACHED_ITEMS_CHANGED);
}
}

