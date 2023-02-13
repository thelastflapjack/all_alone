using Godot;
using System;
using System.Collections.Generic;
using Weapons;


namespace GunStatsEditor
{
    [Tool]
    public class EditorDock : VBoxContainer
    {
        private const String _gunDirectoryPath = "res://src/weapons/guns/";

        private GridContainer _statsGrid;
        private Dictionary<String, Dictionary<String, SpinBox>> _statCells = new Dictionary<string, Dictionary<String, SpinBox>>();


        public override void _Ready()
        {
            _statsGrid = GetNode<GridContainer>("Panel/ScrollContainer/GridContainer");
            LoadData();
        }
        

        private List<String> GetAllGunNames()
        {
            List<String> allNames = new List<String>();

            Directory dir = new Directory();
            dir.Open(_gunDirectoryPath);
            dir.ListDirBegin(true, true);

            String fileName = dir.GetNext();
            while (fileName != "")
            {
                if (dir.CurrentIsDir())
                {
                    allNames.Add(fileName);
                }
                fileName = dir.GetNext();
            }

            return allNames;
        }

        private void LoadData()
        {
            List<String> allGunNames = GetAllGunNames();
            foreach (String gunName in allGunNames)
            {
                GunStats gunStats = ResourceLoader.Load<GunStats>(
                    $"{_gunDirectoryPath}/{gunName}/stats.tres"
                );

                _statCells.Add(gunName, new Dictionary<String, SpinBox>());

                AddRowTitle(gunName, Label.AlignEnum.Right);

                AddStatCell(gunName, nameof(GunStats.ProjectileDamage), gunStats.ProjectileDamage);
                AddStatCell(gunName, nameof(GunStats.FireRate), gunStats.FireRate);
                AddStatCell(gunName, nameof(GunStats.MagazineSize), gunStats.MagazineSize);
                AddStatCell(gunName, nameof(GunStats.Spread), gunStats.Spread);
                AddStatCell(gunName, nameof(GunStats.SpreadGainPerShot), gunStats.SpreadGainPerShot);
                AddStatCell(gunName, nameof(GunStats.Recoil), gunStats.Recoil);
                AddStatCell(gunName, nameof(GunStats.ShotProjectileCount), gunStats.ShotProjectileCount);
                AddStatCell(gunName, nameof(GunStats.ProjectileSpeed), gunStats.ProjectileSpeed);
                AddStatCell(gunName, nameof(GunStats.MaxRange), gunStats.MaxRange);
                AddStatCell(gunName, nameof(GunStats.ReloadTime), gunStats.ReloadTime);
                AddStatCell(gunName, nameof(GunStats.MaxAmmo), gunStats.MaxAmmo);
                AddStatCell(gunName, nameof(GunStats.IsAutomatic), gunStats.IsAutomatic ? 1 : 0);
                AddStatCell(gunName, nameof(GunStats.BuyCost), gunStats.BuyCost);
                
                AddRowTitle(gunName, Label.AlignEnum.Left);
            }
        }

        private void AddRowTitle(String gunName, Label.AlignEnum alignment)
        {
            Label rowTitle = new Label();
            rowTitle.Text = gunName;
            rowTitle.Align = alignment;
            _statsGrid.AddChild(rowTitle);
        }

        private void AddStatCell(String gunName, String statName, float value)
        {
            SpinBox newCell = new SpinBox();
            switch(statName)
            {
                case nameof(GunStats.ProjectileDamage):
                    newCell.Step = 1;
                    newCell.MaxValue = 200;
                    newCell.MinValue = 1;
                    break;
                    
                case nameof(GunStats.FireRate):
                    newCell.Step = 25;
                    newCell.MaxValue = 1500;
                    newCell.MinValue = 10;
                    break;

                case nameof(GunStats.MagazineSize):
                    newCell.Step = 1;
                    newCell.MaxValue = 200;
                    newCell.MinValue = 1;
                    break;

                case nameof(GunStats.Spread):
                    newCell.Step = 0.5;
                    newCell.MaxValue = 20;
                    newCell.MinValue = 0;
                    break;

                case nameof(GunStats.SpreadGainPerShot):
                    newCell.Step = 0.05;
                    newCell.MaxValue = 20;
                    newCell.MinValue = 0;
                    break;

                case nameof(GunStats.Recoil):
                    newCell.Step = 0.01;
                    newCell.MaxValue = 1;
                    newCell.MinValue = 0;
                    break;

                case nameof(GunStats.ShotProjectileCount):
                    newCell.Step = 1;
                    newCell.MaxValue = 20;
                    newCell.MinValue = 1;
                    break;

                case nameof(GunStats.ProjectileSpeed):
                    newCell.Step = 5;
                    newCell.MaxValue = 250;
                    newCell.MinValue = 50;
                    break;

                case nameof(GunStats.MaxRange):
                    newCell.Step = 5;
                    newCell.MaxValue = 200;
                    newCell.MinValue = 25;
                    break;

                case nameof(GunStats.ReloadTime):
                    newCell.Step = 0.1;
                    newCell.MaxValue = 10;
                    newCell.MinValue = 1;
                    break;

                case nameof(GunStats.MaxAmmo):
                    newCell.Step = 1;
                    newCell.MaxValue = 500;
                    newCell.MinValue = 5;
                    break;

                case nameof(GunStats.IsAutomatic):
                    newCell.Step = 1;
                    newCell.MaxValue = 1;
                    newCell.MinValue = 0;
                    break;

                case nameof(GunStats.BuyCost):
                    newCell.Step = 50;
                    newCell.MaxValue = 2000;
                    newCell.MinValue = 100;
                    break;
                
                default:
                    break;
            }

            newCell.Align = LineEdit.AlignEnum.Center;
            newCell.AllowGreater = false;
            newCell.AllowLesser = false;
            newCell.Value = value;

            _statsGrid.AddChild(newCell);
            _statCells[gunName][statName] = newCell;
        }
    
        private void SaveChanges()
        {
            foreach (String gunName in _statCells.Keys)
            {
                Dictionary<String, SpinBox> gunStatCells = _statCells[gunName];
                String saveFilePath = $"{_gunDirectoryPath}{gunName}/stats.tres";

                // Can't create and save new custom resources with c#
                GunStats gunStats = ResourceLoader.Load<GunStats>(saveFilePath);

                gunStats.ProjectileDamage = (int)gunStatCells[nameof(GunStats.ProjectileDamage)].Value;
                gunStats.FireRate = (int)gunStatCells[nameof(GunStats.FireRate)].Value;
                gunStats.MagazineSize = (int)gunStatCells[nameof(GunStats.MagazineSize)].Value;
                gunStats.Spread = (float)gunStatCells[nameof(GunStats.Spread)].Value;
                gunStats.SpreadGainPerShot = (float)gunStatCells[nameof(GunStats.SpreadGainPerShot)].Value;
                gunStats.Recoil = (float)gunStatCells[nameof(GunStats.Recoil)].Value;
                gunStats.ShotProjectileCount = (int)gunStatCells[nameof(GunStats.ShotProjectileCount)].Value;
                gunStats.ProjectileSpeed = (int)gunStatCells[nameof(GunStats.ProjectileSpeed)].Value;
                gunStats.MaxRange = (int)gunStatCells[nameof(GunStats.MaxRange)].Value;
                gunStats.ReloadTime = (float)gunStatCells[nameof(GunStats.ReloadTime)].Value;
                gunStats.MaxAmmo = (int)gunStatCells[nameof(GunStats.MaxAmmo)].Value;
                gunStats.IsAutomatic = gunStatCells[nameof(GunStats.IsAutomatic)].Value == 1;
                gunStats.BuyCost = (int)gunStatCells[nameof(GunStats.BuyCost)].Value;

                Error err = ResourceSaver.Save(saveFilePath, gunStats);
            }
        }
    }
}

