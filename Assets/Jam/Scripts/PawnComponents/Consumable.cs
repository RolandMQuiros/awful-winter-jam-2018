using System;

namespace Jam.Component {
    public class Consumable : PawnComponent<Consumable> {
        public int Health { get { return _health; } }
        public Tag Tag;
        public event Action<int> Damaged;
        public event Action<int> Healed;
        public event Action Died;
        private int _health = 0;

        public Consumable(int health) {
            _health = health;
        }

        public void Affect(int healthChange) {
            _health += healthChange;
            if (healthChange < 0 && Damaged != null)     { Damaged(-healthChange); }
            else if (healthChange > 0 && Healed != null) { Healed(healthChange); }
            if (_health <= 0 && Died != null) { Died(); }
        }

        public override Consumable Clone() {
            return new Consumable(_health);
        }
    }
}