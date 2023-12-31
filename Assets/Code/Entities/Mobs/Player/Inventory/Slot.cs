using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot<I> where I : Item
{
    private PlayerInventory _inventory;

    private I _containedItem;

    public Slot(PlayerInventory i) {
        this._inventory = i;
    }

    /**
        Place a new Item in the slot, returning the existing item if any
    */
    public I Place(I item) {
        I oldItem = _containedItem;

        _containedItem = item;

        Debug.Log(item.id);

        if (oldItem != null)
            return oldItem;
        else
            return null;
    }
}