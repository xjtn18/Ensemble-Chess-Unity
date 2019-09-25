using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Rook : Piece
{

	public int speed = 10;
	public int tempSortOrder;
	public override int color {get; set;}
	public override GameObject obj {get; set;}
	public override bool moved {get; set;} = false;
	public override Vector2 destination {get; set;} = -Vector2.one;
	

	public Rook(int setColor, GameObject setObj){
		color = setColor;
		obj = setObj;
	}


	public override List<Vector2> getPlacements(Piece[,] board, int col, int row, bool lookAhead = true){

		List<Vector2> res = new List<Vector2>(); // return list
		Vector2[] direc = new Vector2[4] { new Vector2(-1,0), new Vector2(0, 1), new Vector2(1,0), new Vector2(0, -1) }; // directions array
		foreach (Vector2 d in direc){
			int ncol = col + (int)d.x;
			int nrow = row + (int)d.y;
			while (true){
				if (Game.inRange(ncol, nrow)){
					Piece piece = board[ncol, nrow];
					if (Game.notEmpty(ncol, nrow) && piece != null && piece.color == color){ // if a team piece blocks the way
						break;
					} else if (Game.notEmpty(ncol, nrow)){ // if an enemy piece blocks the way
						res.Add(new Vector2(ncol, nrow));
						break;
					}
					res.Add(new Vector2(ncol, nrow)); // if no one blocks the way
					ncol = ncol + (int) d.x;
					nrow = nrow + (int) d.y;
				} else { // if we've reached the edge of the board
					break;
				}
			}
		}

		if (lookAhead){
			res = Game.filterSuicide(res, board, col, row, color);
		}
		
		return res;
	}



}




