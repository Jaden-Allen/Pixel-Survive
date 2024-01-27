using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Hotbar hotbar;
    public Camera cam;
    public GameObject hitEffect;
    public Player player;

    public float timeUntilNextFire = 0;
    public int fireTypeIndex = 0;
    public bool updatedData = false;
    public Recoil recoil;
    public ItemStack lastItemStack;
    public AudioSource audioSource;
    private void Update()
    {
        if (player.manager.uiManager.uiOpen) { return; }
        if (!SlotManager.IsSlotEmpty(hotbar.slots[hotbar.selectedSlotIndex]))
        {
            ChangeFireType();
            AttemptToShoot();
            UpdateItemStackData();
            Aim();
        }
    }
    void UpdateItemStackData()
    {
        if (hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.data.item.itemType == ItemType.gun)
        {
            ItemStack itemStack = hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack;
            if (lastItemStack != itemStack)
            {
                updatedData = false;
            }
            if (!updatedData)
            {
                lastItemStack = itemStack;
                hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.gunData.damage = GetDamage(itemStack);
                hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.gunData.range = GetRange(itemStack);
                hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.gunData.maxBulletCount = GetMaxBullets(itemStack);
                hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.gunData.recoil = GetRecoil(itemStack);
                hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.gunData.damageChance = GetDamageChance(itemStack);
                hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.gunData.maxDurability = GetDurability(itemStack);
                hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.gunData.aimModifier = GetAim(itemStack);

                updatedData = true;
            }
        }
    }
    void Aim()
    {
        if (hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.data.item.itemType == ItemType.gun)
        {
            if (Input.GetButton("Fire2"))
            {
                player.movement.aimModifier = (hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.gunData.aimModifier);
            }
            else
            {
                player.movement.aimModifier = 0f;
            }
        }
    }
    float GetAim(ItemStack itemStack)
    {
        float aimScale = 0;
        aimScale += itemStack.data.item.gunData.aimModifier;
        foreach (Attatchment attatchment in ItemStackManager.GetActiveAttatchments(itemStack))
        {
            aimScale += attatchment.item.gunPartData.aimModifier;
        }

        return aimScale;
    }
    int GetDamage(ItemStack itemStack)
    {
        int damage = 0;
        damage += itemStack.data.item.gunData.damage;
        foreach (Attatchment attatchment in ItemStackManager.GetActiveAttatchments(itemStack)) 
        {
            damage += attatchment.item.gunPartData.damage;
        }
        
        return damage;
    }
    int GetMaxBullets(ItemStack itemStack)
    {
        int bullets = 0;
        bullets += itemStack.data.item.gunData.maxBullets;
        foreach (Attatchment attatchment in ItemStackManager.GetActiveAttatchments(itemStack))
        {
            bullets += attatchment.item.gunPartData.maxBullets;
        }
        
        return bullets;
    }
    float GetRange(ItemStack itemStack)
    {
        float range = 0;
        range += itemStack.data.item.gunData.range;
        foreach (Attatchment attatchment in ItemStackManager.GetActiveAttatchments(itemStack))
        {
            range += attatchment.item.gunPartData.range;
        }
        return range;
    }
    float GetRecoil(ItemStack itemStack)
    {
        float recoil = 0;
        recoil += itemStack.data.item.gunData.recoil;
        foreach (Attatchment attatchment in ItemStackManager.GetActiveAttatchments(itemStack))
        {
            recoil += attatchment.item.gunPartData.recoil;
        }
        return recoil;
    }
    float GetDamageChance(ItemStack itemStack)
    {
        float damageChance = 0;
        damageChance += itemStack.data.item.gunData.damageChance;
        foreach (Attatchment attatchment in ItemStackManager.GetActiveAttatchments(itemStack))
        {
            damageChance += attatchment.item.gunPartData.damageChance;
        }
        return damageChance;
    }
    int GetDurability(ItemStack itemStack)
    {
        int durability = 0;
        durability += itemStack.data.item.gunData.maxDurability;
        foreach (Attatchment attatchment in ItemStackManager.GetActiveAttatchments(itemStack))
        {
            durability += attatchment.item.gunPartData.durability;
        }
        return durability;
    }
    void ChangeFireType()
    {
        if (hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.data.item.itemType == ItemType.gun)
        {
            ItemStack itemStack = hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack;
            if (Input.GetKeyDown(KeyCode.B))
            {
                fireTypeIndex++;

                if (fireTypeIndex > itemStack.data.item.gunData.fireTypes.Length - 1)
                {
                    fireTypeIndex = 0;
                }
                if (fireTypeIndex < 0)
                {
                    fireTypeIndex = itemStack.data.item.gunData.fireTypes.Length - 1;
                }
                itemStack.gunData.activeFireType = itemStack.data.item.gunData.fireTypes[fireTypeIndex];
            }
        }
    }
    void AttemptToShoot()
    {
        if (hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.data.item.itemType == ItemType.gun)
        {
            ItemStack itemStack = hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack;
            if (itemStack.gunData.bulletCount > 0)
            {
                if (itemStack.gunData.activeFireType == FireType.single)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        itemStack.gunData.bulletCount--;
                        Shoot(itemStack);
                    }
                }
                if (itemStack.gunData.activeFireType == FireType.burst)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        for (int i = 3; i > 0; i--)
                        {
                            if (itemStack.gunData.bulletCount > 0)
                            {
                                itemStack.gunData.bulletCount--;
                                Shoot(itemStack);
                            }
                        }
                        
                    }
                }
                if (itemStack.gunData.activeFireType == FireType.fullAuto)
                {
                    
                    if (Input.GetButton("Fire1") && Time.time > timeUntilNextFire)
                    {
                        timeUntilNextFire = Time.time + itemStack.data.item.gunData.fireRate;

                        itemStack.gunData.bulletCount--;
                        Shoot(itemStack);
                    }
                }
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                PlayAudio(GunAudioType.empty, itemStack);
            }
        }
    }
    public void PlayAudio(GunAudioType gunAudioType, ItemStack itemStack)
    {
        if (gunAudioType == GunAudioType.empty)
        {
            audioSource.clip = itemStack.data.item.gunData.emptyClip;
            audioSource.loop = false;
            audioSource.Play();
        }
        if (gunAudioType == GunAudioType.fire)
        {
            audioSource.clip = itemStack.data.item.gunData.fireClip;
            audioSource.loop = false;
            audioSource.Play();
        }
        if (gunAudioType == GunAudioType.reload)
        {
            audioSource.clip = itemStack.data.item.gunData.reloadClip;
            audioSource.loop = false;
            audioSource.Play();
        }
    }
    void Shoot(ItemStack itemStack)
    {
        float damageAttempt = Random.Range(0.0f, 1.0f);

        if (damageAttempt > itemStack.gunData.damageChance)
        {
            itemStack.gunData.durability--;
            player.manager.slotHandler.slotsToUpdate.Add(player.inventory.hotbar.slots[player.inventory.mainHotbar.selectedSlotIndex]);
            player.manager.slotHandler.slotsToUpdate.Add(player.inventory.mainHotbar.slots[player.inventory.mainHotbar.selectedSlotIndex]);
        }

        PlayAudio(GunAudioType.fire, itemStack);
        player.look.RecoilFire(itemStack);
        RaycastHit hit;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, itemStack.gunData.range);
        if (hit.collider != null)
        {
            Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Entity entity = hit.collider.GetComponent<Entity>();
            
            if (entity != null)
            {
                entity.TakeDamage(itemStack.gunData.damage);
            }
        }
    }
}
public enum GunAudioType
{
    empty,
    fire,
    reload
}