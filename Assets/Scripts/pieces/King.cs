using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class King : Piece
{

	public int speed = 10;
	public int tempSortOrder;
	public override int color {get; set;}
	public override GameObject obj {get; set;}
	public override bool moved {get; set;} = false;
	public override Vector2 destination {get; set;} = -Vector2.one;

	public King(int setColor, GameObject setObj){
		color = setColor;
		obj = setObj;
	}




	public override List<Vector2> getPlacements(Piece[,] board, int col, int row, bool lookAhead = true){

		List<Vector2> res = new List<Vector2>();
		Queen queen = new Queen(color, null);
		res = queen.getPlacements(board, col, row, false);
		res = res.Where(spot => Vector2.Distance(new Vector2(col, row), new Vector2(spot.x, spot.y)) <= 1.5).ToList();

		if (lookAhead){
			// AddRange the result of self.add_castle
			res.AddRange(addCastle(board, col, row));
			res = Game.filterSuicide(res, board, col, row, color);
		}

		return res;
	}



	HashSet<Vector2> addCastle(Piece[,] board, int col, int row){
		HashSet<Vector2> res = new HashSet<Vector2>();
		Vector2 kingPos = new Vector2(col,row);
		if (!moved && !Game.allVictims(board, color).Contains(kingPos)){
			Piece rook = board[col+3, row];
			HashSet<Vector2> middle = new HashSet<Vector2>{new Vector2(col+1, row),
																			new Vector2(col+2, row)};
			if (rook != null && !rook.moved && spotsEmpty(board, middle) && !anyWatched(board, middle)){
				res.Add(new Vector2(col + 2, row));
			}

			rook = board[col-4, row];
			middle = new HashSet<Vector2>{new Vector2(col-1, row),
													new Vector2(col-2, row),
													new Vector2(col-3, row)};
			if (rook != null && !rook.moved && spotsEmpty(board, middle) && !anyWatched(board, middle)){
				res.Add(new Vector2(col - 2, row));
			}
		}
		return res;

	}


	bool spotsEmpty(Piece[,] board, HashSet<Vector2> spots){
		foreach (Vector2 vec in spots){
			if (board[(int)vec.x, (int)vec.y] != null){
				return false;
			}
		}
		return true;
	}


	bool anyWatched(Piece[,] board, HashSet<Vector2> spots){
		HashSet<Vector2> all = Game.allVictims(board, color);
		foreach (Vector2 spot in spots){
			if (all.Contains(spot)){
				return true;
			}
		}
		return false;
	}



}


