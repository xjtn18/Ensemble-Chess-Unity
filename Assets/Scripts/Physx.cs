using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// AKA the display class
// Holds the main display loop
// Also handles all transitions and visual aspects

public class Physx : MonoBehaviour
{

	public bool 		selectState = false;
	public GameObject selectHint;
	const int 			SPEED = 10;
	public static List<GameObject> placements = new List<GameObject>();
   
   //HINTS
   private Vector2 hoverHintScale = new Vector3(1.3f, 1.3f, 1.3f);


	void Start(){
		Application.targetFrameRate = 120;
		selectHint = Game.CreateObject("select", 100, 100);
		selectHint.SetActive(false);
	}

	// Update is called once per frame
	void Update(){ //main display update method
		showSelect();           // show when a piece has been selected
		showPieceTransition();  // show piece movements
		showHints();            // show possible placement for selected piece


		if (Input.GetKey("escape")){
			Application.Quit();
		}
	}



	void showHints(){
		if (Game.isSelection()){
			if (placements.Count == 0){
				int col = (int)Game.selection.x;
				int row = (int)Game.selection.y;

				foreach (Vector2 vec in Game.selection_placements){
					int pcol = (int)vec.x;
					int prow = (int)vec.y;
					
					GameObject newObj;
					if (Game.mainboard[pcol, prow] == null){
						newObj = Game.CreateObject("placement", pcol, prow);
                  //Vector2 boardSquare = boardPosition(CPM.mouseRelativeLocation());
                  //if (vec == boardSquare){
                     //newObj.transform.localScale *= 1.3;
                  //}

					} else {
						newObj = Game.CreateObject("attack", pcol, prow);
					}

					placements.Add(newObj);

				}
			}
		
		} else {
			clearPlacements();
		}

      //hoveringHint();

	}

    /*
   void hoveringHint(){
      foreach (GameObject hint in placements) {
         if (hint.OnMouseOver()){
            hint.transform.localScale = hoverHintScale; // increase size of hint when hovered over
         } else {
            hint.transform.localScale = Vector3.one; // reset the scale if the mouse isnt over it
         }
      }
   }
    */


   Vector2 boardPosition(Vector3 click){
      int col = (int)System.Math.Floor(click.x);
      int row = (int)System.Math.Floor(click.y);
      return new Vector2(col, row);
   }


	public static void clearPlacements(){
		for (int i = placements.Count-1; i >= 0; --i){
			Destroy(placements[i]);
		}
		placements.Clear();
	}


	void showPieceTransition(){
		for (int i = Game.movingPieces.Count-1; i >= 0; --i){ // ITERATE IN REVERSE so removing from it will not cause problems
			Piece piece = Game.movingPieces[i];
			move(piece, SPEED);
		}
	}


	void showSelect(){
		if (Game.isSelection()){ // if a selection has been made
			int col = (int)Game.selection.x;
			int row = (int)Game.selection.y;

			if (!selectState){
				selectHint.SetActive(true);
				displaySelect(col,row);

			}
			//move(Game.mainboard[x,y].obj, 10);
			//Physx.bringForward(GetComponent<SpriteRenderer>(), tempSortOrder);
		} else { // if a selection has NOT been made
			selectState = false;
			selectHint.SetActive(false);

			//Physx.bringBack(GetComponent<SpriteRenderer>(), tempSortOrder);
		}
	}


	void move(Piece piece, float speed){
		Vector3 dest = piece.destination;
		dest.x += 0.5f; // so piece displays at center of square
		dest.y += 0.5f;
		float distance = Vector3.Distance(piece.obj.transform.position, dest);
		piece.obj.transform.position = Vector3.MoveTowards(piece.obj.transform.position, dest, speed/(1/distance) * Time.deltaTime);

		if (distance < 0.01f){
			piece.obj.transform.position = dest;
			reset(piece);
            if (Game.movingPieces.Count == 0){
                Game.moveInProcess = false;
            }
		}
	}



	void reset(Piece piece){
		piece.destination = -Vector2.one;
		Game.movingPieces.Remove(piece);
		
	}

	void bringForward(SpriteRenderer sprite, ref int tempSortOrder){
		tempSortOrder = sprite.sortingOrder;
		sprite.sortingOrder = 50; // trivial high number
	}

	void bringBack(SpriteRenderer sprite, int tempSortOrder){
		sprite.sortingOrder = tempSortOrder;
	}


	void displaySelect(int col, int row){
		selectHint.transform.position = new Vector3(col + 0.5f, row + 0.5f,0);
	}



}


