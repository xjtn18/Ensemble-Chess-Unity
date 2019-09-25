using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece
{
   public abstract bool moved { get; set; }
   public abstract GameObject obj { get; set; }
   public abstract int color { get; set; }
   public abstract List<Vector2> getPlacements(Piece[,] board, int x, int y, bool lookAhead = true);
   public abstract Vector2 destination { get; set; }
   public virtual int en_passant { get; set; } // virtual so that only the Pawn needs to implement it
}