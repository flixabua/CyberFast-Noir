
/*
* written by Jonas Hack
* 
* standart button cf. portal etc. 
* triggers visuals and the desired event for the objects it references
* 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class Button : Interactable
{
   [SerializeField] private ButtonObject buttonObject = null;
   //[SerializeField] private SpecialPlatform buttonObject = null;
   [SerializeField] private Color gizmoColor = Color.blue;

   private Animator animator;

   private new void Start()
   {
       base.Start();

       animator = GetComponent<Animator>();
   }

   public override void trigger(GameObject actor)
   {
       buttonObject.trigger();
       animator.Play("Trigger");
       //Debug.Log(this.name + "-button was triggered by " + actor.name);
       //TODO: sound
   }

   private void OnDrawGizmos()
   {
       if(buttonObject != null)
       {
           Gizmos.color = gizmoColor;
           Gizmos.DrawLine(transform.position, buttonObject.transform.position);
       }
   }   
}
