using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Queen : Piece
{

	public int speed = 10;
	public int tempSortOrder;
	public override int color {get; set;}
	public override GameObject obj {get; set;}
	public override bool moved {get; set;} = false;
	public override Vector2 destination {get; set;} = -Vector2.one;
	

	public Queen(int setColor, GameObject setObj){
		color = setColor;
		obj = setObj;
	}

	public override List<Vector2> getPlacements(Piece[,] board, int col, int row, bool lookAhead = true){

		Rook rook = new Rook(color, null); Bishop bishop = new Bishop(color, null); // dummy instances so we can call their methods
		List<Vector2> bishopPlacements = bishop.getPlacements(board, col, row, false);
		List<Vector2> res = rook.getPlacements(board, col,row, false);
		res.AddRange(bishopPlacements);

		if (lookAhead){
			res = Game.filterSuicide(res, board, col, row, color);
		}
		
		return res;
	}


}




