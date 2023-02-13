using Godot;
using GameGeneral;


namespace UI
{
    public class SettingsPage : UI.MultiPage.Page
    {
        private UserPreferences _userPreferences;
        
        private HSlider _sliderAudioMaster;
        private HSlider _sliderAudioSfx;
        private HSlider _sliderAudioMusic;
        private HSlider _sliderAudioUI;
        private HSlider _sliderMouseSensitivity;
        private CheckButton _checkBtnBorderless;
        private CheckButton _checkBtnVsync;
        private CheckButton _checkBtnFxaa;
        private OptionButton _optionBtnMsaa;

        public override void _Ready()
        {
            CacheNodeReferences();
            InitalizeSettingUIValues();
        }


        private void OnBackButtonPressed()
        {
            if(IsActive)
            {
                _userPreferences.Save();
                EmitSignal(nameof(ChangePageRequested), _backPageName);
            }
        }

        private void OnAudioSliderValueChanged(float value, int busIdx)
        {
            if (IsActive)
            {
                _userPreferences.SetAudioBusVolume((UserPreferences.AudioBus)busIdx, value);
            }
        }

        private void OnSliderMouseSenseValueChanged(float value)
        {
            if (IsActive)
            {
                _userPreferences.MouseSensitivity = value;
            }
        }

        private void OnCheckBtnBorderlessPressed()
        {
            if (IsActive)
            {
                _userPreferences.BorderlessWindow = _checkBtnBorderless.Pressed;
            }
        }

        private void OnCheckBtnVsyncPressed()
        {
            if (IsActive)
            {
                _userPreferences.Vsync = _checkBtnVsync.Pressed;
            }
        }

        private void OnCheckBtnFxaaPressed()
        {
            if (IsActive)
            {
                _userPreferences.Fxaa = _checkBtnVsync.Pressed;
            }
        }

        private void OnOptionBtnMsaaItemSeleted(int index)
        {
            if (IsActive)
            {
                _userPreferences.Msaa = (Viewport.MSAA)index;
            }
        }


        private void CacheNodeReferences()
        {
            _userPreferences = GetNode<UserPreferences>("/root/UserPreferences");
            
            _sliderAudioMaster = (HSlider)FindNode("SliderAudioMaster");
            _sliderAudioSfx = (HSlider)FindNode("SliderAudioSFX");
            _sliderAudioMusic = (HSlider)FindNode("SliderAudioMusic");
            _sliderAudioUI = (HSlider)FindNode("SliderAudioUI");
            _sliderMouseSensitivity = (HSlider)FindNode("SliderMouseSense");
            _checkBtnBorderless = (CheckButton)FindNode("BorderlessCheckButton");
            _checkBtnVsync = (CheckButton)FindNode("VSyncCheckButton");
            _checkBtnFxaa = (CheckButton)FindNode("FXAACheckButton");
            _optionBtnMsaa = (OptionButton)FindNode("MSAAOptionButton");
        }

        private void InitalizeSettingUIValues()
        {
            _sliderAudioMaster.Value = _userPreferences.AudioVolumes[UserPreferences.AudioBus.Master];
            _sliderAudioSfx.Value = _userPreferences.AudioVolumes[UserPreferences.AudioBus.SFX];
            _sliderAudioMusic.Value = _userPreferences.AudioVolumes[UserPreferences.AudioBus.Music];
            _sliderAudioUI.Value = _userPreferences.AudioVolumes[UserPreferences.AudioBus.UI];

            _sliderMouseSensitivity.Value = _userPreferences.MouseSensitivity;

            _checkBtnBorderless.Pressed = _userPreferences.BorderlessWindow;
            _checkBtnFxaa.Pressed = _userPreferences.Fxaa;
            _checkBtnVsync.Pressed = _userPreferences.Vsync;
            _optionBtnMsaa.Selected = (int)_userPreferences.Msaa;
        }
    }
}

