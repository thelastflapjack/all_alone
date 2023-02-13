using Godot;
using System;
using System.Collections;
using System.Collections.Generic;


namespace ZombieHoardGame
{
    public class SavedData : Node
    {
        
        private const String LevelBestsFilePath = "user://level_bests.cfg";

        public int LevelHighestRound(String levelName)
        {
            ConfigFile levelBestsFile = new ConfigFile();
            Error err = levelBestsFile.Load(LevelBestsFilePath);
            if (err != Error.Ok)
            {
                return 0;
            }
            else
            {
                return (int)levelBestsFile.GetValue(levelName, "round", 0);
            }
        }

        public void UpdateLevelHighestRound(String levelName, int newBest)
        {
            File fileChecker = new File();
            if (!fileChecker.FileExists(LevelBestsFilePath))
            {
                // Create new file
                fileChecker.Open(LevelBestsFilePath, File.ModeFlags.Write);
            }
            fileChecker.Close();

            ConfigFile levelBestsFile = new ConfigFile();
            Error err = levelBestsFile.Load(LevelBestsFilePath);
            if (err == Error.Ok)
            {
                levelBestsFile.SetValue(levelName, "round", newBest);
                levelBestsFile.Save(LevelBestsFilePath);
            }
        }
    }
}

