using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

            if (txt_Source.Text != "")
            {
                parameters.Add("source-dir", txt_Source.Text);
            }

            if (txt_Path.Text != "")
            {
                parameters.Add("output-dir", txt_Path.Text);
            }

            if (txt_Config.Text != "")
            {
                parameters.Add("config-file", txt_Config.Text);
            }

            if (chk_ConstraintChanges.IsEnabled && chk_ConstraintChanges.IsChecked.HasValue && chk_ConstraintChanges.IsChecked.Value && txt_ConstraintChanges.Text != "")
            {
                parameters.Add("constraint-changes", txt_ConstraintChanges.Text);
            }

            if (chk_MinChainLength.IsEnabled && chk_MinChainLength.IsChecked.HasValue && chk_MinChainLength.IsChecked.Value && txt_MinChainLength.Text != "")
            {
                parameters.Add("min-chain-length", txt_MinChainLength.Text);
            }

            if (chk_MinDifficulty.IsEnabled && chk_MinDifficulty.IsChecked.HasValue && chk_MinDifficulty.IsChecked.Value && txt_MinDifficulty.Text != "")
            {
                parameters.Add("min-difficulty", txt_MinDifficulty.Text);
            }

            if (chk_MaxSequenceBreakability.IsEnabled && chk_MaxSequenceBreakability.IsChecked.HasValue && chk_MaxSequenceBreakability.IsChecked.Value && txt_MaxSequenceBreakability.Text != "")
            {
                parameters.Add("max-sequence-breakability", txt_MaxSequenceBreakability.Text);
            }

            if (chk_MaxAttempts.IsEnabled && chk_MaxAttempts.IsChecked.HasValue && chk_MaxAttempts.IsChecked.Value && txt_MaxAttempts.Text != "")
            {
                parameters.Add("max-attempts", txt_MaxAttempts.Text);
            }

            if (chk_NumHardToReach.IsEnabled && chk_NumHardToReach.IsChecked.HasValue && chk_NumHardToReach.IsChecked.Value && txt_NumHardToReach.Text != "")
            {
                parameters.Add("num-hard-to-reach", txt_NumHardToReach.Text);
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

            if (chk_MapTransitionShuffle.IsEnabled && chk_MapTransitionShuffle.IsChecked.HasValue && chk_MapTransitionShuffle.IsChecked.Value)
            {
                settings.Add("shuffle-map-transitions");
            }

            if (chk_GiftShuffle.IsEnabled && chk_GiftShuffle.IsChecked.HasValue && chk_GiftShuffle.IsChecked.Value)
            {
                settings.Add("shuffle-gift-items");
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

            if (chk_HyperAttackMode.IsEnabled && chk_HyperAttackMode.IsChecked.HasValue && chk_HyperAttackMode.IsChecked.Value)
            {
                settings.Add("hyper-attack-mode");
            }

            if (chk_OpenMode.IsEnabled && chk_OpenMode.IsChecked.HasValue && chk_OpenMode.IsChecked.Value)
            {
                settings.Add("open-mode");
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

            if (txt_Source.Text != "")
            {
                parameters.Add("source-dir", txt_Source.Text);
            }
            if (txt_Path.Text != "")
            {
                parameters.Add("output-dir", txt_Path.Text);
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
                parameters.Add("output-dir", txt_Path.Text);
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

        private void chk_SuperAttackMode_Checked(object sender, RoutedEventArgs e)
        {
            chk_HyperAttackMode.IsChecked = false;
        }

        private void chk_HyperAttackMode_Checked(object sender, RoutedEventArgs e)
        {
            chk_SuperAttackMode.IsChecked = false;
        }

        private void btn_Source_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    txt_Source.Text = dialog.FileName;
                }
            }
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

        private bool advancedOptionsShown = false;
        private void btn_ShowAdvanced_Click(object sender, RoutedEventArgs e)
        {
            double advancedOptionsHeight = grd_AdvancedOptions.Height;

            if (advancedOptionsShown == false)
            {
                advancedOptionsShown = true;
                btn_ShowAdvanced.Content = "Hide advanced options";
                Height += advancedOptionsHeight;
                txt_Output.Height += advancedOptionsHeight;
                grd_Params.Height += advancedOptionsHeight;

                grd_AdvancedOptions.Visibility = Visibility.Visible;
            }
            else
            {
                advancedOptionsShown = false;
                btn_ShowAdvanced.Content = "Show advanced options";
                Height -= advancedOptionsHeight;
                txt_Output.Height -= advancedOptionsHeight;
                grd_Params.Height -= advancedOptionsHeight;

                grd_AdvancedOptions.Visibility = Visibility.Hidden;
            }
        }

        private void ChangeInfo(object sender, MouseEventArgs e)
        {
            object test = e.OriginalSource;
            FrameworkElement element = sender as FrameworkElement;
            switch (element.Name)
            {
                case "txt_Source":
                case "btn_Source":
                    Info = "Path to take original maps from. If left empty, original maps will be taken from original_maps folder.";
                    break;
                case "txt_Path":
                case "btn_Path":
                    Info = "Path to deposit generated maps into. If left empty, this will go in the generated_maps folder.";
                    break;
                case "txt_Config":
                case "btn_Config":
                    Info = "Path to config file to use. If left empty, the config.txt file in this application's folder will be used.";
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
                case "chk_GiftShuffle":
                    Info = "When checked, the Randomizer will also shuffle gift items in the game. These include: Speed Boost, Bunny Strike and P Hairpin.";
                    break;
                case "chk_MapTransitionShuffle":
                    Info = "When checked, the Randomizer will also shuffle all map transitions in the game. These are shuffled in pairs, so you won't get too lost.";
                    break;
                case "chk_ConstraintChanges":
                case "txt_ConstraintChanges":
                    Info = "When checked, the Randomizer will add the specified number of constraints to random areas. If not set, defaults to 0.";
                    break;
                case "chk_OpenMode":
                    Info = "When checked, the Randomizer will remove all blocking triggers, such as the ones that stop you from leaving Starting Forest in prologue.";
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
                    Info = "When checked, the Randomizer will give Erina 20 attack ups from the start. These attack ups are extra, and do not change any of the attack ups randomized in the game. Cannot be used with Hyper Attack Mode.";
                    break;
                case "chk_HyperAttackMode":
                    Info = "When checked, the Randomizer will give Erina 30 attack ups from the start. These attack ups are extra, and do not change any of the attack ups randomized in the game. Cannot be used with Super Attack Mode.";
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
                case "chk_MinChainLength":
                case "txt_MinChainLength":
                    Info = "Set a minimum chain length for generation. Faster than minimum difficulty. If not set, defaults to 0.";
                    break;
                case "chk_MinDifficulty":
                case "txt_MinDifficulty":
                    Info = "Set a minimum difficulty for generation. Slower than minimum chain length. If not set, defaults to 0.";
                    break;
                case "chk_MaxSequenceBreakability":
                case "txt_MaxSequenceBreakability":
                    Info = "Set a maximum seed breakability for generation. Slower than minimum chain length. If not set, defaults to None (infinity).";
                    break;
                case "chk_MaxAttempts":
                case "txt_MaxAttempts":
                    Info = "Set the maximum amount of seed generation attempts before giving up. If not set, defaults to 1000.";
                    break;
                case "chk_NumHardToReach":
                case "txt_NumHardToReach":
                    Info = "Set the number of hard to reach items/eggs. If not set, defaults to 5.";
                    break;
            }
        }

        private void RemoveInfo(object sender, MouseEventArgs e)
        {
            Info = "";
        }

        //
        // Profile
        //

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            StreamWriter writer = new StreamWriter("profile.ini");

            if (txt_Source.Text != "")
            {
                writer.WriteLine("source_path:" + txt_Source.Text);
            }
            if (txt_Path.Text != "")
            {
                writer.WriteLine("output_path:" + txt_Path.Text);
            }
            if (txt_Config.Text != "")
            {
                writer.WriteLine("config_file:" + txt_Config.Text);
            }

            writer.WriteLine("music_shuffle:" + chk_MusicShuffle.IsChecked);
            writer.WriteLine("bg_shuffle:" + chk_BgShuffle.IsChecked);
            writer.WriteLine("gift_shuffle:" + chk_GiftShuffle.IsChecked);
            writer.WriteLine("map_transition_shuffle:" + chk_MapTransitionShuffle.IsChecked);

            writer.WriteLine("constraint_changes_status:" + chk_ConstraintChanges.IsChecked);
            if (txt_ConstraintChanges.Text != "")
            {
                writer.WriteLine("constraint_changes:" + txt_ConstraintChanges.Text);
            }

            writer.WriteLine("open_mode:" + chk_OpenMode.IsChecked);
            writer.WriteLine("super_attack:" + chk_SuperAttackMode.IsChecked);
            writer.WriteLine("hyper_attack:" + chk_HyperAttackMode.IsChecked);
            writer.WriteLine("hide_difficulty:" + chk_HideDifficulty.IsChecked);
            writer.WriteLine("no_laggy_bg:" + chk_NoLaggyBackgrounds.IsChecked);
            writer.WriteLine("no_difficult_bg:" + chk_NoDifficultBackgrounds.IsChecked);
            writer.WriteLine("no_fixes:" + chk_NoFixes.IsChecked);

            writer.WriteLine("min_chain_length_status:" + chk_MinChainLength.IsChecked);
            writer.WriteLine("min_difficulty_status:" + chk_MinDifficulty.IsChecked);
            writer.WriteLine("max_sequence_breakability_status:" + chk_MaxSequenceBreakability.IsChecked);
            writer.WriteLine("max_attempts_status:" + chk_MaxAttempts.IsChecked);
            writer.WriteLine("num_hard_to_reach_status:" + chk_NumHardToReach.IsChecked);
            if (txt_MinChainLength.Text != "")
            {
                writer.WriteLine("min_chain_length:" + txt_MinChainLength.Text);
            }
            if (txt_MinDifficulty.Text != "")
            {
                writer.WriteLine("min_difficulty:" + txt_MinDifficulty.Text);
            }
            if (txt_MaxSequenceBreakability.Text != "")
            {
                writer.WriteLine("max_sequence_breakability:" + txt_MaxSequenceBreakability.Text);
            }
            if (txt_MaxAttempts.Text != "")
            {
                writer.WriteLine("max_attempts:" + txt_MaxAttempts.Text);
            }
            if (txt_NumHardToReach.Text != "")
            {
                writer.WriteLine("num_hard_to_reach:" + txt_NumHardToReach.Text);
            }

            writer.WriteLine("egg_mode:" + chk_EggGoalsMode.IsChecked);
            if ((bool)chk_EggGoalsMode.IsChecked && txt_ExtraEggs.Text != "")
            {
                writer.WriteLine("extra_eggs:" + txt_ExtraEggs.Text);
            }

            if (txt_ExtraParams.Text != "")
            {
                writer.WriteLine("extra_params:" + txt_ExtraParams.Text);
            }

            writer.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists("profile.ini"))
            {
                StreamReader reader = new StreamReader("profile.ini");

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    int index = line.IndexOf(':');
                    switch (line.Substring(0, index))
                    {
                        case "source_path":
                            txt_Source.Text = line.Substring(index + 1);
                            break;

                        case "output_path":
                            txt_Path.Text = line.Substring(index + 1);
                            break;

                        case "config_file":
                            txt_Config.Text = line.Substring(index + 1);
                            break;

                        case "music_shuffle":
                            chk_MusicShuffle.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "bg_shuffle":
                            chk_BgShuffle.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "gift_shuffle":
                            chk_GiftShuffle.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "map_transition_shuffle":
                            chk_MapTransitionShuffle.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "constraint_changes_status":
                            chk_ConstraintChanges.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "constraint_changes":
                            txt_ConstraintChanges.Text = line.Substring(index + 1);
                            break;

                        case "open_mode":
                            chk_OpenMode.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "super_attack":
                            chk_SuperAttackMode.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "hyper_attack":
                            chk_HyperAttackMode.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "no_laggy_bg":
                            chk_NoLaggyBackgrounds.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "no_difficult_bg":
                            chk_NoDifficultBackgrounds.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "hide_difficulty":
                            chk_HideDifficulty.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "no_fixes":
                            chk_NoFixes.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "min_chain_length_status":
                            chk_MinChainLength.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "min_difficulty_status":
                            chk_MinDifficulty.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "max_sequence_breakability_status":
                            chk_MaxSequenceBreakability.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "max_attempts_status":
                            chk_MaxAttempts.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "num_hard_to_reach_status":
                            chk_NumHardToReach.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "min_chain_length":
                            txt_MinChainLength.Text = line.Substring(index + 1);
                            break;

                        case "min_difficulty":
                            txt_MinDifficulty.Text = line.Substring(index + 1);
                            break;

                        case "max_sequence_breakability":
                            txt_MaxSequenceBreakability.Text = line.Substring(index + 1);
                            break;

                        case "max_attempts":
                            txt_MaxAttempts.Text = line.Substring(index + 1);
                            break;

                        case "num_hard_to_reach":
                            txt_NumHardToReach.Text = line.Substring(index + 1);
                            break;

                        case "egg_mode":
                            chk_EggGoalsMode.IsChecked = bool.Parse(line.Substring(index + 1));
                            break;

                        case "extra_eggs":
                            txt_ExtraEggs.Text = line.Substring(index + 1);
                            break;

                        case "extra_params":
                            txt_ExtraParams.Text = line.Substring(index + 1);
                            break;
                    }
                }

                reader.Close();
            }
        }
    }
}
