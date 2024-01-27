using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public Player player;

    public armour armour;
    public offhand offhand;
    public crafting crafting;
    public craftingResult craftingResult;
    public Slots inventory;
    public Slots hotbar;

    public Hotbar mainHotbar;

    public Slot cursorSlot;

    public Camera fpCam;
    public Camera iconCam;
    public RenderTexture renderTexture;

    public EventSystem eventSystem;
    public GraphicRaycaster graphicRaycaster;

    public List<Slot> slots = new List<Slot>();
    private void Update()
    {
        cursorSlot.transform.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.F))
        {
            SwapToOffhand();
        }
    }
    private void Start()
    {
        foreach (Slot slot in armour.slots) { slots.Add(slot); }
        foreach (Slot slot in crafting.slots) { slots.Add(slot); }
        foreach (Slot slot in inventory.slots) { slots.Add(slot); }
        foreach (Slot slot in hotbar.slots) { slots.Add(slot); }
        foreach (Slot slot in mainHotbar.slots) { slots.Add(slot); }
        slots.Add(offhand.slot);
        slots.Add(craftingResult.slot);


        UpdateAllSlots();
    }
    void UpdateAllSlots()
    {
        foreach (Slot slot in slots)
        {
            //Debug.Log(slot);
            player.manager.slotHandler.slotsToUpdate.Add(slot);

        }
    }
    public void SwapToOffhand()
    {
        if (player.manager.uiManager.uiOpen)
        {
            player.inventory.offhand.slot.SwapToOffhand(eventSystem, graphicRaycaster);
        }
    }
    public void AddItemToInventory(ItemEntity itemEntity)
    {
        int amountToAdd = itemEntity.itemStack.data.amount;
        for (int i = 0; i < hotbar.slots.Count; i++)
        {
            if (SlotManager.IsSlotEmpty(hotbar.slots[i]))
            {
                if (amountToAdd <= itemEntity.itemStack.data.item.itemData.maxStackSize)
                {
                    hotbar.slots[i].playerItemStack.itemStack = itemEntity.itemStack.Clone();
                    hotbar.slots[i].playerItemStack.itemStack.data.amount = amountToAdd; Debug.Log(hotbar.slots[i].playerItemStack.itemStack.data.amount + "Slot empty, amountToAdd <= maxStackSize");
                    player.manager.slotHandler.slotsToUpdate.Add(hotbar.slots[i]);
                    player.manager.slotHandler.slotsToUpdate.Add(mainHotbar.slots[i]);
                    itemEntity.pickedUp = true;
                    return;
                }
                else
                {
                    amountToAdd -= itemEntity.itemStack.data.item.itemData.maxStackSize;
                    hotbar.slots[i].playerItemStack.itemStack = itemEntity.itemStack.Clone(); 
                    hotbar.slots[i].playerItemStack.itemStack.data.amount = itemEntity.itemStack.data.item.itemData.maxStackSize; Debug.Log(hotbar.slots[i].playerItemStack.itemStack.data.amount + "Slot empty, amountToAdd > maxStackSize");
                    player.manager.slotHandler.slotsToUpdate.Add(hotbar.slots[i]);
                    player.manager.slotHandler.slotsToUpdate.Add(mainHotbar.slots[i]);
                }
                
            }
            else if (ItemStackManager.IsItemTypeSame(hotbar.slots[i].playerItemStack.itemStack, itemEntity.itemStack))
            {
                if ((hotbar.slots[i].playerItemStack.itemStack.data.amount + amountToAdd) <= itemEntity.itemStack.data.item.itemData.maxStackSize)
                {
                    hotbar.slots[i].playerItemStack.itemStack.data.amount += amountToAdd;
                    player.manager.slotHandler.slotsToUpdate.Add(hotbar.slots[i]);
                    player.manager.slotHandler.slotsToUpdate.Add(mainHotbar.slots[i]);
                    return;
                }
                else
                {
                    amountToAdd = amountToAdd - (itemEntity.itemStack.data.item.itemData.maxStackSize - hotbar.slots[i].playerItemStack.itemStack.data.amount);
                    hotbar.slots[i].playerItemStack.itemStack.data.amount = itemEntity.itemStack.data.item.itemData.maxStackSize;
                    player.manager.slotHandler.slotsToUpdate.Add(hotbar.slots[i]);
                    player.manager.slotHandler.slotsToUpdate.Add(mainHotbar.slots[i]);
                }
            }
        }
        for (int i = 0; i < inventory.slots.Count; i++)
        {
            if (SlotManager.IsSlotEmpty(inventory.slots[i]))
            {
                if (amountToAdd <= itemEntity.itemStack.data.item.itemData.maxStackSize)
                {
                    inventory.slots[i].playerItemStack.itemStack = itemEntity.itemStack.Clone();
                    inventory.slots[i].playerItemStack.itemStack.data.amount = amountToAdd; Debug.Log(inventory.slots[i].playerItemStack.itemStack.data.amount + "Slot empty, amountToAdd <= maxStackSize");
                    player.manager.slotHandler.slotsToUpdate.Add(inventory.slots[i]);
                    itemEntity.pickedUp = true;
                    return;
                }
                else
                {
                    amountToAdd -= itemEntity.itemStack.data.item.itemData.maxStackSize;
                    inventory.slots[i].playerItemStack.itemStack = itemEntity.itemStack.Clone();
                    inventory.slots[i].playerItemStack.itemStack.data.amount = itemEntity.itemStack.data.item.itemData.maxStackSize; Debug.Log(inventory.slots[i].playerItemStack.itemStack.data.amount + "Slot empty, amountToAdd > maxStackSize");
                    player.manager.slotHandler.slotsToUpdate.Add(inventory.slots[i]);
                }

            }
            else if (ItemStackManager.IsItemTypeSame(inventory.slots[i].playerItemStack.itemStack, itemEntity.itemStack))
            {
                if ((inventory.slots[i].playerItemStack.itemStack.data.amount + amountToAdd) <= itemEntity.itemStack.data.item.itemData.maxStackSize)
                {
                    inventory.slots[i].playerItemStack.itemStack.data.amount += amountToAdd;
                    player.manager.slotHandler.slotsToUpdate.Add(inventory.slots[i]);
                    return;
                }
                else
                {
                    amountToAdd = amountToAdd - (itemEntity.itemStack.data.item.itemData.maxStackSize - inventory.slots[i].playerItemStack.itemStack.data.amount);
                    inventory.slots[i].playerItemStack.itemStack.data.amount = itemEntity.itemStack.data.item.itemData.maxStackSize;
                    player.manager.slotHandler.slotsToUpdate.Add(inventory.slots[i]);
                }
            }
        }
    }
}
[System.Serializable]
public class hotbar
{
    public Slot[] slots;
}
[System.Serializable]
public class inventory
{
    public Slot[] slots;
}
[System.Serializable]
public class armour
{
    public Slot[] slots;
}
[System.Serializable]
public class offhand
{
    public Slot slot;
}
[System.Serializable]
public class crafting
{
    public Slot[] slots;
}
[System.Serializable]
public class craftingResult
{
    public Slot slot;
}