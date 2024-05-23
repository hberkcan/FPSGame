using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CrosshairFocus : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private InputReader input;

    private static int FocusHash;

    private float duration = 0.1f;
    private float focusTarget = 1f;
    private float focus;

    private void Awake()
    {
        FocusHash = Animator.StringToHash("Focus");
    }

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
            DOTween.To(() => focus, x => focus = x, focusTarget, duration)
                .OnUpdate(() => {
                    animator.SetFloat(FocusHash, focus);
                });
        }
        else
        {
            DOTween.To(() => focus, x => focus = x, 0, duration)
                .OnUpdate(() => {
                    animator.SetFloat(FocusHash, focus);
                });
        }
    }
}
