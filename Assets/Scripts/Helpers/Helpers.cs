using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    static readonly Dictionary<float, WaitForSeconds> WaitForSeconds = new();

    public static WaitForSeconds GetWaitForSeconds(float seconds)
    {
        if (WaitForSeconds.TryGetValue(seconds, out var forSeconds)) return forSeconds;

        var waitForSeconds = new WaitForSeconds(seconds);
        WaitForSeconds.Add(seconds, waitForSeconds);
        return WaitForSeconds[seconds];
    }

    public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
