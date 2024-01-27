using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public Player player;

    public PlayerItemStack playerItemStack;

    public RawImage iconImage;
    public RawImage backgroundImage;
    public Slider durabilitySlider;
    public RawImage durabilityFill;
    public TMP_Text itemAmountText;

    public SlotType slotType;
    public GunPartType gunPartType;
    public bool isDisabled;

    private void Awake()
    {
        player = GameObject.FindAnyObjectByType<Player>();
    }
    private void Update()
    {
        
    }
    public void UpdateSlot(RenderTexture renderTexture, Camera camPos)
    {
        if (slotType == SlotType.gunPart)
        {
            if (isDisabled)
            {
                backgroundImage.color = new Color(0.35f, 0.35f, 0.35f, 1f);
                return;
            }
            else
            {
                backgroundImage.color = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1f);
            }
        }
        if (playerItemStack.itemStack.data.item == null || playerItemStack.itemStack.data.amount == 0)
        {
            ResetSlot();
            return;
        }
        if (playerItemStack.itemStack.data.item.itemType == ItemType.ammo)
        {
            UpdateSlot_Normal(renderTexture, camPos);
        }
        if (playerItemStack.itemStack.data.item.itemType == ItemType.normal)
        {
            UpdateSlot_Normal(renderTexture, camPos);
        }
        if (playerItemStack.itemStack.data.item.itemType == ItemType.tool)
        {
            UpdateSlot_Normal(renderTexture, camPos);
            UpdateSlot_Tool(renderTexture, camPos);
        }
        if (playerItemStack.itemStack.data.item.itemType == ItemType.food)
        {
            UpdateSlot_Normal(renderTexture, camPos);
            UpdateSlot_Food(renderTexture, camPos);
        }
        if (playerItemStack.itemStack.data.item.itemType == ItemType.gun)
        {
            UpdateSlot_Normal(renderTexture, camPos);
            UpdateSlot_Gun(renderTexture, camPos);
        }
        if (playerItemStack.itemStack.data.item.itemType == ItemType.gunPart)
        {
            UpdateSlot_Normal(renderTexture, camPos);
        }
    }
    void UpdateSlot_Normal(RenderTexture renderTexture, Camera camPos)
    {
        if (playerItemStack.itemStack.data.amount == 1)
        {
            itemAmountText.text = "";
        }
        else
        {
            itemAmountText.text = playerItemStack.itemStack.data.amount.ToString();
        }

        if (playerItemStack.itemStack.data.item.itemData.customGeometry != null)
        {
            StartCoroutine(UpdateCustomGeoSlot(renderTexture, camPos));
            return;
        }
        else
        {
            iconImage.texture = playerItemStack.itemStack.data.item.itemData.texture;
            iconImage.color = Color.white;
        }
        durabilitySlider.gameObject.SetActive(false);
    }
    void UpdateSlot_Tool(RenderTexture renderTexture, Camera camPos)
    {
        if (playerItemStack.itemStack.toolData.durability == 0)
        {
            ResetSlot();
            durabilitySlider.gameObject.SetActive(false);
        }
        else
        {
            durabilitySlider.gameObject.SetActive(true);
            durabilitySlider.maxValue = playerItemStack.itemStack.data.item.toolData.maxDurability;
            durabilitySlider.value = playerItemStack.itemStack.toolData.durability;
            durabilityFill.color = Color.Lerp(SlotManager.lowDurability, SlotManager.fullDurability, (float)playerItemStack.itemStack.toolData.durability / (float)playerItemStack.itemStack.data.item.toolData.maxDurability);
            
        }

    }
    void UpdateSlot_Gun(RenderTexture renderTexture, Camera camPos)
    {
        if (playerItemStack.itemStack.gunData.durability == 0)
        {
            ResetSlot();
            durabilitySlider.gameObject.SetActive(false);
        }
        else
        {
            durabilitySlider.maxValue = playerItemStack.itemStack.gunData.maxDurability;
            durabilitySlider.value = playerItemStack.itemStack.gunData.durability;
            durabilityFill.color = Color.Lerp(SlotManager.lowDurability, SlotManager.fullDurability, (float)playerItemStack.itemStack.gunData.durability / (float)playerItemStack.itemStack.gunData.maxDurability);
            durabilitySlider.gameObject.SetActive(true);
        }

    }
    void UpdateSlot_Food(RenderTexture renderTexture, Camera camPos)
    {

    }
    public void ResetSlot()
    {
        iconImage.color = new Color(1, 1, 1, 0);
        iconImage.texture = null;
        durabilitySlider.value = 0;
        durabilitySlider.maxValue = 0;
        durabilitySlider.gameObject.SetActive(false);
        itemAmountText.text = "";
        playerItemStack.itemStack.Clear();
    }
    public IEnumerator UpdateCustomGeoSlot(RenderTexture renderTexture, Camera iconGenCam)
    {
        GameObject item = CreateSlotIconObject(this, renderTexture, iconGenCam);
        yield return new WaitForEndOfFrame();
        iconImage.texture = IconGen(this, item, renderTexture, iconGenCam);
    }
    public void TransferItemStack(Slot fromSlot)
    {
        ItemStack itemStack1 = playerItemStack.itemStack.Clone();
        ItemStack itemStack2 = fromSlot.playerItemStack.itemStack.Clone();

        playerItemStack.itemStack = itemStack2;
        fromSlot.playerItemStack.itemStack = itemStack1;

        player.manager.slotHandler.slotsToUpdate.Add(this);
        player.manager.slotHandler.slotsToUpdate.Add(fromSlot);

        player.inventory.mainHotbar.UpdateRightHandItem();
        player.inventory.mainHotbar.UpdateLeftHandItem();
        StartCoroutine(player.inventory.mainHotbar.UpdateSlots());
    }
    public void AddToItemStack(Slot fromSlot, Slot toSlot)
    {
        ItemStack itemStack1 = fromSlot.playerItemStack.itemStack.Clone();
        ItemStack itemStack2 = toSlot.playerItemStack.itemStack.Clone();

        int leftover = itemStack1.data.amount;

        if (itemStack2.data.amount == itemStack2.data.item.itemData.maxStackSize)
        {
            return;
        }

        if (itemStack1.data.amount + itemStack2.data.amount <= itemStack2.data.item.itemData.maxStackSize) 
        {
            
            toSlot.playerItemStack.itemStack.data.amount = itemStack1.data.amount + itemStack2.data.amount;
            fromSlot.playerItemStack.itemStack.data.amount = 0;

            player.manager.slotHandler.slotsToUpdate.Add(toSlot);
            player.manager.slotHandler.slotsToUpdate.Add(fromSlot);

            player.inventory.mainHotbar.UpdateRightHandItem();
            player.inventory.mainHotbar.UpdateLeftHandItem();
            StartCoroutine(player.inventory.mainHotbar.UpdateSlots());
            return;
        }
        else
        {
            leftover = (itemStack2.data.amount + itemStack1.data.amount) - itemStack2.data.item.itemData.maxStackSize;

            itemStack1.data.amount = leftover;
            itemStack2.data.amount = itemStack2.data.item.itemData.maxStackSize;

            fromSlot.playerItemStack.itemStack = itemStack1;
            toSlot.playerItemStack.itemStack = itemStack2;

            player.manager.slotHandler.slotsToUpdate.Add(toSlot);
            player.manager.slotHandler.slotsToUpdate.Add(fromSlot);

            player.inventory.mainHotbar.UpdateRightHandItem();
            player.inventory.mainHotbar.UpdateLeftHandItem();
            StartCoroutine(player.inventory.mainHotbar.UpdateSlots());
            return;
        }
    }
    public void TransferSingleItem(Slot fromSlot, Slot toSlot)
    {
        ItemStack itemStack1 = fromSlot.playerItemStack.itemStack.Clone();
        ItemStack itemStack2 = toSlot.playerItemStack.itemStack.Clone();
        if (SlotManager.IsSlotEmpty(toSlot))
        {
            itemStack2 = itemStack1.Clone();
            itemStack2.data.amount = 1;
            itemStack1.data.amount--;

            Debug.Log("ItemStack1 Amount : " + itemStack1.data.amount + ", ItemStack2 Amount : " + itemStack2.data.amount);

            fromSlot.playerItemStack.itemStack = itemStack1;
            toSlot.playerItemStack.itemStack = itemStack2;

            player.manager.slotHandler.slotsToUpdate.Add(toSlot);
            player.manager.slotHandler.slotsToUpdate.Add(fromSlot);

            player.inventory.mainHotbar.UpdateRightHandItem();
            player.inventory.mainHotbar.UpdateLeftHandItem();
            StartCoroutine(player.inventory.mainHotbar.UpdateSlots());
            return;
        }
        if (itemStack1.data.amount - 1 >= 0 && itemStack2.data.amount + 1 <= itemStack2.data.item.itemData.maxStackSize)
        {
            itemStack1.data.amount--;
            itemStack2.data.amount++;

            fromSlot.playerItemStack.itemStack = itemStack1;
            toSlot.playerItemStack.itemStack = itemStack2;

            player.manager.slotHandler.slotsToUpdate.Add(toSlot);
            player.manager.slotHandler.slotsToUpdate.Add(fromSlot);

            player.inventory.mainHotbar.UpdateRightHandItem();
            player.inventory.mainHotbar.UpdateLeftHandItem();
            StartCoroutine(player.inventory.mainHotbar.UpdateSlots());
        }
        
    }
    public void TakeHalf(Slot fromSlot, Slot toSlot)
    {
        ItemStack itemStack1 = fromSlot.playerItemStack.itemStack.Clone();

        if (itemStack1.data.amount > 1)
        {
            int amountToTake = Mathf.RoundToInt(itemStack1.data.amount / 2);
            int leftoverAmount = itemStack1.data.amount - amountToTake;

            fromSlot.playerItemStack.itemStack.data.amount = leftoverAmount;

            toSlot.playerItemStack.itemStack.data.item = itemStack1.data.item;
            toSlot.playerItemStack.itemStack.data.amount = amountToTake;

            player.manager.slotHandler.slotsToUpdate.Add(toSlot);
            player.manager.slotHandler.slotsToUpdate.Add(fromSlot);

            player.inventory.mainHotbar.UpdateRightHandItem();
            player.inventory.mainHotbar.UpdateLeftHandItem();
            StartCoroutine(player.inventory.mainHotbar.UpdateSlots());
        }
        else
        {
            TransferItemStack(toSlot);

            player.inventory.mainHotbar.UpdateRightHandItem();
            player.inventory.mainHotbar.UpdateLeftHandItem();
            StartCoroutine(player.inventory.mainHotbar.UpdateSlots());
        }
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LeftClick(eventData);
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClick(eventData);
        }
    }
    void RightClick(PointerEventData eventData)
    {
        if (slotType == SlotType.any)
        {
            if (SlotManager.IsSlotEmpty(player.inventory.cursorSlot) && IsSlotEmpty())
            {
                return;
            }
            if (SlotManager.IsSlotEmpty(player.inventory.cursorSlot) && !IsSlotEmpty())
            {
                TakeHalf(this, player.inventory.cursorSlot);
                Debug.Log("taking half");
                return;
            }
            if (!SlotManager.IsSlotEmpty(player.inventory.cursorSlot) && IsSlotEmpty())
            {
                TransferSingleItem(player.inventory.cursorSlot, this);
                Debug.Log("placing single item");
                return;
            }
            if (!SlotManager.IsSlotEmpty(player.inventory.cursorSlot) && !IsSlotEmpty())
            {
                if (ItemStackManager.IsItemTypeSame(player.inventory.cursorSlot.playerItemStack.itemStack, playerItemStack.itemStack))
                {
                    TransferSingleItem(player.inventory.cursorSlot, this);
                    Debug.Log("placing single item");
                    return;
                }
                else
                {
                    TransferItemStack(player.inventory.cursorSlot);
                    Debug.Log("swapping item stacks");
                    return;
                }
                
            }
        }
    }
    void LeftClick(PointerEventData eventData)
    {
        if (isDisabled)
        {
            return;
        }
        if (slotType == SlotType.any)
        {
            if (!SlotManager.IsSlotEmpty(player.inventory.cursorSlot) && !SlotManager.IsSlotEmpty(this))
            {
                if (player.inventory.cursorSlot.playerItemStack.itemStack.data.item.itemType == ItemType.ammo)
                {
                    if (playerItemStack.itemStack.data.item.itemType == ItemType.gun)
                    {
                        if (player.inventory.cursorSlot.playerItemStack.itemStack.data.item.ammoData.type == playerItemStack.itemStack.data.item.gunData.gunType)
                        {
                            int bulletsLeftover = player.inventory.cursorSlot.playerItemStack.itemStack.data.amount - (playerItemStack.itemStack.gunData.maxBulletCount - playerItemStack.itemStack.gunData.bulletCount);
                            if (bulletsLeftover <= 0)
                            {
                                playerItemStack.itemStack.gunData.bulletCount += player.inventory.cursorSlot.playerItemStack.itemStack.data.amount;
                                player.inventory.cursorSlot.playerItemStack.itemStack.data.amount = 0;

                                player.manager.slotHandler.slotsToUpdate.Add(player.inventory.cursorSlot);
                                return;
                            }
                            else
                            {
                                playerItemStack.itemStack.gunData.bulletCount = playerItemStack.itemStack.gunData.maxBulletCount;
                                player.inventory.cursorSlot.playerItemStack.itemStack.data.amount = bulletsLeftover;

                                player.manager.slotHandler.slotsToUpdate.Add(player.inventory.cursorSlot);
                                return;
                            }
                        }
                    }
                }
                if (ItemStackManager.IsItemTypeSame(player.inventory.cursorSlot.playerItemStack.itemStack, playerItemStack.itemStack))
                {
                    AddToItemStack(player.inventory.cursorSlot, this);
                    return;
                }
                else
                {
                    TransferItemStack(player.inventory.cursorSlot);
                    return;
                }
            }
            if (SlotManager.IsSlotEmpty(player.inventory.cursorSlot) && !SlotManager.IsSlotEmpty(this) || !SlotManager.IsSlotEmpty(player.inventory.cursorSlot) && SlotManager.IsSlotEmpty(this))
            {
                TransferItemStack(player.inventory.cursorSlot);
                return;
            }
        }
        if (slotType == SlotType.result)
        {
            if (player.inventory.cursorSlot.playerItemStack.itemStack.data.item == null)
            {
                TransferItemStack(player.inventory.cursorSlot);
            }
        }
        if (slotType == SlotType.gun)
        {
            if (!SlotManager.IsSlotEmpty(player.inventory.cursorSlot))
            {
                if (player.inventory.cursorSlot.playerItemStack.itemStack.data.item.itemType == ItemType.gun)
                {
                    if (player.inventory.cursorSlot.playerItemStack.itemStack.gunData.bottomRailAttatchment != null)
                    {
                        player.manager.uiManager.weaponWorkbenchUI.modification_bottomRailSlot.playerItemStack.itemStack.data.item = player.inventory.cursorSlot.playerItemStack.itemStack.gunData.bottomRailAttatchment.item;
                        player.manager.uiManager.weaponWorkbenchUI.modification_bottomRailSlot.playerItemStack.itemStack.data.amount = 1;
                    }
                    if (player.inventory.cursorSlot.playerItemStack.itemStack.gunData.topRailAttatchment != null)
                    {
                        player.manager.uiManager.weaponWorkbenchUI.modification_topRailSlot.playerItemStack.itemStack.data.item = player.inventory.cursorSlot.playerItemStack.itemStack.gunData.topRailAttatchment.item;
                        player.manager.uiManager.weaponWorkbenchUI.modification_topRailSlot.playerItemStack.itemStack.data.amount = 1;
                    }
                    if (player.inventory.cursorSlot.playerItemStack.itemStack.gunData.magazineAttatchment != null)
                    {
                        player.manager.uiManager.weaponWorkbenchUI.modification_magazineSlot.playerItemStack.itemStack.data.item = player.inventory.cursorSlot.playerItemStack.itemStack.gunData.magazineAttatchment.item;
                        player.manager.uiManager.weaponWorkbenchUI.modification_magazineSlot.playerItemStack.itemStack.data.amount = 1;
                    }
                    if (player.inventory.cursorSlot.playerItemStack.itemStack.gunData.muzzleAttachment != null)
                    {
                        player.manager.uiManager.weaponWorkbenchUI.modification_muzzleSlot.playerItemStack.itemStack.data.item = player.inventory.cursorSlot.playerItemStack.itemStack.gunData.muzzleAttachment.item;
                        player.manager.uiManager.weaponWorkbenchUI.modification_muzzleSlot.playerItemStack.itemStack.data.amount = 1;
                    }

                    player.manager.uiManager.weaponWorkbenchUI.UpdateSlots();
                    TransferItemStack(player.inventory.cursorSlot);
                }
            }
            else
            {
                TransferItemStack(player.inventory.cursorSlot);
                player.manager.uiManager.weaponWorkbenchUI.ResetPartsSlots();

                player.manager.uiManager.weaponWorkbenchUI.UpdateSlots();
            }
        }
        if (slotType == SlotType.gunPart)
        {
            if (SlotManager.IsSlotEmpty(player.inventory.cursorSlot))
            {
                player.manager.uiManager.weaponWorkbenchUI.updateRecipe = true;
                TransferItemStack(player.inventory.cursorSlot);
                return;
            }
            if (player.inventory.cursorSlot.playerItemStack.itemStack.data.item.itemType == ItemType.gunPart)
            {
                if (gunPartType == player.inventory.cursorSlot.playerItemStack.itemStack.data.item.gunPartData.type)
                {
                    if (!SlotManager.IsSlotEmpty(player.manager.uiManager.weaponWorkbenchUI.modification_weaponSlot))
                    {
                        if (ItemManager.IsWeaponPartCompatible(player.manager.uiManager.weaponWorkbenchUI.modification_weaponSlot.playerItemStack.itemStack, player.inventory.cursorSlot.playerItemStack.itemStack))
                        {
                            player.manager.uiManager.weaponWorkbenchUI.updateRecipe = true;
                            TransferItemStack(player.inventory.cursorSlot);
                        }
                        else
                        {
                            Debug.Log("Weapon Part is not compatible");
                        }
                    }

                }
            }
            player.manager.uiManager.weaponWorkbenchUI.UpdateSlots();
        }
    }

    public bool IsSlotEmpty()
    {
        if (playerItemStack.itemStack.data.item != null) { return false; }
        else { return true; }
    }
    public GameObject CreateSlotIconObject(Slot slot, RenderTexture renderTexture, Camera iconGenCam)
    {

        

        GameObject item = GameObject.Instantiate(slot.playerItemStack.itemStack.data.item.itemData.customGeometry, iconGenCam.transform);
        if (slot.playerItemStack.itemStack.data.item.itemType == ItemType.gun)
        {
            Gun gun = item.GetComponent<Gun>();
            if (gun != null)
            {
                if (slot.playerItemStack.itemStack.gunData.bottomRailAttatchment != null)
                {
                    gun.lowerRail = slot.playerItemStack.itemStack.gunData.bottomRailAttatchment;
                }
                if (slot.playerItemStack.itemStack.gunData.magazineAttatchment != null)
                {
                    gun.magazine = slot.playerItemStack.itemStack.gunData.magazineAttatchment;
                }
                if (slot.playerItemStack.itemStack.gunData.muzzleAttachment != null)
                {
                    gun.muzzle = slot.playerItemStack.itemStack.gunData.muzzleAttachment;
                }
                if (slot.playerItemStack.itemStack.gunData.topRailAttatchment != null)
                {
                    gun.upperRail = slot.playerItemStack.itemStack.gunData.topRailAttatchment;
                }

                gun.GenerateWeapon(true);
            }
        }
        
        item.layer = LayerMask.NameToLayer("Icon");

        item.transform.localPosition = slot.playerItemStack.itemStack.data.item.itemData.iconRenderOffset;
        item.transform.localEulerAngles = slot.playerItemStack.itemStack.data.item.itemData.iconRenderRotation;
        item.transform.localScale = new Vector3(slot.playerItemStack.itemStack.data.item.itemData.renderSize, slot.playerItemStack.itemStack.data.item.itemData.renderSize, slot.playerItemStack.itemStack.data.item.itemData.renderSize);
        foreach (Transform child in item.transform)
        {
            foreach (Transform child2 in child.transform)
            {
                child2.gameObject.layer = LayerMask.NameToLayer("Icon");
            }
            child.gameObject.layer = LayerMask.NameToLayer("Icon");
        }
        return item;
    }
    public Texture2D IconGen(Slot slot, GameObject item, RenderTexture renderTexture, Camera iconGenCam)
    {
        RenderTexture rt = new RenderTexture(renderTexture.width, renderTexture.height, 16, RenderTextureFormat.ARGB64);
        rt.Create();

        Texture2D newTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false, true);
        RenderTexture.active = renderTexture;
        newTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        newTexture.Apply();
        slot.iconImage.color = Color.white;

        GameObject.Destroy(item.gameObject);
        Resources.UnloadUnusedAssets();
        RenderTexture.active = null;
        renderTexture.Release();
        return newTexture;
    }
    public void SwapToOffhand(EventSystem eventSystem, GraphicRaycaster graphicRaycaster)
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);

        Slot _slot = results[0].gameObject.transform.parent.GetComponent<Slot>();
        if (_slot != null)
        {

            if (_slot != _slot.player.inventory.offhand.slot)
            {
                Debug.Log(_slot.gameObject.name);
                _slot.TransferItemStack(_slot.player.inventory.offhand.slot);
            }

        }
    }
}

public enum SlotType
{
    any,
    gunPart,
    gun,
    muzzle,
    topRail,
    lowerRail,
    magazine,
    armour,
    result
}

public static class SlotManager
{
    public static Color fullDurability = new Color(0.03867035f, 0.745283f, 0.07586052f, 1f);
    public static Color lowDurability = new Color(0.5943396f, 0.09544369f, 0.01962443f, 1f);
    public static bool IsSlotEmpty(Slot slot)
    {
        if (slot.playerItemStack.itemStack.data.item != null) { return false; }
        else { return true; }
    }
    public static void UpdateSlot(Slot slot)
    {
        if (slot.playerItemStack.itemStack.data.item == null || slot.playerItemStack.itemStack.data.amount == 0)
        {
            ResetSlot(slot);
            return;
        }
        if (slot.playerItemStack.itemStack.data.item.itemType == ItemType.normal)
        {
            UpdateSlot_Normal(slot);
        }
        if (slot.playerItemStack.itemStack.data.item.itemType == ItemType.tool)
        {
            UpdateSlot_Normal(slot);
            UpdateSlot_Tool(slot);
        }
        if (slot.playerItemStack.itemStack.data.item.itemType == ItemType.food)
        {
            UpdateSlot_Normal(slot);
            UpdateSlot_Food(slot);
        }
    }
    static void UpdateSlot_Normal(Slot slot)
    {
        if (slot.playerItemStack.itemStack.data.amount == 1)
        {
            slot.itemAmountText.text = "";
        }
        else
        {
            slot.itemAmountText.text = slot.playerItemStack.itemStack.data.amount.ToString();
        }

        if (slot.playerItemStack.itemStack.data.item.itemData.customGeometry != null)
        {
            return;
        }
        else
        {
            slot.iconImage.texture = slot.playerItemStack.itemStack.data.item.itemData.texture;
            slot.iconImage.color = Color.white;
        }
    }
    static void UpdateSlot_Tool(Slot slot)
    {
        if (slot.playerItemStack.itemStack.toolData.durability == 0)
        {
            ResetSlot(slot);
            slot.durabilitySlider.gameObject.SetActive(false);
        }
        else
        {
            slot.durabilitySlider.maxValue = slot.playerItemStack.itemStack.data.item.toolData.maxDurability;
            slot.durabilitySlider.value = slot.playerItemStack.itemStack.toolData.durability;
            slot.durabilityFill.color = Color.Lerp(lowDurability, fullDurability, (float)slot.playerItemStack.itemStack.toolData.durability / (float)slot.playerItemStack.itemStack.data.item.toolData.maxDurability);
            slot.durabilitySlider.gameObject.SetActive(true);
        }
        
    }
    static void UpdateSlot_Food(Slot slot)
    {

    }
    public static void ResetSlot(Slot slot)
    {
        slot.iconImage.color = new Color(1, 1, 1, 0);
        slot.iconImage.texture = null;
        slot.durabilitySlider.gameObject.SetActive(false);
        slot.itemAmountText.text = "";
        slot.durabilitySlider.value = 0;
        slot.durabilitySlider.maxValue = 0;
    }
    
    public static bool HasCustomGeometry(Slot slot, ItemStack itemStack)
    {
        if (itemStack.data.item == null) return false;
        if (itemStack.data.item.itemData.customGeometry == null) return false;
        else
        {
            return true;
        }
    }
}