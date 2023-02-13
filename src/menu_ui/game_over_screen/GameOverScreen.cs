using Godot;
using ZombieHoardGame;


namespace UI
{
    public class GameOverScreen : Control
    {
        [Signal]
        public delegate void QuitRequested();
        [Signal]
        public delegate void RestartRequested();


        private AnimationPlayer _animPlayer;

        private Label _labelRounds;
        private Label _labelTime;
        private Label _labelPoints;
        private Label _labelKilled;
        private Label _labelHeadshot;
        private Label _labelPowerups;
        private Label _labelHighRound;


        public override void _Ready()
        {
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _labelRounds = (Label)FindNode("ValueRounds");
            _labelTime = (Label)FindNode("ValueTime");
            _labelPoints = (Label)FindNode("ValuePoints");
            _labelKilled = (Label)FindNode("ValueKilled");
            _labelHeadshot = (Label)FindNode("ValueHeadshot");
            _labelPowerups = (Label)FindNode("ValuePowerup");
            _labelHighRound = (Label)FindNode("LabelHighRound");
            _labelHighRound.RectPivotOffset = _labelHighRound.RectSize * 0.5f;
        }
        

        public async void Popup(int zombiesKilled, int zombiesKilledHeadshot)
        {
            LevelServices services = LevelServices.Instance;
            int roundsFinished = services.RoundCounter.RoundsStarted - 1;
            _labelRounds.Text = (roundsFinished).ToString();
            _labelTime.Text = services.Stopwatch.ElapsedTimeFormattedString();
            _labelPoints.Text = services.PointsAwarder.TotalPointsGained(services.Player).ToString();
            _labelKilled.Text = zombiesKilled.ToString();
            _labelHeadshot.Text = zombiesKilledHeadshot.ToString();
            _labelPowerups.Text = services.PowerUpSpawner.TotalPowerupsCollected.ToString();

            Input.MouseMode = Input.MouseModeEnum.Confined;
            _animPlayer.Play("popup");
            _labelHighRound.Hide();
            GetTree().Paused = true;
            await ToSignal(_animPlayer, "animation_finished");

            SavedData savedData = GetNode<SavedData>("/root/SavedData");
            int currentBestRound = savedData.LevelHighestRound(Owner.Name);
            if (currentBestRound < roundsFinished)
            {
                savedData.UpdateLevelHighestRound(Owner.Name, roundsFinished);
                _labelHighRound.Show();
                _animPlayer.Play("new_highest_round");
            }
        }


        private void OnBtnQuitPressed()
        {
            GetTree().Paused = false;
            EmitSignal(nameof(QuitRequested));
        }
        
        private void OnBtnRestartPressed()
        {
            GetTree().Paused = false;
            EmitSignal(nameof(RestartRequested));
        }
    }
}

