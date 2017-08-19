using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RabiRibiRandomizerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string m_Info;
        public string Info
        {
            get { return m_Info; }
            set
            {
                m_Info = value;
                RaisePropertyChanged("Info");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;

            handlers(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Generate_Button_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            HashSet<string> settings = new HashSet<string>();

            if (txt_Seed.Text != "")
            {
                int seed;
                if (int.TryParse(txt_Seed.Text, out seed))
                {
                    parameters.Add("seed", seed);
                }
                else
                {
                    MessageBox.Show("Invalid seed. Seed must be an integer.\nRandom will be used.");
                }
            }

            if (txt_Path.Text != "")
            {
                parameters.Add("output_dir", txt_Path.Text);
            }

            if (chk_NoWrite.IsChecked.HasValue && chk_NoWrite.IsChecked.Value)
            {
                settings.Add("no-write");
            }

            if (chk_MusicShuffle.IsChecked.HasValue && chk_MusicShuffle.IsChecked.Value)
            {
                settings.Add("shuffle-music");
            }

            if (chk_EggGoalsMode.IsChecked.HasValue && chk_EggGoalsMode.IsChecked.Value)
            {
                settings.Add("egg-goals");
                if (txt_ExtraEggs.Text != "")
                {
                    int extraEggs;
                    if (int.TryParse(txt_ExtraEggs.Text, out extraEggs))
                    {
                        parameters.Add("extra-eggs", extraEggs);
                    }
                    else
                    {
                        MessageBox.Show("Invalid number of extra eggs. Defaulting to 0.");
                    }
                }
            }

            if (txt_ExtraParams.Text != "")
            {
                string[] extraParams = txt_ExtraParams.Text.Split(' ');
                for (int i = 0; i < extraParams.Length; i++)
                {
                    settings.Add(extraParams[i]);
                }
            }

            string output = FileIO.CallRandomizer(parameters, settings);

            MessageBox.Show(output);

            //ConfigData data = FileIO.ReadConfig("config.txt");
            //FileIO.WriteConfig("config2.txt", data);
        }

        private void Reset_Maps_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            HashSet<string> settings = new HashSet<string>();

            if (txt_Path.Text != "")
            {
                parameters.Add("output_dir", txt_Path.Text);
            }

            settings.Add("reset");

            string output = FileIO.CallRandomizer(parameters, settings);

            MessageBox.Show(output);
        }

        private void btn_Path_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    txt_Path.Text = dialog.FileName;
                }
            }
        }

        private void ChangeInfo(object sender, MouseEventArgs e)
        {
            object test = e.OriginalSource;
            FrameworkElement element = sender as FrameworkElement;
            switch (element.Name)
            {
                case "txt_Path":
                case "btn_Path":
                    Info = "Path to deposit generated maps into. If left empty, this will go in the generated_maps folder.";
                    break;
                case "lbl_Seed":
                case "txt_Seed":
                    Info = "Integer used to randomize maps. Using the same seed more than once will result in the same randomized maps. If left empty, a random seed will be used.";
                    break;
                case "chk_NoWrite":
                    Info = "When checked, the Randomizer will not generate maps and only return an analysis.";
                    break;
                case "chk_MusicShuffle":
                    Info = "When checked, the Randomizer will also shuffle music tracks used in the game.";
                    break;
                case "chk_EggGoalsMode":
                    Info = "When checked, the Randomizer will replace all \"Hard to Reach\" items with eggs, and remove all other eggs. The goal is to collect X number of eggs in this case.";
                    break;
                case "lbl_ExtraEggs":
                case "txt_ExtraEggs":
                    Info = "For Egg goals mode. You can specify a number of eggs to keep in the maps on top of the \"Hard to Reach\" eggs.";
                    break;
                case "lbl_ExtraParams":
                case "txt_ExtraParams":
                    Info = "Send parameters to the Randomizer by hand. Not recommended unless you know what you're doing.";
                    break;
            }
        }

        private void RemoveInfo(object sender, MouseEventArgs e)
        {
            Info = "";
        }
    }
}
