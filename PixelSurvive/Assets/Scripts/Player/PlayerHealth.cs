using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public Player player;

    public void TakeDamage(int damage)
    {
        if ((health - damage) <= 0)
        {
            health = 0;
        }
        else
        {
            health -= damage;
        }
        HeartManager.UpdateHeartUI(player.manager.uiManager.hearts, health);
    }
}
