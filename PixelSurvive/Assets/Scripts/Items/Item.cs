using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName ="Scriptable Objects/Items/Default Item")]
public class Item : ScriptableObject
{
    public ItemType itemType;

    public ItemData itemData;
    public ToolData toolData;
    public FoodData foodData;
    public GunPartData gunPartData;
    public GunData gunData;
    public AmmoData ammoData;
}

[System.Serializable]
public class ItemData
{
    [Header("Base Data")]
    public string typeId;
    public Texture2D texture;
    public int maxStackSize = 1;
    public GameObject customGeometry;

    [Header("Item Generation")]
    [Range(0f, 2f)] public float size = 0.5f;
    [Range(0, 16)] public float thickness = 1;
    public Vector3 renderOffset;
    public Vector3 renderRotation = new Vector3(0f, 90f, 0f);

    [Header("Icon Generation")]
    [Range(0f, 20f)] public float renderSize = 1f;
    public Vector3 iconRenderOffset = new Vector3(-0.2f, -0.5f, 1.6f);
    public Vector3 iconRenderRotation = new Vector3(0f, 135f, 0f);
}
public static class ItemManager
{
    public static bool IsWeaponPartCompatible(ItemStack gun, ItemStack gunPart)
    {
        if (gun.data.item.gunData.gunType == gunPart.data.item.gunPartData.gunType) { return true; }
        else { return false; }
    }
}

[System.Serializable]
public class ToolData
{
    public int maxDurability;

    public float miningSpeed;
    public int damage;
}

[System.Serializable]
public class FoodData
{
    public int saturation;
}
[System.Serializable]
public class GunPartData
{
    public GunPartType type;
    public GunType gunType;
    public Attatchment attatchment;

    public float recoil;
    public int damage;
    public float range;
    public int maxBullets;
    [Range(-1f, 1f)]public float damageChance;
    public int durability;
    public float aimModifier;
     
    public FireType[] fireTypes;
}
[System.Serializable]
public class GunData
{
    public GunType gunType;

    public float recoil;
    public int damage;
    public float range;
    public int maxBullets;
    public float fireRate;
    [Range(0f, 1f)] public float damageChance;
    public int maxDurability;
    public float aimModifier;
    public FireType[] fireTypes;

    public AudioClip emptyClip;
    public AudioClip fireClip;
    public AudioClip reloadClip;
}
[System.Serializable]
public class AmmoData
{
    public GunType type;
}

public enum ItemType
{
    normal,
    tool,
    food,
    gun,
    gunPart,
    ammo
}
public enum GunPartType
{
    main,
    magazine,
    upperRail,
    lowerRail,
    muzzle
}
public enum GunType
{
    rifle,
    pistol
}
public enum FireType
{
    single,
    burst,
    fullAuto
}