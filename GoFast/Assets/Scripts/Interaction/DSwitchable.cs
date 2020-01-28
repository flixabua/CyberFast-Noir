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
    public int dimensionIndex = 0;//only used for initialisation
    private Dimension dimension;


    GameObject[] children = new GameObject[0];

    private Renderer[] render = new Renderer[0];
    private UnityEngine.Rendering.ShadowCastingMode[] castShadow = new UnityEngine.Rendering.ShadowCastingMode[0];

    private List<Material[]> myMaterial = new List<Material[]>();
    private List<Material[]> dimMaterial = new List<Material[]>();

    private int[] myLayer = new int[0];
    private int layer = 0;

    
    private void Awake()//Initiallize before intial switch
    {
        dimension = DSwitcher.register(this);//register to switcher
        if (dimension == null) Debug.LogWarning(name + " failed to register to " + dimensionIndex);
        else Debug.Log(name + " registered to dimension " + dimension.index);


        Transform[] childTrans = gameObject.GetComponentsInChildren<Transform>();//gat all childern
        children = new GameObject[childTrans.Length];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = childTrans[i].gameObject;
        }

        if (render.Length == 0) render = GetComponentsInChildren<Renderer>();//get renderes
        if (render.Length == 0) Debug.LogError("There is no valid renderer for " + name);


        myLayer = new int[children.Length];//get layers of children
        for (int i = 0; i < children.Length; i++)
        {
            myLayer[i] = children[i].layer;
        }
        layer = gameObject.layer;

        castShadow = new UnityEngine.Rendering.ShadowCastingMode[render.Length];//get show of renders
        for (int i = 0; i < render.Length; i++)
        {
            castShadow[i] = render[i].shadowCastingMode;
        }

        for (int i = 0; i < render.Length; i++)//get materials of renders
        {
            myMaterial.Add(render[i].materials);
            Material[] mat = new Material[render[i].materials.Length];
            for (int j = 0; j < mat.Length; j++)
            {
                mat[j] = dimension.mat;
            }
            dimMaterial.Add(mat);
        }
    }

    public void switchTo(int dim)
    {
        if (dimension != null)
        {
            for (int i = 0; i < render.Length; i++)//set render settings
            {
                if (dimension.index != dim)
                {
                    //Debug.Log(i + ", " + render.Length + ", " + dimMaterial.Count);
                    render[i].materials = dimMaterial[i];
                    render[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                else
                {
                    render[i].materials = myMaterial[i];
                    render[i].shadowCastingMode = castShadow[i];
                }
            }

            for (int i = 0; i < children.Length; i++)//set layer of chidren
            {
                if (dimension.index != dim)
                {
                    children[i].gameObject.layer = 9; //passthrough
                    gameObject.layer = 9;
                }
                else
                {
                    children[i].gameObject.layer = myLayer[i];
                    gameObject.layer = layer;
                }
            }
        }
        else Debug.LogWarning(name + " tried to switch dimension, but its own dimension is null");
    }
}
//TODO: support multiple materials per mesh
//also change layer of all objects, not just one