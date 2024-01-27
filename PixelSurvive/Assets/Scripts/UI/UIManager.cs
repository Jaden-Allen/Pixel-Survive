using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Player player;
    public Hunger hunger;
    public Hearts hearts;
    public WeaponWorkenchUI weaponWorkbenchUI;
    public GameObject hotbarUI;
    public GameObject inventoryUITop;
    public GameObject inventoryUIBottom;
    public LayerMask interactableMask;

    public RenderTexture renderTexture;
    public Camera iconCam;

    public bool uiOpen;
    public bool inventoryOpen;
    public bool workbenchOpen;
    Vector3 scale;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (workbenchOpen)
            {
                OpenWeaponWorkbench();
            }
            else
            {
                OpenInventory();
            }
        }


        


        if (Input.GetButtonDown("Fire2") && !workbenchOpen && !inventoryOpen)
        {
            RaycastHit hit;
            Physics.Raycast(player.movement.cam.transform.position, player.movement.cam.transform.forward, out hit, 4f, interactableMask);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.tag == "Weapon Workbench")
                {
                    OpenWeaponWorkbench();
                }
            }

            
        }
    }
    public void OpenWeaponWorkbench()
    {
        if (scale == Vector3.one)
        {
            Cursor.lockState = CursorLockMode.Locked;
            uiOpen = false;
            workbenchOpen = false;
            scale = Vector3.zero;
        }
        else
        {
            uiOpen = true;
            workbenchOpen = true;
            Cursor.lockState = CursorLockMode.Confined;
            scale = Vector3.one;
        }

        weaponWorkbenchUI.weaponWorkbenchUI.transform.localScale = scale;
        inventoryUIBottom.transform.localScale = scale;
        weaponWorkbenchUI.weaponRepairPage.SetActive(false);
        weaponWorkbenchUI.UpdateSlots();
        Cursor.visible = uiOpen;
    }
    public void OpenInventory()
    {
        if (scale == Vector3.one)
        {
            Cursor.lockState = CursorLockMode.Locked;
            inventoryOpen = false;
            uiOpen = false;
            scale = Vector3.zero;
        }
        else
        {
            uiOpen = true;
            inventoryOpen = true;
            Cursor.lockState = CursorLockMode.Confined;
            scale = Vector3.one;
        }

        inventoryUITop.transform.localScale = scale;
        inventoryUIBottom.transform.localScale = scale;
        Cursor.visible = uiOpen;
    }
}
