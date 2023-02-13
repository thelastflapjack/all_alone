using Godot;
using System.Collections.Generic;

// Basic vesion which just plays a looped animation. Add more advanced features later
namespace GameGeneral
{
    public class MusicSystem : Node
    {
        // public enum TrackName{
        //     MainMenu, Atmospheric
        // }

        // private Dictionary<TrackName, AudioStream> _trackStreams = new Dictionary<TrackName, AudioStream>();
        // private List<AudioStreamPlayer> _streamPlayers = new List<AudioStreamPlayer>();
        // private int _activePlayerIdx = -1;

        public override void _Ready()
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("music_loop");
        }
        
        // public void TransitionToTrack(TrackName newTrack, float fadeTime = 1.5f)
        // {
        //     AudioStreamPlayer lastPlayer = null;
        //     AudioStreamPlayer nextPlayer;
            
        //     if (_activePlayerIdx == 0)
        //     {
        //         lastPlayer = _streamPlayers[0];
        //         nextPlayer = _streamPlayers[1];
        //     }
        //     else if (_activePlayerIdx == 1)
        //     {
        //         lastPlayer = _streamPlayers[1];
        //         nextPlayer = _streamPlayers[0];
        //     }
        //     else
        //     {
        //         nextPlayer = _streamPlayers[0];
        //     }

        //     if (lastPlayer != null)
        //     {
        //         StreamPlayerFade(lastPlayer, false, fadeTime);
        //     }
        //     StreamPlayerFade(nextPlayer, false, fadeTime);
        // }

        // public void StopPlaying(float fadeTime = 1.5f)
        // {
        //     if (_activePlayerIdx > 0)
        //     {
        //         StreamPlayerFade(_streamPlayers[_activePlayerIdx], false, fadeTime);
        //         _activePlayerIdx = -1;
        //     }
        // }

        // private void StreamPlayerFade(AudioStreamPlayer player, bool fadeIn, float fadeTime)
        // {
        //     SceneTreeTween fadeTween = GetTree().CreateTween();
        //     if (fadeIn)
        //     {
        //         fadeTween.TweenCallback(player, "play");
        //         fadeTween.TweenProperty(player, "volume_db", 0.0f, fadeTime).From(-20.0f).SetEase(Tween.EaseType.In);
        //     }
        //     else
        //     {
        //         fadeTween.TweenProperty(player, "volume_db", -20.0f, fadeTime).From(0.0f).SetEase(Tween.EaseType.Out);
        //         fadeTween.TweenCallback(player, "stop");
        //     }
        //     fadeTween.Play();
        // }
    }
}

