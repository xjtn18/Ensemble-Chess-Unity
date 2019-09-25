using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Bishop : Piece
{

	public int speed = 10;
	public int tempSortOrder;
	public override int color {get; set;}
	public override GameObject obj {get; set;}
	public override bool moved {get; set;} = false;
	public override Vector2 destination {get; set;} = -Vector2.one;

	public Bishop(int setColor, GameObject setObj){
		color = setColor;
		obj = setObj;
	}


	public override List<Vector2> getPlacements(Piece[,] board, int col, int row, bool lookAhead = true){

		List<Vector2> res = new List<Vector2>();

		Vector2[] direc = new Vector2[4]{
			new Vector2(-1,-1), new Vector2(-1,1),
			new Vector2(1,1), new Vector2(1,-1)};

		foreach (Vector2 d in direc){
			int ncol = col + (int)d.x;
			int nrow = row + (int)d.y;

			while (true){
				if (Game.inRange(ncol, nrow)){
					Piece piece = board[ncol,nrow];
					if (piece != null && piece.color == color){
						break;
					} else if (piece != null){
						res.Add(new Vector2(ncol, nrow));
						break;
					}
					res.Add(new Vector2(ncol, nrow));
					ncol += (int)d.x;
					nrow += (int)d.y;
				} else {
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




