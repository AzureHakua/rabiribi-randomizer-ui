using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace RabiRibiRandomizerUI
{
    /// <summary>
    /// Interaction logic for ConfigEditor.xaml
    /// </summary>
    public partial class ConfigEditor : Window
    {
        private int currentList = 0;
        private string[] labels = new string[] { "Additional items", "To shuffle", "Must be reachable" };
        private List<string>[] lists = new List<string>[3];
        private Button[] buttons;
        private string s_configPath;

        public ConfigEditor(string configPath)
        {
            InitializeComponent();

            buttons = new Button[] { btn_switchAI, btn_switchTS, btn_switchMBR };

            if (configPath != "")
            {
                s_configPath = configPath;

                ConfigData config = FileIO.ReadConfig(configPath);

                for (int k = 0; k < cbo_Knowledge.Items.Count; k++)
                {
                    ComboBoxItem item = (ComboBoxItem)cbo_Knowledge.Items[k];
                    if (config.knowledge.Equals(item.Content))
                    {
                        cbo_Knowledge.SelectedIndex = k;
                    }
                }

                for (int t = 0; t < cbo_Trick.Items.Count; t++)
                {
                    ComboBoxItem item = (ComboBoxItem)cbo_Trick.Items[t];
                    if (config.trick_difficulty.Equals(item.Content))
                    {
                        cbo_Trick.SelectedIndex = t;
                    }
                }

                chk_Darkness.IsChecked = config.settings["DARKNESS_WITHOUT_LIGHT_ORB"];
                chk_Zip.IsChecked = config.settings["ZIP_REQUIRED"];
                chk_Semisolid.IsChecked = config.settings["SEMISOLID_CLIPS_REQUIRED"];
                chk_Block.IsChecked = config.settings["BLOCK_CLIPS_REQUIRED"];
                chk_Plurkwood.IsChecked = config.settings["PLURKWOOD_REACHABLE"];
                chk_Postgame.IsChecked = config.settings["POST_GAME_ALLOWED"];
                chk_Irisu.IsChecked = config.settings["POST_IRISU_ALLOWED"];
                chk_Halloween.IsChecked = config.settings["HALLOWEEN_REACHABLE"];
                chk_WarpDestination.IsChecked = config.settings["WARP_DESTINATION_REACHABLE"];
                chk_EventWarps.IsChecked = config.settings["EVENT_WARPS_REQUIRED"];

                lists[0] = new List<string>(config.additional_items);
                lists[1] = new List<string>(config.to_shuffle);
                lists[2] = new List<string>(config.must_be_reachable);

                //load additional items list
                lbl_ConfigList.Content = labels[0];
                for (int ai = 0; ai < config.additional_items.Length; ai++)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = config.additional_items[ai];
                    lst_ConfigList.Items.Add(item);
                }
                buttons[0].IsEnabled = false;
            }
            else
            {
                for (int l = 0; l < lists.Length; l++)
                {
                    lists[l] = new List<string>();
                }

                s_configPath = "config.txt";
            }

            if (!File.Exists("all_items.txt"))
            {
                throw new FileNotFoundException("all_items.txt does not exist!");
            }

            string[] items = File.ReadAllLines("all_items.txt");
            for (int i = 0; i < items.Length; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = items[i];
                lst_AllItems.Items.Add(item);
            }
        }

        private void lst_AllItems_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SetAddButtonEnabled();
        }

        private void SetAddButtonEnabled()
        {
            if (lst_AllItems.SelectedIndex != -1)
            {
                if (currentList == 0)
                {
                    btn_Add.IsEnabled = true;
                }
                else
                {
                    string item = (string)((ListBoxItem)lst_AllItems.SelectedValue).Content;
                    btn_Add.IsEnabled = (lists[currentList].IndexOf(item) == -1);
                }
            }
            else
            {
                btn_Add.IsEnabled = false;
            }
        }

        private void lst_ConfigList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            btn_Remove.IsEnabled = (lst_ConfigList.SelectedIndex != -1);
        }

        private void AddToList(object sender, RoutedEventArgs e)
        {
            string item = (string)((ListBoxItem)lst_AllItems.SelectedValue).Content;
            lists[currentList].Add(item);
            ResetConfigList(lists[currentList]);
            SetAddButtonEnabled();
        }

        private void RemoveFromList(object sender, RoutedEventArgs e)
        {
            string item = (string)((ListBoxItem)lst_ConfigList.SelectedValue).Content;
            lists[currentList].Remove(item);
            ResetConfigList(lists[currentList]);
            SetAddButtonEnabled();
        }

        private void ChangeList(object sender, RoutedEventArgs e)
        {
            buttons[currentList].IsEnabled = true;

            string tag = (string)((Button)sender).Tag;
            switch (tag)
            {
                case "AI":
                    currentList = 0;
                    break;

                case "TS":
                    currentList = 1;
                    break;

                case "MBR":
                    currentList = 2;
                    break;
            }

            lbl_ConfigList.Content = labels[currentList];
            ResetConfigList(lists[currentList]);
            buttons[currentList].IsEnabled = false;
            SetAddButtonEnabled();
        }

        private void ResetConfigList(List<string> newList)
        {
            lst_ConfigList.Items.Clear();
            for (var i = 0; i < newList.Count; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = newList[i];
                lst_ConfigList.Items.Add(item);
            }
        }

        private void SaveConfig(object sender, RoutedEventArgs e)
        {
            ConfigData config = new ConfigData();

            ComboBoxItem knowledge = (ComboBoxItem)cbo_Knowledge.SelectedItem;
            config.knowledge = (string)knowledge.Content;

            ComboBoxItem trick_difficulty = (ComboBoxItem)cbo_Trick.SelectedItem;
            config.trick_difficulty = (string)trick_difficulty.Content;

            config.settings = new Dictionary<string, bool>();
            config.settings["DARKNESS_WITHOUT_LIGHT_ORB"] = chk_Darkness.IsChecked.Value;
            config.settings["ZIP_REQUIRED"] = chk_Zip.IsChecked.Value;
            config.settings["SEMISOLID_CLIPS_REQUIRED"] = chk_Semisolid.IsChecked.Value;
            config.settings["BLOCK_CLIPS_REQUIRED"] = chk_Block.IsChecked.Value;
            config.settings["PLURKWOOD_REACHABLE"] = chk_Plurkwood.IsChecked.Value;
            config.settings["POST_GAME_ALLOWED"] = chk_Postgame.IsChecked.Value;
            config.settings["POST_IRISU_ALLOWED"] = chk_Irisu.IsChecked.Value;
            config.settings["HALLOWEEN_REACHABLE"] = chk_Halloween.IsChecked.Value;
            config.settings["WARP_DESTINATION_REACHABLE"] = chk_WarpDestination.IsChecked.Value;
            config.settings["EVENT_WARPS_REQUIRED"] = chk_EventWarps.IsChecked.Value;

            config.additional_items = lists[0].ToArray();
            config.to_shuffle = lists[1].ToArray();
            config.must_be_reachable = lists[2].ToArray();

            try
            {
                FileIO.WriteConfig(s_configPath, config);
                MessageBox.Show("Config saved to path: " + s_configPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured.\n\nDetails: " + ex.Message);
                throw;
            }
        }
    }
}
