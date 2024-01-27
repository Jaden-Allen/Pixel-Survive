using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerHands : MonoBehaviour
{
    public Player player;
    [Header("Hand Vars")]
    public Transform rightHand;
    public Transform leftHand;
    public Animator anim;

    public GameObject rightHandItem;
    public GameObject leftHandItem;

    [Header("Hand Bob Vars")]
    public float distance;
    public float jumpDistance;
    public HandBobSpeeds bobSpeeds;
    float maxDistance = 0.01f;
    Direction dir = Direction.None;
    Vector3 newPos = Vector3.zero;
    Vector3 targetPos = Vector3.zero;


    [Header("Swing Vars")]
    public PlayerAttack playerAttack;
    [SerializeField] private Route[] routes;
    private int routeToGo;
    private float tParam;
    private Vector3 catPos;
    [SerializeField] private Vector3 catRotHand;
    public float swingSpeed;
    bool isSwinging = false;
    private void Update()
    {
        if (player.manager.uiManager.uiOpen) { return; }
        if (!SlotManager.IsSlotEmpty(player.inventory.mainHotbar.slots[player.inventory.mainHotbar.selectedSlotIndex]))
        {
            if (player.inventory.mainHotbar.slots[player.inventory.mainHotbar.selectedSlotIndex].playerItemStack.itemStack.data.item.itemType == ItemType.gun) { return; }
        }
        
        if (Input.GetButton("Fire1") && !isSwinging)
        {
            routeToGo = 0;
            tParam = 0f;
            catRotHand = Vector3.zero;

            StartCoroutine(Swing(routeToGo));
        }
        anim.SetBool("isSwinging", isSwinging);
    }

    IEnumerator Swing(int routeNumber)
    {
        isSwinging= true;
        anim.SetLayerWeight(1, 1f);
        playerAttack.Attack();

        while (tParam < 1)
        {
            Vector3 p0 = routes[routeNumber].controlPoints[0].position;
            Vector3 p1 = routes[routeNumber].controlPoints[1].position;
            Vector3 p2 = routes[routeNumber].controlPoints[2].position;
            Vector3 p3 = routes[routeNumber].controlPoints[3].position;

            tParam += Time.deltaTime * swingSpeed;

            catPos = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                Mathf.Pow(tParam, 3) * p3;
            if (tParam < 0.4)
            {
                catRotHand = new Vector3(tParam * 40f, 0f, 0f);
            }
            else
            {
                catRotHand = Vector3.Lerp(rightHand.localEulerAngles, Vector3.zero, 12f * Time.deltaTime);
            }
            

            rightHand.transform.position = catPos;
            rightHand.localRotation = Quaternion.Euler(catRotHand.x, catRotHand.y, catRotHand.z);
            yield return new WaitForEndOfFrame();
        }
        tParam = 0f;

        routeToGo += 1;

        if (routeToGo > routes.Length)
        {
            routeToGo = 0;
        }
        
        anim.SetLayerWeight(1, 0f);
        isSwinging = false;
    }
    public void HandMovement(bool isMoving, bool isRunning, bool isGrounded)
    {
        if (isSwinging)
        {
            rightHand.localPosition = Vector3.Lerp(rightHand.localPosition, Vector3.zero, bobSpeeds.returnToNormalSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 rot = Vector3.Lerp(rightHand.localEulerAngles, Vector3.zero, 12f * Time.deltaTime);

            rightHand.localEulerAngles = rot;
            rightHand.localPosition = new Vector3(0.5f, newPos.y, 0f);
        }
        if (isGrounded)
        {
            if (isMoving)
            {
                if (isRunning)
                {
                    HandMovementSprint();
                }
                else
                {
                    HandMovementWalk();
                }
            }
            else
            {
                HandMovementNone();
            }
        }
        else
        {
            HandMovementJump();
        }
        

        
        leftHand.localPosition = new Vector3(-0.5f, newPos.y, 0f);

    }
    void HandMovementNone()
    {
        dir = Direction.None;
        newPos = Vector3.Lerp(newPos, Vector3.zero, bobSpeeds.returnToNormalSpeed * Time.deltaTime);
    }
    void HandMovementWalk()
    {
        if (dir == Direction.None)
        {
            dir = Direction.Up;
        }

        if (dir == Direction.Up)
        {
            targetPos = new Vector3(0f, distance, 0f);

            if (Vector3.Distance(newPos, targetPos) < maxDistance)
            {
                dir = Direction.Down;
            }
        }
        if (dir == Direction.Down)
        {
            targetPos = new Vector3(0f, -1 * distance, 0f);

            if (Vector3.Distance(newPos, targetPos) < maxDistance)
            {
                dir = Direction.Up;
            }
        }
        newPos = Vector3.Lerp(newPos, targetPos, bobSpeeds.walkSpeed * Time.deltaTime);
    }
    void HandMovementSprint()
    {
        if (dir == Direction.None)
        {
            dir = Direction.Up;
        }

        if (dir == Direction.Up)
        {
            targetPos = new Vector3(0f, distance, 0f);

            if (Vector3.Distance(newPos, targetPos) < maxDistance)
            {
                dir = Direction.Down;
            }
        }
        if (dir == Direction.Down)
        {
            targetPos = new Vector3(0f, -1 * distance, 0f);

            if (Vector3.Distance(newPos, targetPos) < maxDistance)
            {
                dir = Direction.Up;
            }
        }
        newPos = Vector3.Lerp(newPos, targetPos, bobSpeeds.sprintSpeed * Time.deltaTime);
    }
    void HandMovementJump()
    {
        targetPos = new Vector3(0f, jumpDistance, 0f);

        newPos = Vector3.Lerp(newPos, targetPos, bobSpeeds.jumpSpeed * Time.deltaTime);
    }

}
    public enum Direction
    {
        Up,
        Down,
        None
    }

[System.Serializable]
public class HandBobSpeeds
{
    public float walkSpeed;
    public float sprintSpeed;
    public float jumpSpeed;
    public float returnToNormalSpeed;
}