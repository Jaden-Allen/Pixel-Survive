using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Attatchment magazine;
    public Attatchment muzzle;
    public Attatchment upperRail;
    public Attatchment lowerRail;

    public Transform magazineParent;
    public Transform muzzleParent;
    public Transform upperRailParent;
    public Transform lowerRailParent;

    public Player player;

    public GunType gunType;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        GenerateWeapon(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !SlotManager.IsSlotEmpty(player.inventory.mainHotbar.slots[player.inventory.mainHotbar.selectedSlotIndex]))
        {
            Reload();
        }
    }
    void Reload()
    {
        ItemStack gunItemStack = player.inventory.mainHotbar.slots[player.inventory.mainHotbar.selectedSlotIndex].playerItemStack.itemStack;

        if (gunItemStack.gunData.bulletCount < gunItemStack.gunData.maxBulletCount)
        {
            player.shoot.PlayAudio(GunAudioType.reload, gunItemStack);
        }

        foreach (Slot slot in player.inventory.inventory.slots)
        {
            if (!SlotManager.IsSlotEmpty(slot))
            {
                if (slot.playerItemStack.itemStack.data.item.itemType == ItemType.ammo)
                {
                    if (slot.playerItemStack.itemStack.data.item.ammoData.type == gunItemStack.data.item.gunData.gunType)
                    {
                        if ((slot.playerItemStack.itemStack.data.amount - (gunItemStack.gunData.maxBulletCount - gunItemStack.gunData.bulletCount)) > 0)
                        {
                            slot.playerItemStack.itemStack.data.amount -= (gunItemStack.gunData.maxBulletCount - gunItemStack.gunData.bulletCount);
                            gunItemStack.gunData.bulletCount = gunItemStack.gunData.maxBulletCount;

                            player.manager.slotHandler.slotsToUpdate.Add(slot);
                            return;
                        }
                        else
                        {
                            gunItemStack.gunData.bulletCount += slot.playerItemStack.itemStack.data.amount;
                            slot.playerItemStack.itemStack.data.amount = 0;
                            player.manager.slotHandler.slotsToUpdate.Add(slot);
                            return;
                        }
                    }
                }
            }
        }
        int p = 0;
        foreach (Slot slot in player.inventory.hotbar.slots)
        {
            if (!SlotManager.IsSlotEmpty(slot))
            {
                if (slot.playerItemStack.itemStack.data.item.itemType == ItemType.ammo)
                {
                    if (slot.playerItemStack.itemStack.data.item.ammoData.type == gunItemStack.data.item.gunData.gunType)
                    {
                        if ((slot.playerItemStack.itemStack.data.amount - (gunItemStack.gunData.maxBulletCount - gunItemStack.gunData.bulletCount)) > 0)
                        {
                            slot.playerItemStack.itemStack.data.amount -= (gunItemStack.gunData.maxBulletCount - gunItemStack.gunData.bulletCount);
                            gunItemStack.gunData.bulletCount = gunItemStack.gunData.maxBulletCount;

                            player.manager.slotHandler.slotsToUpdate.Add(slot);
                            player.manager.slotHandler.slotsToUpdate.Add(player.inventory.mainHotbar.slots[p]);
                            return;
                        }
                        else
                        {
                            gunItemStack.gunData.bulletCount += slot.playerItemStack.itemStack.data.amount;
                            slot.playerItemStack.itemStack.data.amount = 0;
                            player.manager.slotHandler.slotsToUpdate.Add(slot);
                            player.manager.slotHandler.slotsToUpdate.Add(player.inventory.mainHotbar.slots[p]);
                            return;
                        }
                    }
                }
            }
            p++;
        }
    }
    public void GenerateWeapon(bool isIcon)
    {
        if (isIcon)
        {
            if (magazine != null)
            {
                GameObject go = Instantiate(magazine, magazineParent).gameObject;
                foreach (Transform t in go.transform)
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Icon");
                }
                go.layer = LayerMask.NameToLayer("Icon");
                go.transform.localPosition = Vector3.zero;
            }
            if (muzzle != null)
            {
                GameObject go = Instantiate(muzzle, muzzleParent).gameObject;
                foreach (Transform t in go.transform)
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Icon");
                }
                go.layer = LayerMask.NameToLayer("Icon");
                go.transform.localPosition = Vector3.zero;
            }
            if (upperRail != null)
            {
                GameObject go = Instantiate(upperRail, upperRailParent).gameObject;
                foreach (Transform t in go.transform)
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Icon");
                }
                go.layer = LayerMask.NameToLayer("Icon");
                go.transform.localPosition = Vector3.zero;
            }
            if (lowerRail != null)
            {
                GameObject go = Instantiate(lowerRail, lowerRailParent).gameObject;

                foreach (Transform t in go.transform)
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Icon");
                }
                go.layer = LayerMask.NameToLayer("Icon");
                go.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            if (magazine != null)
            {
                GameObject go = Instantiate(magazine, magazineParent).gameObject;
                go.transform.localPosition = Vector3.zero;
            }
            if (muzzle != null)
            {
                GameObject go = Instantiate(muzzle, muzzleParent).gameObject;
                go.transform.localPosition = Vector3.zero;
            }
            if (upperRail != null)
            {
                GameObject go = Instantiate(upperRail, upperRailParent).gameObject;
                go.transform.localPosition = Vector3.zero;
            }
            if (lowerRail != null)
            {
                GameObject go = Instantiate(lowerRail, lowerRailParent).gameObject;
                go.transform.localPosition = Vector3.zero;
            }
        }
    }
}
