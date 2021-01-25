﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AForge.Video.DirectShow;
using Microsoft.WindowsAPICodePack.Dialogs;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow
    {
        /// <summary>
        /// Keeps track of the currently selected webcam
        /// </summary>
        private VideoCaptureDevice _selectedDevice;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigWindow()
        {
            InitializeComponent();
            FolderLabel.Content = string.IsNullOrEmpty(Config.Instance.FolderPath) ? "No folder selected." : Config.Instance.FolderPath;
            DurationInput.Text = $"{Config.Instance.Timeout}";
            BlurInput.Text = $"{Config.Instance.BlurAmount}";
            CamDelayInput.Text = $"{Config.Instance.CamTimer}";
            PrefixInput.Text = Config.Instance.Prefix;
            TokenInput.Text = Config.Instance.Token;
            AutoStartCheckBox.IsChecked = Config.Instance.AutoStartDiscord;
            MinimizeCheckBox.IsChecked = Config.Instance.MinimizeToTray;
            var webcams = new List<string>();
            for (var i = 0; i < Constants.Cameras.Count; i++)
                webcams.Add(Constants.Cameras[i].Name);
            CamSelector.ItemsSource = webcams;
            CamSelector.SelectedIndex = Config.Instance.DefaultCam;
            _selectedDevice = new VideoCaptureDevice(Constants.Cameras[Config.Instance.DefaultCam].MonikerString);
            UpdateResolutionSelector();
        }

        /// <summary>
        /// Handles clicking the folder picker
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">Event args</param>
        private void FolderSelect_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog("Folder Resources") { IsFolderPicker = true })
            {
                if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;
                Config.Instance.FolderPath = dialog.FileName;
                FolderLabel.Content = $"{Config.Instance.FolderPath}";
            }
        }

        /// <summary>
        /// Handles clicking the folder reset button
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">Event args</param>
        private void FolderClear_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.FolderPath = "";
            FolderLabel.Content = "No folder selected.";
        }

        /// <summary>
        /// Handles changing the camera selection
        /// </summary>
        /// <param name="sender">Combobox object</param>
        /// <param name="e">Event args</param>
        private void CamSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Config.Instance.DefaultCam = CamSelector.SelectedIndex;
            _selectedDevice = new VideoCaptureDevice(Constants.Cameras[Config.Instance.DefaultCam].MonikerString);
            UpdateResolutionSelector();
        }

        /// <summary>
        /// Updates contents of the resolution selector
        /// </summary>
        private void UpdateResolutionSelector()
            => ResolutionSelector.ItemsSource =
                _selectedDevice.VideoCapabilities.Select(x => $"{x.FrameSize.Width}x{x.FrameSize.Height}");

        /// <summary>
        /// Handles changing the resolution selection
        /// </summary>
        /// <param name="sender">Combobox object</param>
        /// <param name="e">Event args</param>
        private void ResolutionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Config.Instance.CamWidth = Convert.ToInt32(((string)ResolutionSelector.SelectedItem).Split('x').First());
            Config.Instance.CamHeight = Convert.ToInt32(((string)ResolutionSelector.SelectedItem).Split('x').Last());
        }

        /// <summary>
        /// Handles the window being closed via the X button
        /// </summary>
        /// <param name="sender">Window object</param>
        /// <param name="e">Event args</param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Config.Instance.SaveConfig();
            Logger.Log("Config saved", LogLevel.Info);
        }

        /// <summary>
        /// Handles text changing in the cam delay input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void CamDelayInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CamDelayInput.Text))
                Config.Instance.CamTimer = Convert.ToInt32(CamDelayInput.Text);
        }

        /// <summary>
        /// Handles text changing in the blur input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void BlurInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(BlurInput.Text))
                Config.Instance.BlurAmount = Convert.ToDouble(BlurInput.Text);
        }

        /// <summary>
        /// Handles text changing in the duration/timeout input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void DurationInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DurationInput.Text))
                Config.Instance.Timeout = Convert.ToDouble(DurationInput.Text);
        }

        /// <summary>
        /// Handles checkbox changes for minimizing
        /// </summary>
        /// <param name="sender">Checkbox object</param>
        /// <param name="e">Event args</param>
        private void MinimizeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (MinimizeCheckBox.IsChecked.HasValue)
                Config.Instance.MinimizeToTray = MinimizeCheckBox.IsChecked.Value;
        }

        /// <summary>
        /// Handles checkbox changes for auto starting
        /// </summary>
        /// <param name="sender">Checkbox object</param>
        /// <param name="e">Event args</param>
        private void AutoStartCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (AutoStartCheckBox.IsChecked.HasValue)
                Config.Instance.AutoStartDiscord = AutoStartCheckBox.IsChecked.Value;
        }

        /// <summary>
        /// Handles text changing in the token input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e"></param>
        private void TokenInput_TextChanged(object sender, TextChangedEventArgs e)
            => Config.Instance.Token = TokenInput.Text;

        /// <summary>
        /// Handles text changing in the prefix input
        /// </summary>
        /// <param name="sender">Textbox input</param>
        /// <param name="e">Event args</param>
        private void PrefixInput_TextChanged(object sender, TextChangedEventArgs e)
            => Config.Instance.Prefix = PrefixInput.Text;
    }
}