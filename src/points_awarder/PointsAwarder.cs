using Godot;
using System.Collections.Generic;
using ZombieHoardGame.PlayerCharacter;


namespace ZombieHoardGame
{
    public class PointsAwarder : Node
    {
        [Export]
        private int _pointsZombieKill = 60;
        [Export]
        private int _pointsZombieKillOutsidePlayerArea = 20;
        [Export]
        private int _pointsZombieKillByCritical = 100;
        [Export]
        private int _pointsBoardBuilt = 10;

        public bool DoublePoints { set; get; } = false;

        private Dictionary<Player, int> _totalPointsGainedByPlayer = new Dictionary<Player, int>();

        public void PlayerKilledZombie(Player player, bool criticalKill, bool insidePlayerArea)
        {
            int pointsAward = 0;
            if(criticalKill)
            {
                pointsAward = _pointsZombieKillByCritical;
            }
            else
            {
                pointsAward = _pointsZombieKill;
            }
            
            if(!insidePlayerArea)
            {
                pointsAward += _pointsZombieKillOutsidePlayerArea;
            }

            AwardPoints(player, pointsAward);
        }

        public void PlayerBuiltBoard(Player player)
        {
            AwardPoints(player, _pointsBoardBuilt);
        }

        public int TotalPointsGained(Player player)
        {
            if (_totalPointsGainedByPlayer.ContainsKey(player))
            {
                return _totalPointsGainedByPlayer[player];
            }
            return 0;
        }

        private void AwardPoints(Player player, int points)
        {
            if (DoublePoints)
            {
                points *= 2;
            }
            player.IncrementPoints(points);

            if(_totalPointsGainedByPlayer.ContainsKey(player))
            {
                _totalPointsGainedByPlayer[player] += points;
            }
            else
            {
                _totalPointsGainedByPlayer[player] = points;
            }

        }
    }
}

