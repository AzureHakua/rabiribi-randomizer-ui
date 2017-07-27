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
            string output = FileIO.CallRandomizer(
                new Dictionary<string, object> {
                    { "seed", 12421 },
                },
                new HashSet<string>
                {

                }
            );

            Console.WriteLine(output);

            //ConfigData data = FileIO.ReadConfig("config.txt");
            //FileIO.WriteConfig("config2.txt", data);
        }
    }
}
