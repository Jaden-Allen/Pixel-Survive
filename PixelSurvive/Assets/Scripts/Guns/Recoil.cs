using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [SerializeField] private float snapiness;
    [SerializeField] private float returnSpeed;

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snapiness * Time.fixedDeltaTime);

        targetRotation.x = Mathf.Clamp(targetRotation.x, -30, 60);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    public void RecoilFire(ItemStack itemStack)
    {
        targetRotation += new Vector3(-itemStack.gunData.recoil, Random.Range(-itemStack.gunData.recoil / 2, itemStack.gunData.recoil / 2), Random.Range(-itemStack.gunData.recoil / 3, itemStack.gunData.recoil / 3));
    }
}
