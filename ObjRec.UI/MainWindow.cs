using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ObjRec.Core.Filters;
using ObjRec.Core.Filters.Sobel;

namespace ObjRec.UI
{
    public partial class MainWindow : Form
    {
        private readonly OpenFileDialog fileDialog = new OpenFileDialog {Filter = @"Picture Files (.bmp)|*.bmp|All Files (*.*)|*.*"};

        public MainWindow()
        {
            InitializeComponent();

            fileDialog.FileOk += (sender, args) =>
            {
                filenameTextbox.Text = fileDialog.FileName;
            };
        }

        private void loadFileButton_Click(object sender, System.EventArgs e)
        {
            if (File.Exists(filenameTextbox.Text))
            {
                sourcePic.Image = Image.FromFile(filenameTextbox.Text);
            }
        }

        private void filenameTextbox_Click(object sender, System.EventArgs e)
        {
            fileDialog.Reset();
            fileDialog.ShowDialog();
        }

        private async void OtsuFilter_Click(object sender, System.EventArgs e)
        {
            //var filter = new ThresholdOtsuFilter();

            statusBarText.Text = "Applying Otsu filter...";

            //processedPic.Image = await filter.Apply(sourcePic.Image);

            statusBarText.Text = "Ready";
        }

        private async void button_sobel_Click(object sender, System.EventArgs e)
        {
            var filter = new SobelFilter();

            statusBarText.Text = "Applying Sobel filter...";

            processedPic.Image = await filter.Apply(sourcePic.Image);

            statusBarText.Text = "Ready";
        }
    }
}
