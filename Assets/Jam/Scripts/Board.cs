using System;
using System.Linq;
using System.Collections.Generic;

namespace Jam {
	public struct Point {
		public int X;
		public int Y;

		public static readonly Point Zero = new Point();
		public static readonly Point One = new Point(1, 1);
		public static readonly Point Up = new Point(0, 1);
		public static readonly Point Right = new Point(1, 0);
		public static readonly Point Down = new Point(0, -1);
		public static readonly Point Left = new Point(-1, 0);

		public static readonly Point[] Neigbors = new Point[] { Up, Right, Down, Left };
		public static readonly Point[] NeighborsFull = new Point[] {
			Up, Up + Right, Right, Right + Down, Down, Down + Left, Left, Left + Up
		};

		public Point(int x = 0, int y = 0) {
			X = x;
			Y = y;
		}

		public static Point operator+(Point first, Point second) {
			return new Point(first.X + second.X, first.Y + second.Y);
		}

		public static Point operator-(Point first, Point second) {
			return new Point(first.X - second.X, first.Y - second.Y);
		}
	}
	public class Board {
		public Point Size { get; }
		private Pawn[,] _pawns;

		public Board(Point size) {
			Size = size;
			_pawns = new Pawn[size.Y, size.X];
		}

		public bool Within(Point point) {
			return point.X < Size.X && point.Y < Size.Y;
		}

		public Pawn At(Point point) {
			return Within(point) ? _pawns[point.Y, point.X] : null;
		}

		public void Set(Pawn pawn, Point at) {
			pawn.Put(this, at);
			_pawns[at.Y, at.X] = pawn;
		}

		public void Remove(Pawn pawn) {
			_pawns[pawn.Position.Y, pawn.Position.X] = null;
		}

		public void Next() {
			for (int x = 0; x < Size.X; x++) {
				for (int y = 0; y < Size.Y; y++) {
					_pawns[y, x].Next();
				}
			}
		}

		public void Back() {
			for (int x = 0; x < Size.X; x++) {
				for (int y = 0; y < Size.Y; y++) {
					_pawns[y, x].Back();
				}
			}
		}
	}

	public abstract class PawnComponent : ICloneable {
		protected static int _idCounter = 0;
		public abstract int ComponentID { get; }
		public abstract PawnComponent Copy();
		public virtual void OnAdd(Pawn pawn) { }
		public virtual void Next() { }
		public virtual void Back() { }
		object ICloneable.Clone() {
			return Copy();
		}
	}

	public abstract class PawnComponent<T> : PawnComponent where T : PawnComponent {
		public static int ID { get { return _componentID; } }
		public override int ComponentID { get { return _componentID; } }
		private static int _componentID = ++_idCounter;
		public abstract T Clone();
		public override PawnComponent Copy() { return Clone(); }
	}

	public class Pawn : ICloneable {
		public Board Board { get { return _board; } }
		public Point Position { get { return _position; } }
		private Board _board;
		private Point _position;
		private Dictionary<int, List<PawnComponent>> _components = new Dictionary<int, List<PawnComponent>>();
		private List<PawnComponent> _componentOrder = new List<PawnComponent>();

		public Pawn Clone() {
			Pawn copy = new Pawn() {
				_board = _board,
				_position = _position
			};

			for (int i = 0; i < _componentOrder.Count; i++) {
				PawnComponent component = _componentOrder[i].Copy();
				
			}

			return copy;
		}
		object ICloneable.Clone() { return Clone(); }

		public void Put(Board board, Point at) {
			_board = board;
			_position = at;
		}
		public PawnComponent AddComponent(PawnComponent component) {
			List<PawnComponent> components;
			int id = component.ComponentID;
			if (!_components.TryGetValue(id, out components)) {
				components = new List<PawnComponent>();
				_components[id] = components;
			}
			if (!components.Contains(component)) {
				components.Add(component);
				_componentOrder.Add(component);
			}
			component.OnAdd(this);
			return component;
		}
		public IEnumerable<PawnComponent> GetComponents() {
			return _componentOrder;
		}
		public T AddComponent<T>(T component) where T : PawnComponent<T> {
			return (T)AddComponent(component);
		}
		public IEnumerable<PawnComponent> GetComponents(int componentID) {
			List<PawnComponent> components;
			if (_components.TryGetValue(componentID, out components)) {
				return components;
			}
			return Enumerable.Empty<PawnComponent>();
		}
		public PawnComponent GetComponent(int componentID) {
			List<PawnComponent> components;
			if (_components.TryGetValue(componentID, out components)) {
				return components.FirstOrDefault();
			}
			return null;
		}
		public IEnumerable<T> GetComponents<T>() where T : PawnComponent<T> {
			List<PawnComponent> components;
			int id = PawnComponent<T>.ID;
			if (_components.TryGetValue(id, out components)) {
				for (int i = 0; i < components.Count; i++) {
					yield return (T)components[i];
				}
			}
		}
		public T GetComponent<T>() where T : PawnComponent<T> {
			List<PawnComponent> components;
			int id = PawnComponent<T>.ID;
			if (_components.TryGetValue(id, out components)) {
				return (T)components.FirstOrDefault();
			}
			return null;
		}
		public T RequireComponent<T>() where T : PawnComponent<T> {
			T component = GetComponent<T>();
			if (component == null) { throw new NullReferenceException("This Pawn does not contain a " + typeof(T)); }
			return component;
		}
		public void Next() {
			for (int i = 0; i < _componentOrder.Count; i++) {
				_componentOrder[i].Next();
			}
		}
		public void Back() {
			for (int i = 0; i < _componentOrder.Count; i++) {
				_componentOrder[i].Back();
			}
		}
	}
}