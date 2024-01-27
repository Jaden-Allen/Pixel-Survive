using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float sensitivity;
    public Player playerBody;

    public Transform playerHead;

    [SerializeField] float xRotation = 0;

    [SerializeField] private Vector3 currentRotation;
    [SerializeField] private Vector3 targetRotation;

    [SerializeField] private float snapiness;
    [SerializeField] private float returnSpeed;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible= false;
    }
    private void Update()
    {
        if (!playerBody.manager.uiManager.uiOpen)
        {
            NormalLook();
        }
        

    }
    void NormalLook()
    {
        float inputX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float inputY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        targetRotation = Vector3.Lerp(targetRotation, new Vector3(0f, 0f, 0f), returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snapiness * Time.fixedDeltaTime);

        xRotation -= inputY;
        xRotation += currentRotation.x;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        

        playerBody.transform.Rotate(Vector3.up * inputX);
        transform.localRotation = Quaternion.Euler(xRotation, currentRotation.y, currentRotation.z);
        playerHead.localRotation = Quaternion.Euler(-1f * xRotation, 0f, 0f);
    }
    public void RecoilFire(ItemStack itemStack)
    {
        targetRotation += new Vector3(-itemStack.gunData.recoil / 4, Random.Range(-itemStack.gunData.recoil, itemStack.gunData.recoil), Random.Range(-itemStack.gunData.recoil / 2, itemStack.gunData.recoil / 2));
    }
    public void Aim(float aimScale)
    {

    }
}
