using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public Player player;
    public Slot[] slots;
    public RectTransform selectedSlotImage;
    public PlayerHands hands;

    public int selectedSlotIndex;

    public Material itemMat;


    private void Start()
    {
        selectedSlotIndex = 1;
        SelectSlot(selectedSlotIndex);
    }
    private void Update()
    {
        if (Input.mouseScrollDelta.y < 0)
        {
            selectedSlotIndex++;
            if (selectedSlotIndex > slots.Length - 1)
            {
                selectedSlotIndex = 0;
            }
            SelectSlot(selectedSlotIndex);
        }
        if (Input.mouseScrollDelta.y > 0)
        {
            selectedSlotIndex--;
            if (selectedSlotIndex < 0)
            {
                selectedSlotIndex = slots.Length - 1;
            }
            SelectSlot(selectedSlotIndex);
        }
        if (Input.GetKeyDown(KeyCode.F) && !player.manager.uiManager.uiOpen)
        {
            player.inventory.offhand.slot.TransferItemStack(slots[selectedSlotIndex]);
            player.manager.slotHandler.slotsToUpdate.Add(player.inventory.hotbar.slots[selectedSlotIndex]);
        }
    }
    public void SelectSlot(int slotIndex)
    {
        //Debug.Log(slotIndex.ToString());
        selectedSlotImage.position = slots[slotIndex].transform.position;
        if (hands.rightHandItem != null)
        {
            GameObject.Destroy(hands.rightHandItem.gameObject);
        }
        if (slots[slotIndex].playerItemStack.itemStack.data.item != null)
        {
            hands.rightHandItem = VoxelData.CreateItemObjectFromData(slots[slotIndex].playerItemStack.itemStack, itemMat, hands.rightHand, true);
        }
    }
    public void UpdateRightHandItem()
    {
        if (hands.rightHandItem != null)
        {
            GameObject.Destroy(hands.rightHandItem.gameObject);
        }
        if (slots[selectedSlotIndex].playerItemStack.itemStack.data.item != null)
        {
            hands.rightHandItem = VoxelData.CreateItemObjectFromData(slots[selectedSlotIndex].playerItemStack.itemStack, itemMat, hands.rightHand, true, true);

        }
    }
    public void UpdateLeftHandItem()
    {
        if (hands.leftHandItem != null)
        {
            GameObject.Destroy(hands.leftHandItem.gameObject);
        }
        if (player.inventory.offhand.slot.playerItemStack.itemStack.data.item != null)
        {
            hands.leftHandItem = VoxelData.CreateItemObjectFromData(player.inventory.offhand.slot.playerItemStack.itemStack, itemMat, hands.leftHand, true, false);

        }
    }
    public IEnumerator UpdateSlots()
    {
        foreach (Slot slot in slots)
        {
            player.manager.slotHandler.slotsToUpdate.Add(slot);
            yield return new WaitForEndOfFrame();
        }
    }
}
