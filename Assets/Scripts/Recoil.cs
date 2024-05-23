using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField] private Transform recoilTransform;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [SerializeField] private float snappiness;
    [SerializeField] private float damping;

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, damping * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        recoilTransform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire(Vector3 spread) 
    {
        targetRotation += spread;
    }
}
