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
        private string[] DevFlags = new string[]
        {
        };

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

            CheckForUpdates();
            string branch = CheckForBranch();
            if (branch.StartsWith("M"))
            {
                foreach (string devFlag in DevFlags)
                {
                    FrameworkElement devElement = (FrameworkElement)FindName(devFlag);
                    devElement.Visibility = Visibility.Hidden;
                }
            }
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
                if (txt_Seed.Text.Length > 65535)
                {
                    MessageBox.Show("Why? Why is your seed over 65535 characters? Please, don't do this. Think of the children.");
                    return;
                }

                if (txt_Seed.Text.All(char.IsLetterOrDigit))
                {
                    parameters.Add("seed", txt_Seed.Text);
                }
                else
                {
                    MessageBox.Show("Invalid seed. Seed must be an alphanumeric string.\nRandom will be used.");
                }
            }

            if (txt_Path.Text != "")
            {
                parameters.Add("output_dir", txt_Path.Text);
            }

            if (txt_Config.Text != "")
            {
                parameters.Add("config_file", txt_Config.Text);
            }

            if (chk_NoWrite.IsEnabled && chk_NoWrite.IsChecked.HasValue && chk_NoWrite.IsChecked.Value)
            {
                settings.Add("no-write");
            }

            if (chk_MusicShuffle.IsEnabled && chk_MusicShuffle.IsChecked.HasValue && chk_MusicShuffle.IsChecked.Value)
            {
                settings.Add("shuffle-music");
            }

            if (chk_BgShuffle.IsEnabled && chk_BgShuffle.IsChecked.HasValue && chk_BgShuffle.IsChecked.Value)
            {
                settings.Add("shuffle-backgrounds");
            }

            if (chk_HideUnreachable.IsEnabled && chk_HideUnreachable.IsChecked.HasValue && chk_HideUnreachable.IsChecked.Value)
            {
                settings.Add("hide-unreachable");
            }

            if (chk_HideDifficulty.IsEnabled && chk_HideDifficulty.IsChecked.HasValue && chk_HideDifficulty.IsChecked.Value)
            {
                settings.Add("hide-difficulty");
            }

            if (chk_NoFixes.IsEnabled && chk_NoFixes.IsChecked.HasValue && chk_NoFixes.IsChecked.Value)
            {
                settings.Add("no-fixes");
            }

            if (chk_NoLaggyBackgrounds.IsEnabled && chk_NoLaggyBackgrounds.IsChecked.HasValue && chk_NoLaggyBackgrounds.IsChecked.Value)
            {
                settings.Add("no-laggy-backgrounds");
            }

            if (chk_NoDifficultBackgrounds.IsEnabled && chk_NoDifficultBackgrounds.IsChecked.HasValue && chk_NoDifficultBackgrounds.IsChecked.Value)
            {
                settings.Add("no-difficult-backgrounds");
            }

            if (chk_SuperAttackMode.IsEnabled && chk_SuperAttackMode.IsChecked.HasValue && chk_SuperAttackMode.IsChecked.Value)
            {
                settings.Add("super-attack-mode");
            }

            if (chk_EggGoalsMode.IsEnabled && chk_EggGoalsMode.IsChecked.HasValue && chk_EggGoalsMode.IsChecked.Value)
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

            string output = FileIO.CallRandomizer(parameters, settings, txt_ExtraParams.Text);

            txt_Output.Text = output;

            //ConfigData data = FileIO.ReadConfig("config.txt");
            //FileIO.WriteConfig("config2.txt", data);
        }

        private void CheckForUpdates()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            HashSet<string> settings = new HashSet<string>();

            settings.Add("check-for-updates");

            string output = FileIO.CallRandomizer(parameters, settings);

            MessageBox.Show(output);
        }

        private string CheckForBranch()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            HashSet<string> settings = new HashSet<string>();

            settings.Add("check-branch");

            string output = FileIO.CallRandomizer(parameters, settings);

            return output;
        }

        private void Reset_Maps_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            HashSet<string> settings = new HashSet<string>() { "reset" };

            if (txt_Path.Text != "")
            {
                parameters.Add("output_dir", txt_Path.Text);
            }
            
            string output = FileIO.CallRandomizer(parameters, settings);

            txt_Output.Text = output;
        }

        private void Hash_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            HashSet<string> settings = new HashSet<string>() { "hash" };

            if (txt_Path.Text != "")
            {
                parameters.Add("output_dir", txt_Path.Text);
            }

            string output = FileIO.CallRandomizer(parameters, settings);

            txt_Output.Text = output;
        }

        private void btn_RandomSeed_Click(object sender, RoutedEventArgs e)
        {
            char[] values = new char[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };

            Random r = new Random();
            string seed = "";
            for (int i = 0; i < 16; i++)
            {
                seed += values[r.Next(values.Length)];
            }

            txt_Seed.Text = seed;
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

        private void btn_Config_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    txt_Config.Text = dialog.FileName;
                }
            }
        }

        private void btn_Version_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            HashSet<string> settings = new HashSet<string>() { "version" };
            
            string output = FileIO.CallRandomizer(parameters, settings);

            MessageBox.Show(output);
        }

        private void btn_CmdHelp_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            HashSet<string> settings = new HashSet<string>() { "help" };

            string output = FileIO.CallRandomizer(parameters, settings);

            // Trim off the first part of the help message
            output = output.Substring(output.IndexOf("Rabi-Ribi"));
            output = "Note: all dev version exclusive features can only be entered via the Extra Parameters text box. The list of parameters is given below.\n\n" + output;

            txt_Output.Text = output;
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
                case "chk_BgShuffle":
                    Info = "When checked, the Randomizer will also shuffle room backgrounds used in the game. This can get pretty wild.";
                    break;
                case "chk_HideUnreachable":
                    Info = "When checked, the Randomizer output will not show the unreachable items in the randomized maps.";
                    break;
                case "chk_HideDifficulty":
                    Info = "When checked, the Randomizer output will not show the difficulty of the seed.";
                    break;
                case "chk_NoFixes":
                    Info = "When checked, the Randomizer will not perform specific map fixes.";
                    break;
                case "chk_NoLaggyBackgrounds":
                    Info = "When checked, the Randomizer will remove laggy backgrounds from the randomized maps.";
                    break;
                case "chk_NoDifficultBackgrounds":
                    Info = "When checked, the Randomizer will remove backgrounds that are considered \"difficult\" (backgrounds that obscure vision and such) from the randomized maps.";
                    break;
                case "chk_SuperAttackMode":
                    Info = "When checked, the Randomizer will give Erina 20 attack ups from the start. These attack ups are extra, and do not change any of the attack ups randomized in the game.";
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
