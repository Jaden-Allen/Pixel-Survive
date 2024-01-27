using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Manager manager;
    public PlayerLook look;
    public PlayerHealth health;
    public PlayerMovement movement;
    public PlayerAttack attack;
    public PlayerHands hands;
    public PlayerInventory inventory;
    public PlayerShoot shoot;
    private void Awake()
    {
        manager = GameObject.FindAnyObjectByType<Manager>();
    }
}
