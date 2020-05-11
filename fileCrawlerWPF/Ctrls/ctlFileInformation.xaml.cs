using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace fileCrawlerWPF.Ctrls
{
    /// <summary>
    /// Interaction logic for ctlFileInformation.xaml
    /// </summary>
    public partial class ctlFileInformation : UserControl
    {
        public ProbeFile ProbeFile { get; set; }
        public static readonly DependencyProperty ProbeFileDependencyProperty =
            DependencyProperty.Register("ProbeFile", typeof(ProbeFile), typeof(ctlFileInformation), new PropertyMetadata());

        public ctlFileInformation()
        {
            InitializeComponent();
        }


        public string FileName { get { return ProbeFile.Name; } set { } }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
