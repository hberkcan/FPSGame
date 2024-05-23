using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFocus : MonoBehaviour
{
    [SerializeField] Transform gun;
    [SerializeField] Transform mainGunPos;
    [SerializeField] Transform focusGunPos;
    [SerializeField] InputReader input;

    private float duration = 0.1f;
    private float focus;

    private void OnEnable()
    {
        input.Focus += Focus;
    }

    private void OnDisable()
    {
        input.Focus -= Focus;
    }

    private void Focus(bool isFocusing)
    {
        if (isFocusing)
        {
            DOTween.To(() => focus, x => focus = x, 1f, duration)
                .OnUpdate(() => {
                    gun.position = Vector3.Lerp(mainGunPos.position, focusGunPos.position, focus);
                });
        }
        else
        {
            DOTween.To(() => focus, x => focus = x, 0, duration)
                .OnUpdate(() => {
                    gun.position = Vector3.Lerp(mainGunPos.position, focusGunPos.position, focus);
                });
        }
    }
}
