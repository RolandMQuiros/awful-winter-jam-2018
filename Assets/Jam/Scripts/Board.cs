using System;
using System.Collections.Generic;

namespace Jam {
	public class Board {
		public int Width { get { return _pawns.GetLength(0); } }
		public int Height { get { return _pawns.GetLength(1); } }
		private Pawn[,] _pawns;

		public Board(int width, int height) {
			_pawns = new Pawn[width, height];
		}
	}

	public abstract class PawnComponent<T> {
		public int ComponentID { get { return _id; } }
		private static int _id = -1;
		private static int _idCounter = 0;
		public PawnComponent() {
			if (_id < 0) { _id = _idCounter++; }
		}
	}

	public abstract class PawnComponent : PawnComponent {
		public abstract void Next();
		public abstract void Back();
	}

	public class Pawn {
		private Board _board;
		private int _x;
		private int _y;

		private Dictionary<int, HashSet<PawnComponent>> _components = new Dictionary<int, HashSet<PawnComponent>>();

		public void AddToBoard(Board board, int x, int y) {
			_board = board;
			_x = x;
			_y = y;
		}
	}
}