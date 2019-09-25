using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Listening for left mouse click
When clicked, converts the pixel position to on board plane, and sends that location to Game
Game converts that to a board square and sets its member var selection to that square

*/

public class CPM : MonoBehaviour
{
	// CPM : Command Manager


	//public clickMask;


	// Update is called once per frame
	void Update(){
      board_click();
		keyboard_commands();
      
 	}

   public static void keyboard_commands(){
      if (Input.GetKey("c")){
         if (Input.GetKeyDown(KeyCode.Space) && !Game.moveInProcess)
         {
            Game.reset_board();
         }
      }

      if (Input.GetKeyDown(KeyCode.Escape)){
         Application.Quit();
      }

   }
   

   public static void board_click(){
      if (Input.GetMouseButtonDown(0))
      {
         Vector3 point = mouseRelativeLocation();
         Game.ClickToBoardSquare(point);
      }
   }


   public static Vector3 mouseRelativeLocation(){
      Plane myPlane = new Plane(Vector3.back, 0f);
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      float distToPlane;
      if (myPlane.Raycast(ray, out distToPlane))
      {
         // A proper click was made on the board
         return ray.GetPoint(distToPlane);
      }
      return new Vector3(-1,-1,-1); // dummy click 
   }
}


