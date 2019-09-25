using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*

Game class stores all the current game data
Creates a 2d array of piece objects, which each store a prefab GameObject which contains their sprite image.

THINGS TO ADD:

   Thursday 9/12
      - when game ends, shift camera to the right and fade in a game over UI (make sure board is still visible)
      - work with new songs and sounds
*/



public class Game : MonoBehaviour
{
	public const int COL = 8;
	public const int ROW = 8;

	public const int white = 0;
	public const int black = 1;
	public static int BOTTOM_PLAYER = 0;
	public static int TOP_PLAYER = 1;

	public static int current_player = white;
	public static int player_in_check = -1;
	public static int winner = -1;

	public static Vector2 selection = -Vector2.one; // the currently selected piece (not where a piece is moving to)
	public static List<Piece> movingPieces = new List<Piece>();
	public static Vector2 blackKingPos = new Vector2(4,7);
	public static Vector2 whiteKingPos = new Vector2(4,0);
	public static bool en_passant_active = false;
	public static bool moveInProcess = false;

	public static List<Vector2> selection_placements = new List<Vector2>();

	public static Piece[,] mainboard;

   private static Piece WKi(int col, int row) { return new King(white, CreateObject("king_white", col, row)); }
   private static Piece WQ(int col, int row) { return new Queen(white, CreateObject("queen_white", col, row)); }
   private static Piece WP(int col, int row) { return new Pawn(white, CreateObject("pawn_white", col, row)); }
   private static Piece WKn(int col, int row) { return new Knight(white, CreateObject("knight_white", col, row)); }
   private static Piece WR(int col, int row) { return new Rook(white, CreateObject("rook_white", col, row)); }
   private static Piece WB(int col, int row) { return new Bishop(white, CreateObject("bishop_white", col, row)); }

   private static Piece BKi(int col, int row) { return new King(black, CreateObject("king_black", col, row)); }
   private static Piece BQ(int col, int row) { return new Queen(black, CreateObject("queen_black", col, row)); }
   private static Piece BP(int col, int row) { return new Pawn(black, CreateObject("pawn_black", col, row)); }
   private static Piece BKn(int col, int row) { return new Knight(black, CreateObject("knight_black", col, row)); }
   private static Piece BR(int col, int row) { return new Rook(black, CreateObject("rook_black", col, row)); }
   private static Piece BB(int col, int row) { return new Bishop(black, CreateObject("bishop_black", col, row)); }


   public static GameObject CreateObject(string prefabName, int col, int row)
   {
      GameObject res = Instantiate(Resources.Load(prefabName),
                     new Vector3(col + 0.5f, row + 0.5f, 0),
                     new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
      return res;
   }




	void Start(){ // called when game runs
      freshBoard();
	}

   public static void freshBoard(){
      mainboard = new Piece[8, 8]
      {
         {WR(0,0), WP(0,1), null, null, null, null, BP(0,6), BR(0,7)},
         {WKn(1,0), WP(1,1), null, null, null, null, BP(1,6), BKn(1,7)},
         {WB(2,0),WP(2,1), null, null, null, null, BP(2,6), BB(2,7)},
         {WQ(3,0),WP(3,1), null, null, null, null, BP(3,6), BQ(3,7)},
         {WKi(4,0),WP(4,1), null, null, null, null, BP(4,6), BKi(4,7)},
         {WB(5,0), WP(5,1), null, null, null, null, BP(5,6), BB(5,7)},
         {WKn(6,0), WP(6,1), null, null, null, null, BP(6,6), BKn(6,7)},
         {WR(7,0), WP(7,1), null, null, null, null, BP(7,6), BR(7,7)}
      };
   }
   

	public static bool inRange(int col, int row){
		return (col >= 0 && col <= COL - 1) && (row >= 0 && row <= ROW - 1);
	}




	public static void ClickToBoardSquare(Vector3 point){
		// takes a world coordinate on the board plane and converts into a square coord on the game board
		if (!check_bounds(point) || moveInProcess){
			resetSelection();
			return;
		};

		int col = (int)selection.x;
		int row = (int)selection.y;
		int dcol = (int)Math.Floor(point.x);
		int drow = (int)Math.Floor(point.y);
		Vector2 newPoint = new Vector2(dcol, drow);
		Piece piece = mainboard[dcol,drow];

		// click analysis
		if (isSelection()){ // a selection has already been made
			Piece selected = mainboard[col,row];
			if ((dcol == col && drow == row)){ // played selected an already selected piece
				resetSelection();
			} else if (piece != null && piece.color == current_player){
				makeSelection(point);
			} else {
				List<Vector2> valid_placements = selected.getPlacements(mainboard, col, row);
				if (valid_placements.Contains(newPoint)){ /// A MOVE WAS MADE
					moveInProcess = true;
					gameMove(col, row, dcol, drow);
				} else {
					resetSelection();
				}
			}


		} else { // a selection has not yet been made
			if (piece != null && piece.color == current_player){
				makeSelection(point);
			}
		}
      CameraEffects.step = 0.0f;
		Physx.clearPlacements();

	}

	public static void makeSelection(Vector3 point){
		int col = (int)point.x, row = (int)point.y;
		selection = point;
		selection_placements = mainboard[col,row].getPlacements(mainboard, col, row);
	}

	public static bool check_bounds(Vector3 point){
		// bounds checking
		return point.x > 0 &&
						point.x < 8 &&
						point.y > 0 &&
						point.y < 8;
	}



	public static void gameMove(int col, int row, int dcol, int drow){
		// Alters mainboard by moving the moved piece
		Piece taken = mainboard[dcol,drow];
		Piece taker = mainboard[col,row];

		taker.moved = true;
		taker.destination = new Vector2(dcol, drow); // this is used to flag the physx to display the transition animation
		movingPieces.Add(taker);
		if (taken != null){ Destroy(taken.obj);} //removes obj of taken piece (doesnt free automatically)


		// Change board
		mainboard[dcol,drow] = mainboard[col,row];
		mainboard[col,row] = null;

		updateKingPositions(taker, dcol, drow);

		check_en_passant(taken, dcol, drow);
		checkCastle(dcol, drow);
		checkGameOver();


		current_player = opp(current_player);
		resetSelection();
	}



	public static void check_en_passant(Piece captured, int dcol, int drow){
		int scol = (int)selection.x;
		int srow = (int)selection.y;
		Piece moved_piece = mainboard[dcol,drow];

		if (en_passant_active){
			en_passant_active = false;
			clear_en_passant();
		}

		//Piece temp;
		if (moved_piece is Pawn){
			if (System.Math.Abs(drow-srow) == 2){
				en_passant_active = true;
				if (dcol < 7 && mainboard[dcol+1, drow] is Pawn
					&& mainboard[dcol+1, drow].color != current_player){
					mainboard[dcol + 1,drow].en_passant = 1;
				}
				if (dcol > 0 && mainboard[dcol-1, drow] is Pawn
					&& mainboard[dcol-1, drow].color != current_player){
					mainboard[dcol - 1,drow].en_passant = -1;
				}

			} else if (scol != dcol && captured == null){ // an en passant move was made ***

				Destroy(mainboard[dcol,drow - (drow-srow)].obj);
				mainboard[dcol,drow - (drow-srow)] = null;
			}
		}
	}



	public static void clear_en_passant(){
		foreach (Piece piece in mainboard){
			if (piece is Pawn && piece.en_passant != 0){
				piece.en_passant = 0;
			}
		}
	}



	public static void checkCastle(int dcol, int drow){
		int scol = (int)selection.x;
		Piece moved_piece = mainboard[dcol,drow];

		int rcol, nrcol;
		if (moved_piece is King){
			int travel = scol - dcol;
			if (System.Math.Abs(travel) > 1){
				if (travel < 0){
					rcol = 7;
					nrcol = 5;
				} else {
					rcol = 0;
					nrcol = 3;
				}

				gameMove(rcol, drow, nrcol, drow);

			}
		}
		
	}


	public static void checkGameOver(){
		int opponent = opp(current_player);
		if (inCheck(mainboard, opponent)){
			player_in_check = opponent;
			if (mate(opponent)){
				gameOver(current_player);
			}
		}
	}


	public static bool mate(int color){
		for (int i = 0; i < 8; ++i){
			for (int j = 0; j < 8; ++j){
				Piece piece = mainboard[i,j];
				if (piece != null && piece.color == color){
					if (piece.getPlacements(mainboard, i, j).Count != 0){
						return false;
					}
				}
			}
		}
		return true;
	}

	public static void gameOver(int player){
		winner = player;
	}





	// HELPERS

	public static void updateKingPositions(Piece taker, int dcol, int drow){
		if (taker is King){
			if (taker.color == black){
				blackKingPos = new Vector2(dcol,drow);
			} else {
				whiteKingPos = new Vector2(dcol,drow);
			}
		}
	}

	public static Vector2 getKingPos(int color){
		return (color == black) ? blackKingPos : whiteKingPos;
	}

	public static bool isSelection(){
		// tells us if a selection has already been made
		return selection != -Vector2.one;
	}

	public static void resetSelection(){
		selection = -Vector2.one;
		selection_placements.Clear();
	}


	public static bool notEmpty(int col, int row){
		return !(mainboard[col,row] is null);
	}

	public static int opp(int color){
		return (color + 1) % 2;
	}



	public static HashSet<Vector2> allVictims(Piece[,] board, int color){
		HashSet<Vector2> res = new HashSet<Vector2>();
		for (int i = 0; i < 8; ++i){
			for (int j = 0; j < 8; ++j){
				Piece piece = board[i,j];
				if (piece != null && piece.color != color){
					foreach (Vector2 vec in piece.getPlacements(board, i, j, false)){
						res.Add(vec);
					}
				}
			}
		}
		return res;
	}


	public static bool inCheck(Piece[,] board, int color){
		for (int i = 0; i < 8; ++i){
			for (int j = 0; j < 8; ++j){
				Piece piece = board[i,j];
				if (piece != null && piece.color != color){

					List<Vector2> plist = piece.getPlacements(board, i, j, false);
					foreach (Vector2 vec in plist){
						if (board[(int)vec.x, (int)vec.y] is King){
							return true;
						}
					}
				}
			}
		}
		return false;
	}



	public static List<Vector2> filterSuicide(List<Vector2> placements, Piece[,] board, int col, int row, int color){
		//Piece[,] boardCopy = createBoardCopy(board);
		
		Piece precessor;
		Vector2 kingPos;
		for (int i = placements.Count - 1; i >= 0; --i){
			int ncol = (int)(placements[i].x);
			int nrow = (int)(placements[i].y);

			precessor = mainboard[ncol,nrow];
			mainboard[ncol,nrow] = mainboard[col,row];
			mainboard[col,row] = null;

			if (mainboard[ncol,nrow] is King){
				kingPos = placements[i];
			} else {
				kingPos = getKingPos(color);
			}

			if (inCheck(mainboard, color)){
				placements.Remove(placements[i]);
			}


			mainboard[col,row] = mainboard[ncol,nrow];
			mainboard[ncol,nrow] = precessor;

		}

		return placements;

	}


	public static Piece[,] createBoardCopy(Piece[,] board){
		Piece[,] boardCopy = new Piece[8,8];
		for (int i = 0; i < 8; ++i){
			for (int j = 0; j < 8; ++j){
				boardCopy[i,j] = (board[i,j] != null) ? board[i,j] : null;
			}
		}
		return boardCopy;
	}


   public static void reset_board(){
      foreach (Piece piece in mainboard){
         if (piece != null){
            Destroy(piece.obj);
         }
      }
      resetSelection();
      en_passant_active = false;
      current_player = white;
      freshBoard();


   }




}




