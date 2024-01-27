using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slots : MonoBehaviour
{
    public List<Slot> slots = new List<Slot>();

    private void Start()
    {
        GetSlots();
    }
    void GetSlots()
    {
        slots = transform.GetComponentsInChildren<Slot>().ToList();
    }
}
