
/*
* written by Jonas Hack
*
* template for anything the player can interact with (look at it and press e)
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
   protected void Start()
   {
       Interactor actor = GameObject.FindObjectOfType<Interactor>();
       if(actor != null)
       {
           actor.register(this);
       }
       else
       {
           Debug.LogWarning(this.name + " could not find an Interactor to register to");
       }
   }

   public abstract void trigger(GameObject actor);

}
