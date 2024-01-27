using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hunger : MonoBehaviour
{
    public RawImage[] hungerIcons;
    public Texture2D fullHunger;
    public Texture2D halfHunger;
    public Texture2D emptyHunger;

    private void Start()
    {
        HungerManager.UpdateHungerUI(this, 20);
    }
}
public static class HungerManager
{
    public static void UpdateHungerUI(Hunger hunger, int hungerAmount)
    {
        for (int p = 2, i = 9; i >= 0; i--, p += 2)
        {
            if (p <= hungerAmount)
            {
                hunger.hungerIcons[i].texture = hunger.fullHunger;
            }
            if (p - 1 == hungerAmount)
            {
                hunger.hungerIcons[i].texture = hunger.halfHunger;
            }
            else if (p > hungerAmount)
            {
                hunger.hungerIcons[i].texture = hunger.emptyHunger;
            }
        }
    }
}