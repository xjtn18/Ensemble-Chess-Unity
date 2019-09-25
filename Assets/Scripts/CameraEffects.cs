using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraEffects : MonoBehaviour
{
   Camera cam;
    public double xcounter = 0, ycounter = 0, zcounter = 0;
    public double xshift_counter = 0, yshift_counter = 0, zshift_counter = 0;
   private float distance = 6.5f;
   private float idle_FOV = 72.0f;
   private float select_FOV = 69.5f;
   private float velocity = 0.0f;
   private float smoothZoom = 0.6f;
   public static float step = 0.0f;


   void Start(){
      cam = GetComponent<Camera>();
   }


   void Update(){
      //rotate();
      //shift();
      zoom();

      
	   //Quaternion target = Quaternion.Euler(vec.x, vec.y, vec.z);
	   //transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
	}


	private void rotate(){
		float xshift = 0, yshift = 0, zshift = 0;
		
		xshift = (float)System.Math.Sin(xcounter) / 2;
		yshift = (float)System.Math.Cos(ycounter + 5) / 2;
		zshift = (float)System.Math.Sin(zcounter + 10) / 2;

		xcounter += 0.03; // controls duration of animation
		ycounter += 0.02;
		zcounter += 0.04;
		
		transform.rotation = Quaternion.Euler(
			xshift * 0.5f - 0.15f * Time.deltaTime,
			yshift * 0.4f * Time.deltaTime,
			zshift * 0.3f * Time.deltaTime);

	}


	private void shift(){
		float xshift = 0, yshift = 0, zshift = 0;
		
		xshift = (float)System.Math.Sin(xshift_counter) / 2;
		yshift = (float)System.Math.Cos(yshift_counter + 5) / 2;
		zshift = (float)System.Math.Sin(zshift_counter + 10) / 2;

		xshift_counter += 0.01/2; // controls duration of animation
		yshift_counter += 0.02/2;
		zshift_counter += 0.03/2;

		transform.position = new Vector3(
			xshift * 0.03f + 4 - 0.15f * Time.deltaTime,
			yshift * 0.05f + 4 * Time.deltaTime,
			zshift * 0.06f - distance * Time.deltaTime);
	}



   public void zoom(){
      float end;


      if (Game.isSelection()){
         end = select_FOV;
         // look at target
         //Vector2 selection = Game.selection;
         //Transform target = Game.mainboard[(int)selection.x, (int)selection.y].obj.transform;
         //lookTowards(new Vector3(4, 4, -6.5f), target.position);
      } else {
         end = idle_FOV;
         //step = 0.0f;
         // look away from target
         //lookTowards(transform.position, new Vector3(4,4,-6.5f));
      }

      float distance = Mathf.Abs(cam.fieldOfView - end);
      if (distance == 0){
         return;
      }
      cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, end, ref velocity, smoothZoom);
   }


   public void lookTowards(Vector3 start, Vector3 end){

      Vector3 targetDir = end - start;
      Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step/50, 0.0f);
      transform.rotation = Quaternion.LookRotation(newDir);
      if (step >= 1.0f){
         return;
      }
      step += 0.02f;
   }


	public float plus_minus(){
		return (float) System.Math.Round(Random.value) * 2 - 1;
	}

}


