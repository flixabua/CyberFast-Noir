/*
 * written by Jonas hack
 *
 * manages all dimensions and triggers the shifting
 * 
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DSwitcher : MonoBehaviour
{

    //Currently does not get reset when respwning. should it?

    //TODO: change colorgrading depending on dimension

    private List<DSwitchable> switchables = new List<DSwitchable>();
    public  Dimension[] dimensions = new Dimension[0];//TODO: make sure they are in the right places
    [HideInInspector] public int[] dimensionVolumeIndex = new int[0];
    int currentDimension = 0;

    public string button = "Switch";

    private void Update()
    {
        if (Input.GetButtonDown(button))
        {
            currentDimension++;
            if (currentDimension >= dimensions.Length) currentDimension = 0;
            switchDimension(currentDimension);
        }
    }

    private void Start()
    {
        switchables = GameObject.FindObjectsOfType<DSwitchable>().ToList<DSwitchable>();

        if (dimensions.Length == 0) Debug.LogError("There are no dimensions to shift to");

        dimensionVolumeIndex = new int[dimensions.Length];
        for (int i = 0; i < dimensionVolumeIndex.Length; i++)
        {
            Effect effect = new Effect();
            effect.ppProfile = dimensions[i].postProcessingVolume;
            effect.fogColor = dimensions[i].fogColor;
            dimensionVolumeIndex[i] = CameraEffectsMaster.addEffect(effect);
        }

        switchDimension(0);
    }

    public static Dimension register(DSwitchable switchObject)
    {
        DSwitcher switcher = GameObject.FindObjectOfType<DSwitcher>();//maybe use singelton instead?

        if (switcher != null)
        {
            switcher.switchables.Add(switchObject);
            int i = switchObject.dimensionIndex;
            if (i >= 0 && i < switcher.dimensions.Length) return switcher.dimensions[i];
            else return null;
        }
        else
        {
            Debug.Log(switchObject.name + " could not find a switcher to register to");
            return null;
        }
    }

    public void switchDimension(int dimension)
    {
        //Debug.Log("Switching to " + dimension);

        CameraEffectsMaster.fovPunch(5, 0.25f);

        for (int i = 0; i < dimensions.Length; i++)
        {
            if (i == dimension) CameraEffectsMaster.setVolumeweight(dimensionVolumeIndex[i], 1);
            else CameraEffectsMaster.setVolumeweight(dimensionVolumeIndex[i], 0);
        }

        if (dimension >= 0 && dimension  < dimensions.Length)
        {
            foreach (DSwitchable switchO in switchables)
            {
                switchO.switchTo(dimension);
            }
        }
        else
        {
            Debug.LogWarning("tried to switch to an invalid dimesion");
        }
    }
}
