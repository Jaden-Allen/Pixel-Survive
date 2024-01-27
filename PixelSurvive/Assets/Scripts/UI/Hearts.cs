using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hearts : MonoBehaviour
{
    public RawImage[] heartIcons;
    public Texture2D fullHeart;
    public Texture2D halfHeart;
    public Texture2D emptyHeart;
}
public static class HeartManager
{
    public static void UpdateHeartUI(Hearts hearts, int heartAmount)
    {
        for (int p = 2, i = 9; i >= 0; i--, p += 2)
        {
            if (p <= heartAmount)
            {
                hearts.heartIcons[i].texture = hearts.fullHeart;
            }
            if (p - 1 == heartAmount)
            {
                hearts.heartIcons[i].texture = hearts.halfHeart;
            }
            else if (p > heartAmount)
            {
                hearts.heartIcons[i].texture = hearts.emptyHeart;
            }
        }
    }
}