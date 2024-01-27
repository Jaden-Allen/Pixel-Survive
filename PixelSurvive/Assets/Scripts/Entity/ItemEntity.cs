using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemEntity : MonoBehaviour
{
    public ItemStack itemStack;
    public Material material;
    public LayerMask playerMask;

    public Transform itemParent;
    public GameObject item;
    float animationHeight = 0.1f;
    float animationRotationSpeed = 100f;
    float animationSpeed = 0.5f;

    float pickupRange = 2f;
    bool playerInPickupRange;
    public bool pickedUp;
    [SerializeField] RaycastHit[] hits;
    
    Vector3 targetPos = Vector3.zero;
    bool up = false;
    private void Start()
    {
       item = VoxelData.CreateItemObjectFromData(itemStack, material, itemParent);
        
    }
    private void Update()
    {
        CheckForPlayer();
        PlayAnimation();
    }
    void CheckForPlayer()
    {
        playerInPickupRange = Physics.CheckSphere(transform.position, pickupRange, playerMask);

        if (pickedUp)
        {
            return;
        }
        if (playerInPickupRange )
        {
            hits = Physics.SphereCastAll(transform.position, pickupRange, transform.forward, playerMask);

            Player player;

            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.name == "Player")
                {
                    player = hit.collider.GetComponent<Player>();
                    player.inventory.AddItemToInventory(this);
                }
            }
        }
    }
    void PlayAnimation()
    {
        if (pickedUp)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, 4f * Time.deltaTime);
            if (transform.localScale.z <= 0.1)
            {
                Die();
            }
        }
        else
        {
            switch (up)
            {
                case false:
                    targetPos = Vector3.MoveTowards(targetPos, new Vector3(0f, -animationHeight, 0f), animationSpeed * Time.deltaTime);
                    if (targetPos.y <= -animationHeight + 0.01f) { up = true; }
                    break;
                case true:
                    targetPos = Vector3.MoveTowards(targetPos, new Vector3(0f, animationHeight, 0f), animationSpeed * Time.deltaTime);
                    if (targetPos.y >= animationHeight - 0.01f) { up = false; }
                    break;
            }
            itemParent.transform.localPosition = targetPos;
            itemParent.transform.Rotate(0f, animationRotationSpeed * Time.deltaTime, 0f);
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
}