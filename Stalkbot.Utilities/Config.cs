﻿using System.IO;
using Newtonsoft.Json;

namespace StalkbotGUI.Stalkbot.Utilities
{
    public sealed class Config
    {
        // Discord
        public string Token { get; set; } = "changeme";
        public string Prefix { get; set; } = "change!";
        // Webcam
        public bool CamEnabled { get; set; } = false;
        public int CamTimer { get; set; } = 3000;
        public int CamWidth { get; set; } = 1280;
        public int CamHeight { get; set; } = 720;
        public int DefaultCam { get; set; } = 0;
        // Screenshot
        public bool SsEnabled { get; set; } = false;
        public double BlurAmount { get; set; } = 1;
        // Sounds
        public bool TtsEnabled { get; set; } = false;
        public bool PlayEnabled { get; set; } = false;
        public double Timeout { get; set; } = 10000;
        // Misc
        public bool ProcessesEnabled { get; set; } = false;
        public string FolderPath { get; set; } = "";
        public bool ClipboardEnabled { get; set; } = false;
        public bool AutoStartDiscord { get; set; } = false;
        public bool CloseToTray { get; set; } = false;

        // Actual Config 
        private static Config _instance;

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                    LoadConfig();
                return _instance;
            }
        }

        public void ReloadConfig()
            => _instance = new Config();

        public void SaveConfig()
            => File.WriteAllText("config.json", JsonConvert.SerializeObject(this, Formatting.Indented));

        public static void LoadConfig()
            => _instance = File.Exists("config.json")
                ? JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"))
                : new Config();

        public bool IsEnabled(string command)
        {
            switch (command)
            {
                case "webcam":
                    return CamEnabled;
                case "play":
                    return PlayEnabled;
                case "screenshot":
                    return SsEnabled;
                case "tts":
                    return TtsEnabled;
                case "folder":
                    return string.IsNullOrEmpty(FolderPath);
                case "proc":
                    return ProcessesEnabled;
                case "clipboard":
                    return ClipboardEnabled;
                default:
                    return false;
            }
        }
    }
}