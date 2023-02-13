using Godot;
using System;
using System.Diagnostics;

// Main scene node, program starts here.
// Manages loading of and transitions between major secnes
namespace GameGeneral
{
    public class Main : Node
    {
        /// Exported Fields ///
        [Export]
        private PackedScene _initialScene = null;

        private const float _POLL_TIME = 0.02f;
        private const String _levelDirectoryPath = "res://src/levels/";
        private const String _mainMenuFilePath = "res://src/menu_ui/main_menu/MainMenu.tscn";


        /// Fields - protected or private ///
        private ResourceInteractiveLoader _loader;
        private AnimationPlayer _animPlayer;
        private ColorRect _fadeRect;
        private TextureProgress _progressBar;
        private Timer _loaderPollTimer;


        private Main() {}
        private static Main instance;
        public static Main Instance {
            get {
                return instance;
            }
        }


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            instance = this;
            CacheNodeReferences();
            Setup();
        }

        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public void TransitionToLevel(String levelName)
        {
            ChangeSceneBackground($"{_levelDirectoryPath}{levelName}.tscn");
        }

        public void TransitionToMainMenu()
        {
            ChangeSceneBackground(_mainMenuFilePath);
        }


        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////
        private void OnChangeSceneRequest(String sceneResPath)
        {
            CallDeferred(nameof(ChangeSceneBackground), sceneResPath);
        }

        private void OnAnimationFinished(String animName)
        {
            _fadeRect.MouseFilter = Control.MouseFilterEnum.Ignore;
        }

        private void OnLoaderPollTimerTimeout()
        {
            Godot.Error err = _loader.Poll();

            switch (err)
            {
                case Godot.Error.FileEof:
                    PackedScene res = (PackedScene)_loader.GetResource();
                    _loader.Dispose(); // https://github.com/godotengine/godot/issues/33809
                    SetCurrentScene(res.Instance());
                    break;
                
                case Godot.Error.Ok:
                    float loadProgress = (float)_loader.GetStage() / (float)_loader.GetStageCount();
                    _progressBar.Value = loadProgress;
                    _loaderPollTimer.Start();
                    break;

                default:
                    Debug.Assert(false, $"ResourceInteractiveLoader Error: {err.ToString()}");
                    break;
            }
        }


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private void CacheNodeReferences()
        {
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _fadeRect = GetNode<ColorRect>("CanvasLayer/ColorRect");
            _progressBar = GetNode<TextureProgress>("CanvasLayer/TextureProgress");
            _loaderPollTimer = GetNode<Timer>("LoaderPollTimer");
        }

        private void Fade(bool fadeIn)
        {
            _fadeRect.MouseFilter = Control.MouseFilterEnum.Stop;
            _progressBar.Hide();
            if (fadeIn)
            {
                _animPlayer.Play("fade");
            }
            else
            {
                _animPlayer.PlayBackwards("fade");
            }
        }

        private async void SetCurrentScene(Node newScene)
        {
            GetTree().Root.AddChild(newScene);
            GetTree().CurrentScene = newScene;

            if (_animPlayer.IsPlaying())
            {
                await ToSignal(_animPlayer, "animation_finished");
            }
            Fade(false);
        }

        private async void Setup()
        {
            _loaderPollTimer.WaitTime = _POLL_TIME;

            // Make sure root node is ready to take new child
            await ToSignal(GetTree().Root, "ready");
            SetCurrentScene(_initialScene.Instance());
        }

        private async void ChangeSceneBackground(String newScenePath)
        {
            Fade(true);
            await ToSignal(_animPlayer, "animation_finished");
            _progressBar.Value = 0;
            _progressBar.Show();
            
            Node currentScene = GetTree().CurrentScene;
            if (currentScene != null)
            {
                currentScene.QueueFree();
                GetTree().CurrentScene = null;
            }
            
            _loader = ResourceLoader.LoadInteractive(newScenePath, "PackedScene");
            Debug.Assert(_loader != null, $"ResourceLoaderInteractive failed. Attemped load target path: {newScenePath}");
            _loaderPollTimer.Start();
        }
    }
}
