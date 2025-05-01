using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class GameMenuUI : MyPanelUI
    {

        public GameMenuUI(Scene scene)  : base(scene, Localization.GetLocalizedText("game_menu"), Color.White)
        {
            ListViewUI exitListView = new ListViewUI(scene, 380, 48, 2, 1, false, false);
            exitListView.Name = "ExitListView";
            exitListView.X = 8;
            exitListView.Y = 8 + 32;
            exitListView.Active = false;
            AddChildNode(exitListView);

            ListViewUI saveListView = new ListViewUI(scene, 380, 40, 6);
            saveListView.Name = "SaveListView";
            saveListView.X = 8;
            saveListView.Y = 8 + 32;
            saveListView.Active = false;
            AddChildNode(saveListView);

            ListViewUI audioListView = new ListViewUI(scene, 380, 40, 6);
            audioListView.Name = "AudioListView";
            audioListView.X = 8;
            audioListView.Y = 8 + 32;
            audioListView.Active = false;
            AddChildNode(audioListView);

            ListViewUI settingsListView = new ListViewUI(scene, 380, 40, 6);
            settingsListView.Name = "SettingsListView";
            settingsListView.X = 8;
            settingsListView.Y = 8 + 32;
            settingsListView.Active = false;
            AddChildNode(settingsListView);

            Width = exitListView.Width + 16;
            Height = saveListView.Height + 64;

            Tab exitTab = new Tab(scene, ResourceManager.TurnOnIcon, false);
            exitTab.Name = "ExitTab";
            exitTab.X = 8;
            exitTab.Y = -exitTab.Height;
            AddChildNode(exitTab);

            Tab saveTab = new Tab(scene, TextureBank.UITexture.GetSubtexture(192, 16, 16, 16), false);
            saveTab.Name = "SaveTab";
            saveTab.X = exitTab.LocalX + exitTab.Width;
            saveTab.Y = -saveTab.Height;
            AddChildNode(saveTab);

            Tab audioTab = new Tab(scene, TextureBank.UITexture.GetSubtexture(64, 144, 16, 16), false);
            audioTab.Name = "AudioTab";
            audioTab.X = saveTab.LocalX + saveTab.Width;
            audioTab.Y = -audioTab.Height;
            AddChildNode(audioTab);

            Tab settingsTab = new Tab(scene, TextureBank.UITexture.GetSubtexture(224, 0, 16, 16), false);
            settingsTab.Name = "SettingsTab";
            settingsTab.X = audioTab.LocalX + audioTab.Width;
            settingsTab.Y = -settingsTab.Height;
            AddChildNode(settingsTab);

            AddComponent(new GameMenuScript());
        }

    }
}
