using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;



public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseInventoryItem;
    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary;
    // Value = InventorySlot ; Key = InventorySlot_UI
    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlot_UI, InventorySlot> SlotDictionary => slotDictionary;

    public virtual void Start()
    {
        //just in case
    }

    public abstract void AssignSlot(InventorySystem invToDisplay);

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in SlotDictionary)
        {
            if (slot.Value == updatedSlot)
            {
                slot.Key.UpdateUISlot(updatedSlot);
            }
        }
    }


    public void SlotClicked(InventorySlot_UI clickedUISlot)
    {
        Debug.Log("Slot clicked.");
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

        // clicked slot has an item - mouse doesnt -> place clicked item on mouse object
        if (clickedUISlot.AssignedInventorySlot.itemData != null && mouseInventoryItem.AssignedInventorySlot.itemData == null)
        {
            Debug.Log("You are trying to pick up an item from the inventory.");
            
            // player is holding shift -> split the stack
            if(isShiftPressed && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot))
            {
                //mouseInventoryItem.AssignedInventorySlot= halfStackSlot; // war falsch, drinlassen falls später fehler tho
                mouseInventoryItem.UpdateMouseSlot(halfStackSlot);
                clickedUISlot.UpdateUISlot();
                
                return;
            }
            else
            {
                mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                clickedUISlot.ClearSlot();
                return;
            }


        }

        // clicked slot has no item - mouse has -> place mouse item in empty slot
        if (clickedUISlot.AssignedInventorySlot.itemData == null && mouseInventoryItem.AssignedInventorySlot.itemData != null)
        {
            Debug.Log("You are trying to place an item in the inventory.");

            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseInventoryItem.ClearSlot();
            return;
        }


        // both have an item
        if (clickedUISlot.AssignedInventorySlot.itemData != null && mouseInventoryItem.AssignedInventorySlot.itemData != null)
        {
            Debug.Log("You are holding an item in your hand and clicke on a occupied slot");
            bool isSameItem = clickedUISlot.AssignedInventorySlot.itemData == mouseInventoryItem.AssignedInventorySlot.itemData;

            // if it is the same item and there is room left in the stack
            if (isSameItem && clickedUISlot.AssignedInventorySlot.RoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
            {
                Debug.Log("There is room left in the slot you clicked.");

                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();
                return;
            }
            // if it is the same item and there is no room left in the stack
            else if (isSameItem && 
                !clickedUISlot.AssignedInventorySlot.RoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack))
            {
                if (leftInStack < 1) SwapSlots(clickedUISlot);      // swap will get to the same result
                else                                                // not at maxStack, so taking from mouse inventory
                {
                    int remainingOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;
                    clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                    clickedUISlot.UpdateUISlot();

                    var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.itemData, remainingOnMouse);
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.UpdateMouseSlot(newItem);
                    return;
                }
            }

            // if it is not the same item, swap them out
            else if(!isSameItem)
            {
                SwapSlots(clickedUISlot);
                return;
            }

        }


        // both are the same -> combine the stacks
        // does it together exceed max stack size? -> only take amountremaining from mouse
        // its less than max stack size -> take from mouse
        // different items -> swap items
    }

    private void SwapSlots(InventorySlot_UI clickedUISlot)
    {
        var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.itemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
        mouseInventoryItem.ClearSlot();

        mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        clickedUISlot.ClearSlot();
        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();
    }
}
