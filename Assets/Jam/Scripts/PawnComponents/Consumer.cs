using System.Collections.Generic;
using Jam;

namespace Jam.Component {
    public class Consumer : PawnComponent<Consumer> {
        private Pawn _pawn;
        private Dictionary<Tag, int> _tagBias = new Dictionary<Tag, int>();

        public Consumer(IDictionary<Tag, int> bias) {
            _tagBias = new Dictionary<Tag, int>(bias);
        }

        #region PawnComponent
        public override Consumer Clone() {
            return new Consumer(_tagBias);
        }

        public override void OnAdd(Pawn pawn) {
            _pawn = pawn;
        }

        public override void Next() {
            // Check adjacent cells
            for (int n = 0; n < Point.NeighborsFull.Length; n++) {
                Point offset = Point.NeighborsFull[n] + _pawn.Position;
                Pawn neighbor = _pawn.Board.At(offset);
                ConsumeNeighbor(neighbor);
            }
        }

        public override void Back() {
            
        }
        #endregion

        #region Private
        private void ConsumeNeighbor(Pawn pawn) {
            // Check if neighboring pawn is consumable
            Consumable consumable = pawn.GetComponent<Consumable>();
            int bias = 0;
            _tagBias.TryGetValue(consumable.Tag, out bias);
            consumable.Affect(bias);
        }
        #endregion
    }
}