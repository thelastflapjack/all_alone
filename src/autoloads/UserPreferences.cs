using Godot;
using System;
using System.Collections.Generic;


namespace GameGeneral
{
    public class UserPreferences : Node
    {

        /// Enums ///
        public enum AudioBus{
            Master = 0, SFX = 1,
            Music = 2, UI = 3
        }


        /// Properties - public, protected, private ///
        public Dictionary<AudioBus, float> AudioVolumes {
            get { return new Dictionary<AudioBus, float>(_audioVolumes); }
        }

        public float MouseSensitivity {
            get { return _mouseSensitivity; }
            set {
                _mouseSensitivity = Mathf.Clamp(value, 0.05f, 1);
            }
        }
        public bool ToggleSprint{
            get { return _toggleSprint; }
            set {
                _toggleSprint = value;
            }
        }

        public bool BorderlessWindow{
            get { return _borderlessWindow; }
            set {
                _borderlessWindow = value;
                OS.WindowBorderless = value;
            }
        }
        public bool Vsync{
            get { return _vsync; }
            set {
                _vsync = value;
                OS.VsyncEnabled = value;
            }
        }
        public bool Fxaa {
            get { return _fxaa; }
            set {
                _fxaa = value;
                GetViewport().Fxaa = value;
            }
        }
        public Viewport.MSAA Msaa{
            get { return _msaa; }
            set {
                _msaa = value;
                GetViewport().Msaa = value;
            }
        }

        /// Fields - protected or private ///
        public Dictionary<AudioBus, float> _audioVolumes = new Dictionary<AudioBus, float>()
        {
            {AudioBus.Master, 0.5f},
            {AudioBus.SFX, 0.5f},
            {AudioBus.Music, 0.5f},
            {AudioBus.UI, 0.5f},
        };
        
        private float _mouseSensitivity = 0.5f;
        private bool _toggleSprint = false;

        private bool _borderlessWindow = false;
        private bool _vsync = true;
        private bool _fxaa = true;
        private Viewport.MSAA _msaa = Viewport.MSAA.Msaa4x;

        private String _saveFilePath = "user://user_prefs.cfg";


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            Load();
        }

        
        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public void SetAudioBusVolume(AudioBus bus, float volumeLinear)
        {
            _audioVolumes[bus] = Mathf.Clamp(volumeLinear, 0, 1);
            AudioServer.SetBusVolumeDb((int)bus, VolumeLinearToDB(volumeLinear));
            AudioServer.SetBusMute((int)bus, volumeLinear == 0); // mute if val is 0, unmute otherwise
        }

        public void Save()
        {
            File fileChecker = new File();
            if (!fileChecker.FileExists(_saveFilePath))
            {
                // Create new file
                fileChecker.Open(_saveFilePath, File.ModeFlags.Write);
            }
            fileChecker.Close();

            ConfigFile preferencesFile = new ConfigFile();
            Error err = preferencesFile.Load(_saveFilePath);
            if (err == Error.Ok)
            {
                preferencesFile.SetValue("user_prefs", "audio_vol_master", _audioVolumes[AudioBus.Master]);
                preferencesFile.SetValue("user_prefs", "audio_vol_music", _audioVolumes[AudioBus.Music]);
                preferencesFile.SetValue("user_prefs", "audio_vol_sfx", _audioVolumes[AudioBus.SFX]);
                preferencesFile.SetValue("user_prefs", "audio_vol_ui", _audioVolumes[AudioBus.UI]);
                
                preferencesFile.SetValue("user_prefs", "mouse_sensitivity", MouseSensitivity);
                preferencesFile.SetValue("user_prefs", "toggle_sprint", ToggleSprint);
                
                preferencesFile.SetValue("user_prefs", "borderless_window", BorderlessWindow);
                preferencesFile.SetValue("user_prefs", "vsync", Vsync);
                preferencesFile.SetValue("user_prefs", "fxaa", Fxaa);
                preferencesFile.SetValue("user_prefs", "msaa", Msaa);

                preferencesFile.Save(_saveFilePath);
            }
        }


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private void Load()
        {
            ConfigFile preferencesFile = new ConfigFile();
            Error err = preferencesFile.Load(_saveFilePath);

            // Load values if file exists
            if (err == Error.Ok)
            {
                SetAudioBusVolume(AudioBus.Master, (float)preferencesFile.GetValue("user_prefs", "audio_vol_master"));
                SetAudioBusVolume(AudioBus.Music, (float)preferencesFile.GetValue("user_prefs", "audio_vol_music"));
                SetAudioBusVolume(AudioBus.SFX, (float)preferencesFile.GetValue("user_prefs", "audio_vol_sfx"));
                SetAudioBusVolume(AudioBus.UI, (float)preferencesFile.GetValue("user_prefs", "audio_vol_ui"));
                
                MouseSensitivity = (float)preferencesFile.GetValue("user_prefs", "mouse_sensitivity");
                ToggleSprint = (bool)preferencesFile.GetValue("user_prefs", "toggle_sprint");
                
                BorderlessWindow = (bool)preferencesFile.GetValue("user_prefs", "borderless_window");
                Vsync = (bool)preferencesFile.GetValue("user_prefs", "vsync");
                Fxaa = (bool)preferencesFile.GetValue("user_prefs", "fxaa");
                Msaa = (Viewport.MSAA)preferencesFile.GetValue("user_prefs", "msaa");
            } 

        }

        private float VolumeDBToLinear(float value)
        {
            return GD.Db2Linear(value - 6);
        }

        private float VolumeLinearToDB(float value)
        {
            return GD.Linear2Db(value) + 6;
        }
    }
}

