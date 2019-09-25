using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMenu : MonoBehaviour
{

   public Animator animator;
   private bool stop = false;
   // Start is called before the first frame update
   void Start()
   {
      animator = GetComponent<Animator>();
   }

   // Update is called once per frame
   void Update()
   {
   if (!stop){
      animator.SetBool("game_over_trigger", Game.winner != -1);
   }
   }


   void toIdle(){
   animator.SetBool("to_idle", true);
   animator.SetBool("game_over_trigger", false);
   stop = true;
   }
}
