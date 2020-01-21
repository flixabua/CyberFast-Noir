/*
 * written by Jonas Hack
 * 
 * every object that only exists in one dimension
 * 
 * changes physics layer, material and shadow casting mode accordingly
 * 
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DSwitchable : MonoBehaviour
{
    private Material[] myMaterial;
    private int[] myLayer = new int[0];
    private UnityEngine.Rendering.ShadowCastingMode[] castShadow = new UnityEngine.Rendering.ShadowCastingMode[0];
    public int dimensionIndex = 0;//only used for initialisation
    private Dimension dimension;

    private Renderer[] render = new Renderer[0];

    private void Awake()//Initiallize before intial switch
    {

        if (render.Length == 0) render = GetComponentsInChildren<Renderer>();
        if (render.Length == 0) Debug.LogError("There is no valid renderer for " + name);

        myMaterial = new Material[render.Length];
        for (int i = 0; i < render.Length; i++)
        {
            myMaterial[i] = render[i].material;
        }

        myLayer = new int[render.Length];
        for (int i = 0; i < render.Length; i++)
        {
            myLayer[i] = render[i].gameObject.layer;
        }

        castShadow = new UnityEngine.Rendering.ShadowCastingMode[render.Length];
        for (int i = 0; i < render.Length; i++)
        {
            castShadow[i] = render[i].shadowCastingMode;
        }

        dimension = DSwitcher.register(this);
        if (dimension == null) Debug.LogWarning(name + " failed to register to " + dimensionIndex);
        else Debug.Log(name + " registered to dimension " + dimension.index);
    }

    public void switchTo(int dim)
    {
        if (dimension != null)
        {
            for (int i = 0; i < render.Length; i++)
            {
                if (dimension.index != dim)
                {
                    render[i].material = dimension.mat;
                    render[i].gameObject.layer = 9;//pass through
                    render[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                else
                {
                    render[i].material = myMaterial[i];
                    render[i].gameObject.layer = myLayer[i];
                    render[i].shadowCastingMode = castShadow[i];
                }
            }
        }
        else Debug.LogWarning(name + " tried to switch dimension, but its own dimension is null");
    }
}
