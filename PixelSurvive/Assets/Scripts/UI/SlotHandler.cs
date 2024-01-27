using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotHandler : MonoBehaviour
{
    public List<Slot> slotsToUpdate = new List<Slot>();
    public List<Slot> updatingSlots= new List<Slot>();
    public RenderTexture renderTexture;
    public Camera iconGenCam;

    bool updating;
    private void Update()
    {
        if (slotsToUpdate.Count > 0 && !updating)
        {
            StartCoroutine(UpdateSlots());
        }
    }
    public IEnumerator UpdateSlots()
    {
        updating = true;
        foreach(Slot slot in slotsToUpdate)
        {
            updatingSlots.Add(slot);
        }
        foreach(Slot slot in updatingSlots)
        {
            
            slot.UpdateSlot(renderTexture, iconGenCam);
            slotsToUpdate.Remove(slot);
            yield return new WaitForEndOfFrame();
        }
        updatingSlots.Clear();

        updating= false;
    }
}
