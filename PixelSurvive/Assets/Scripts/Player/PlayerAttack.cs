using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Hotbar hotbar;
    public Transform fpsCam;
    public float attackRange = 5f;
    public LayerMask enemyMask;
    public int baseDamage = 2;
    
    public void Attack()
    {
        RaycastHit hit;
        Physics.Raycast(fpsCam.position, fpsCam.forward, out hit, attackRange, enemyMask);

        if (hit.collider != null)
        {
            Entity entity = hit.collider.GetComponent<Entity>();
            if (entity != null)
            {
                

                entity.TakeDamage(GetDamage());
            }
        }
    }
    int GetDamage()
    {
        int damage;
        Slot selectedSlot = hotbar.slots[hotbar.selectedSlotIndex];

        if (selectedSlot.playerItemStack.itemStack.data.item != null)
        {
            if (selectedSlot.playerItemStack.itemStack.data.item.itemType != ItemType.tool)
            {
                damage = baseDamage;
            }
            else
            {
                damage = hotbar.slots[hotbar.selectedSlotIndex].playerItemStack.itemStack.data.item.toolData.damage;
            }
        }
        else
        {
            damage = baseDamage;
        }

        return damage;
    }
}
