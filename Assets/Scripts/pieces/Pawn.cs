using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Pawn : Piece
{

	public int speed = 10;
	public int tempSortOrder;
	public override int color {get; set;}
	public override GameObject obj {get; set;}
	public override bool moved {get; set;} = false;
	public override int en_passant {get; set;} = 0;
	public override Vector2 destination {get; set;} = -Vector2.one;

	public Pawn(int setColor, GameObject setObj){
		color = setColor;
		obj = setObj;
	}


	public override List<Vector2> getPlacements(Piece[,] board,
																int col, int row, bool lookAhead = true){

		List<Vector2> res = new List<Vector2>();
		if (row == 0 || row == 7){
			return res;
		}

		int forward = 1;
		if (color == Game.black){
			forward = -1;
		}
		int times = 1;
		if (!moved){
			times = 2;
		}

		int nrow = row + forward;

		int dir = 1;
		for (int i = 0; i < 2; ++i){
			if ((col + dir >= 0 && col + dir <= 7) && board[col + dir, nrow] != null
														&& board[col+dir, nrow].color != color){
				res.Add(new Vector2(col+dir, nrow));
			}
			dir = -1;
		}

		for (int i = 0; i < times; ++i){
			if (nrow >= 0 && nrow <= 7){
				if (board[col, nrow] == null){
					res.Add(new Vector2(col, nrow));
					nrow = nrow + forward;
				} else {
					break;
				}
			}
		}

		// Check en passant

		if (en_passant != 0){
			if (color == Game.black){
				res.Add(new Vector2(col - en_passant, row - 1));
			} else {
				res.Add(new Vector2(col - en_passant, row + 1));
			}
		}


		if (lookAhead){
			res = Game.filterSuicide(res, board, col, row, color);
		}

		return res;
	}


}




