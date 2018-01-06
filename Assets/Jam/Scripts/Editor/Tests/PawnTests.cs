using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace Jam.Test {
	public class PawnTests {

		private class ComponentA : PawnComponent<ComponentA> {
			private int _a = 1;
			public override PawnComponent Clone() {
				return new ComponentA() { _a = _a };
			}
			public override void Next() { }
			public override void Back() { }
		}

		private class ComponentB : PawnComponent<ComponentB> {
			private int _b = 2;
			public override PawnComponent Clone() {
				return new ComponentB() { _b = _b };
			}
			public override void Next() { }
			public override void Back() { }
		}

		[Test]
		public void PawnComponentID() {
			Console.WriteLine("ComponentA.ID = " + ComponentA.ID);
			Console.WriteLine("ComponentB.ID = " + ComponentB.ID);
			Assert.NotZero(ComponentA.ID);
			Assert.NotZero(ComponentB.ID);
			Assert.AreNotEqual(ComponentA.ID, ComponentB.ID);
		}

		[Test]
		public void GetComponentFromPawn() {
			Pawn pawn = new Pawn();
			ComponentA a = pawn.AddComponent<ComponentA>(new ComponentA());
			ComponentB b = pawn.AddComponent<ComponentB>(new ComponentB());

			Assert.NotNull(pawn.GetComponent<ComponentA>());
			Assert.AreNotEqual(a, b);
		}
	}
}