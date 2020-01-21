/*
 * written by Jonas Hack
 * 
 * loads prefabs based on a tilemap (texture)
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D tex;

    // Start is called before the first frame update
    [SerializeField] private Color[] colors;
    [SerializeField] private GameObject[] tiles;

    //TODO: scale, time till yield, color palette object 



    void Start()
    {
        if (colors.Length != tiles.Length) Debug.LogError("Colors and Tiles do not match");

        StartCoroutine(generate());
    }

    [ExecuteInEditMode]
    public IEnumerator generate()
    {
        // Debug.Log("generate");

        for (int i = transform.childCount-1; i > 1; i--)//reset
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
       
        //TODO: apply scale

        for (int i = 0; i < tex.height; i++)//for every pixel in the tilema´p
        {
            for (int j = 0; j < tex.width; j++)
            {

                bool success = false;
                for (int c = 0; c < colors.Length; c++) // search for corresponding tile
                {
                   // Debug.Log("Check for color " + colors[c] + " Found color " + tex.GetPixel(i,j));
                    if (tex.GetPixel(i, j).Equals(colors[c]))
                    {
                        //Debug.Log("Found Color");
                        if(tiles[c] != null)//dont try to instatiate air
                        Instantiate(tiles[c], new Vector3(transform.position.x + i, transform.position.y, +transform.position.z + j), Quaternion.identity, transform);//create tile
                        success = true;
                    }
                }
                if(!success)
                {
                    Debug.LogWarning(this.name + " could not find a GameObject for" + tex.name + " at Pixel " + i + " " + j + ". Check your colors and texture importsettings");
                }
            }
        }

        //transform.Rotate(transform.rotation.eulerAngles);

        yield return 0;
    }

    private void OnDrawGizmos()
    {
        if(tex.height < 100 && tex.width < 100)//big tilemaps would otherwise cras unity
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < tex.height; i++)
            {
                for (int j = 0; j < tex.width; j++)
                {
                    Gizmos.DrawWireCube(new Vector3(transform.position.x + i, transform.position.y, +transform.position.z + j), Vector3.one);
                }
            }
        }
        else
        {
            //Gizmos.DrawIcon(transform.position, "standart");
        } 
    }
}
