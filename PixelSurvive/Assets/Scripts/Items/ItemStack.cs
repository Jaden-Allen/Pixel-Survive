using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

[System.Serializable]
public class ItemStack
{
    public ItemStack_Base data;
    public ItemStack_Tool toolData;
    public ItemStack_Gun gunData;
    public ItemStack(ItemStack_Base _data, ItemStack_Tool _toolData, ItemStack_Gun _gunData)
    {
        this.data = _data;
        this.toolData = _toolData;
        this.gunData = _gunData;
    }

    public ItemStack Clone()
    {
        ItemStack newStack = (ItemStack)this.MemberwiseClone();

        ItemStack_Base newdata = new ItemStack_Base();
        ItemStack_Tool newtoolData = new ItemStack_Tool();
        ItemStack_Gun newgunData = new ItemStack_Gun();

        newdata.item = data.item;
        newdata.amount = data.amount;

        newtoolData.durability = toolData.durability;

        newgunData.bulletCount = gunData.bulletCount;
        newgunData.maxBulletCount = gunData.maxBulletCount;
        newgunData.range = gunData.range;
        newgunData.damage = gunData.damage;
        newgunData.recoil = gunData.recoil;
        newgunData.damageChance = gunData.damageChance;
        newgunData.durability = gunData.durability;
        newgunData.maxDurability = gunData.maxDurability;
        newgunData.activeFireType = gunData.activeFireType;
        newgunData.topRailAttatchment = gunData.topRailAttatchment;
        newgunData.bottomRailAttatchment = gunData.bottomRailAttatchment;
        newgunData.muzzleAttachment = gunData.muzzleAttachment;
        newgunData.magazineAttatchment = gunData.magazineAttatchment;
        newgunData.aimModifier = gunData.aimModifier;

        newStack.data = newdata;
        newStack.toolData = newtoolData;
        newStack.gunData = newgunData;

        return newStack;
    }
    public ItemStack Clear()
    {
        ItemStack newStack = this;

        ItemStack_Base newdata = new ItemStack_Base();
        ItemStack_Tool newtoolData = new ItemStack_Tool();
        ItemStack_Gun newgunData = new ItemStack_Gun();

        newStack.data = newdata;
        newStack.toolData = newtoolData;
        newStack.gunData = newgunData;

        return newStack;
    }
}
[System.Serializable]
public class ItemStack_Base
{
    public Item item;
    public int amount;
}
[System.Serializable]
public class ItemStack_Tool
{
    public int durability;
}
[System.Serializable]
public class ItemStack_Gun
{
    public Attatchment topRailAttatchment;
    public Attatchment bottomRailAttatchment;
    public Attatchment muzzleAttachment;
    public Attatchment magazineAttatchment;

    public int bulletCount;
    public int maxBulletCount;
    public float range;
    public int damage;
    public float recoil;
    public float damageChance;
    public int durability;
    public int maxDurability;
    public float aimModifier;
    public FireType activeFireType;
}
public static class ItemStackManager
{
    public static bool IsItemTypeSame(ItemStack itemStack1, ItemStack itemStack2)
    {
        if (itemStack1.data.item.itemData.typeId == itemStack2.data.item.itemData.typeId) { return true; }

        else { return false; }
    }
    public static bool HasCustomGeometry(ItemStack itemStack)
    {
        if (itemStack.data.item.itemData.customGeometry != null) { return true; }

        else { return false; }
    }
    public static Attatchment[] GetActiveAttatchments(ItemStack itemStack)
    {
        List<Attatchment> attatchments = new List<Attatchment>();
        if (itemStack.gunData.bottomRailAttatchment != null)
        {
            attatchments.Add(itemStack.gunData.bottomRailAttatchment);
        }
        if (itemStack.gunData.topRailAttatchment != null)
        {
            attatchments.Add(itemStack.gunData.topRailAttatchment);
        }
        if (itemStack.gunData.magazineAttatchment != null)
        {
            attatchments.Add(itemStack.gunData.magazineAttatchment);
        }
        if (itemStack.gunData.muzzleAttachment != null)
        {
            attatchments.Add(itemStack.gunData.muzzleAttachment);
        }

        return attatchments.ToArray();
    }
}