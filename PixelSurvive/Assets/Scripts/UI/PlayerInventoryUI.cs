using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour
{
    public Slots hotbar;
    public Slots inventory;

    [HideInInspector] public Player player;

    public List<Slot> allSlots= new List<Slot>();

    private void Start()
    {
        player = GameObject.FindAnyObjectByType<Player>();
        allSlots.Clear();
        foreach (var slot in hotbar.slots)
        {
            allSlots.Add(slot);
        }
        foreach (var slot in inventory.slots)
        {
            allSlots.Add(slot);
        }

        UpdateAllSlots();
    }
    public void UpdateAllSlots()
    {
        foreach(Slot slot in allSlots)
        {
            player.manager.slotHandler.slotsToUpdate.Add(slot);
        }
    }
}
