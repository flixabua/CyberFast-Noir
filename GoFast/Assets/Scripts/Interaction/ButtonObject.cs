
/*
* written by Jonas Hack
* 
* template for anything that can be triggered using a butten. a door for example
*/ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ButtonObject : MonoBehaviour
{
   [HideInInspector] public bool isTriggered = false;

   public abstract void trigger();

   /*public virtual void trigger()
   {

       if (isTriggered) isTriggered = false;
       else isTriggered = true;

       Debug.Log(this.name + "-buttonObject was triggered by its button. Its triggerState is now " + isTriggered);

       //also do stuff please
   }*/

protected void change()
    {
        if (isTriggered) isTriggered = false;
        else isTriggered = true;
    }
}
