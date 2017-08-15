using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

            string output = FileIO.CallRandomizer(parameters, settings);

            MessageBox.Show(output);

            //ConfigData data = FileIO.ReadConfig("config.txt");
            //FileIO.WriteConfig("config2.txt", data);
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
    }
}
