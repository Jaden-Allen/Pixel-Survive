using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWorkenchUI : MonoBehaviour
{
    public GameObject weaponWorkbenchUI;
    public GameObject weaponModificationPage;
    public GameObject weaponRepairPage;

    public Slot modification_weaponSlot;
    public Slot modification_topRailSlot;
    public Slot modification_magazineSlot;
    public Slot modification_bottomRailSlot;
    public Slot modification_muzzleSlot;

    public RawImage modification_weaponSlotIcon;
    public RawImage modification_topRailSlotIcon;
    public RawImage modification_magazineSlotIcon;
    public RawImage modification_bottomRailSlotIcon;
    public RawImage modification_muzzleSlotIcon;


    public Slot repair_mainWeaponSlot;
    public Slot repair_repairItemSlot;

    public Player player;

    bool modificationPage;
    public Color slotIconColor;

    List<Slot> slots = new List<Slot>();
    public bool updateRecipe;
    private void Start()
    {
        //slots.Add(modification_weaponSlot);
        slots.Add(modification_topRailSlot);
        slots.Add(modification_magazineSlot);
        slots.Add(modification_bottomRailSlot);
        slots.Add(modification_muzzleSlot);
    }
    private void Update()
    {
        if (updateRecipe)
        {
            StartCoroutine(CheckForRecipe());
        }
        ChangeIconColor();
    }
    public void UpdateSlots()
    {
        foreach (Slot slot in slots)
        {
            player.manager.slotHandler.slotsToUpdate.Add(slot);
        }
    }
    public void ChangePage()
    {
        modificationPage = !modificationPage;

        weaponModificationPage.SetActive(modificationPage);
        weaponRepairPage.SetActive(!modificationPage);
    }
    public void ResetPartsSlots()
    {
        modification_topRailSlot.ResetSlot();
        modification_magazineSlot.ResetSlot();
        modification_bottomRailSlot.ResetSlot();
        modification_muzzleSlot.ResetSlot();
    }
    public bool IsAnyPartSlotFull()
    {
        if (!SlotManager.IsSlotEmpty(modification_bottomRailSlot) || !SlotManager.IsSlotEmpty(modification_muzzleSlot) || !SlotManager.IsSlotEmpty(modification_topRailSlot) || !SlotManager.IsSlotEmpty(modification_magazineSlot)) { return true; }
        else { return false; }
    }
    public IEnumerator CheckForRecipe()
    {
        if (!SlotManager.IsSlotEmpty(modification_bottomRailSlot))
        {
            modification_weaponSlot.playerItemStack.itemStack.gunData.bottomRailAttatchment = modification_bottomRailSlot.playerItemStack.itemStack.data.item.gunPartData.attatchment;
        }
        else
        {
            modification_weaponSlot.playerItemStack.itemStack.gunData.bottomRailAttatchment = null;
        }
        if (!SlotManager.IsSlotEmpty(modification_magazineSlot))
        {
            modification_weaponSlot.playerItemStack.itemStack.gunData.magazineAttatchment = modification_magazineSlot.playerItemStack.itemStack.data.item.gunPartData.attatchment;
        }
        else
        {
            modification_weaponSlot.playerItemStack.itemStack.gunData.magazineAttatchment = null;
        }
        if (!SlotManager.IsSlotEmpty(modification_muzzleSlot))
        {
            modification_weaponSlot.playerItemStack.itemStack.gunData.muzzleAttachment = modification_muzzleSlot.playerItemStack.itemStack.data.item.gunPartData.attatchment;
        }
        else
        {
            modification_weaponSlot.playerItemStack.itemStack.gunData.muzzleAttachment = null;
        }
        if (!SlotManager.IsSlotEmpty(modification_topRailSlot))
        {
            modification_weaponSlot.playerItemStack.itemStack.gunData.topRailAttatchment = modification_topRailSlot.playerItemStack.itemStack.data.item.gunPartData.attatchment;
        }
        else
        {
            modification_weaponSlot.playerItemStack.itemStack.gunData.topRailAttatchment = null;
        }

        yield return new WaitForEndOfFrame();
        player.manager.slotHandler.slotsToUpdate.Add(modification_weaponSlot);

        updateRecipe = false;
    }
    void ChangeIconColor()
    {
        if (!SlotManager.IsSlotEmpty(modification_weaponSlot))
        {
            modification_weaponSlotIcon.color = Color.clear;
        }
        else
        {
            modification_weaponSlotIcon.color = slotIconColor;
        }
        if (!SlotManager.IsSlotEmpty(modification_bottomRailSlot))
        {
            modification_bottomRailSlotIcon.color = Color.clear;
        }
        else
        {
            modification_bottomRailSlotIcon.color = slotIconColor;
        }
        if (!SlotManager.IsSlotEmpty(modification_magazineSlot))
        {
            modification_magazineSlotIcon.color = Color.clear;
        }
        else
        {
            modification_magazineSlotIcon.color = slotIconColor;
        }
        if (!SlotManager.IsSlotEmpty(modification_muzzleSlot))
        {
            modification_muzzleSlotIcon.color = Color.clear;
        }
        else
        {
            modification_muzzleSlotIcon.color = slotIconColor;
        }
        if (!SlotManager.IsSlotEmpty(modification_topRailSlot))
        {
            modification_topRailSlotIcon.color = Color.clear;
        }
        else
        {
            modification_topRailSlotIcon.color = slotIconColor;
        }
    }
}
