/*
 * written by Jonas hack
 *
 * appies stuff like fov-punch to cameras
 * gets triggered by CameraEffectsMaster
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    private Camera cam;
    private float standartFov = 60;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        if (cam == null) Debug.LogError(name + " could not find a camera");
        standartFov = cam.fieldOfView;
    }

    public IEnumerator fovPunch(float strength, float duration, AnimationCurve curve)
    {   
        for (float i = 0; i <= duration; i += Time.deltaTime)
        {
            float currentChange = strength * (curve.Evaluate(i / duration));
            cam.fieldOfView = standartFov + currentChange;
            
            yield return new WaitForEndOfFrame();
        }
        cam.fieldOfView = standartFov;

        yield return null;
    }
}
