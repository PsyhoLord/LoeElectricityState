using System.Windows.Controls;

namespace LoeApp.Controls
{
    /// <summary>
    /// Interaction logic for DescriptionControl.xaml
    /// </summary>
    public partial class DescriptionControl : UserControl
    {
        public DescriptionControl()
        {
            InitializeComponent();
        }

        public void SetTitle(string title)
        {
            Title.Content = title;
        }

        public void SetDescription(string description)
        {
            Description.Text = description;
        }
    }
}
