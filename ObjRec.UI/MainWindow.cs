using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using ObjRec.Core.Algorithms;
using ObjRec.Core.Filters;
using ObjRec.Core.Filters.Sobel;
using SiftLib;

namespace ObjRec.UI
{
    public partial class MainWindow : Form
    {
        private readonly OpenFileDialog fileDialog = new OpenFileDialog {Filter = @"Picture Files (.bmp)|*.bmp|All Files (*.*)|*.*"};

        private int sz = 60;

        List<Feature> manFeatures;

        public MainWindow()
        {
            InitializeComponent();

            fileDialog.FileOk += (sender, args) =>
            {
                filenameTextbox.Text = fileDialog.FileName;

                LoadFile(filenameTextbox.Text);
            };

            manFeatures = new Sift().SiftFeatures(new Image<Gray, float>((Bitmap) Image.FromFile("man.png")));
        }

        private void LoadFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    sourcePic.Image = Image.FromFile(fileName);
                    processedPic.Image = new Bitmap(sourcePic.Image);
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        @"Invalid image format. Select another file",
                        @"Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            }

        private void loadFileButton_Click(object sender, System.EventArgs e)
        {
            LoadFile(filenameTextbox.Text);
        }

        private void filenameTextbox_Click(object sender, System.EventArgs e)
        {
            fileDialog.Reset();
            fileDialog.ShowDialog();
        }

        private async void OtsuFilter_Click(object sender, System.EventArgs e)
        {
            var filter = new ThresholdOtsuFilter();

            statusBarText.Text = @"Applying Otsu filter...";

            sourcePic.Image = new Bitmap(processedPic.Image);
            processedPic.Image = await filter.Apply(processedPic.Image);

            statusBarText.Text = $"Ready (Computed threshold : {filter.Threshold})";
        }

        private async void medianButton_Click(object sender, System.EventArgs e)
        {
            var filter = new MedianFilter();

            statusBarText.Text = @"Applying Median filter...";

            sourcePic.Image = new Bitmap(processedPic.Image);
            processedPic.Image = await filter.Apply(processedPic.Image);

            statusBarText.Text = @"Ready";
        }

        private async void sobelButton_Click(object sender, EventArgs e)
        {
            var filter = new SobelFilter();

            statusBarText.Text = @"Applying Sobel filter...";

            sourcePic.Image = new Bitmap(processedPic.Image);
            processedPic.Image = await filter.Apply(processedPic.Image);

            statusBarText.Text = @"Ready";
        }

        private async void sharpButton_Click(object sender, EventArgs e)
        {
            var filter = new SharpnessFilter();

            statusBarText.Text = @"Applying Sharpness filter...";

            sourcePic.Image = new Bitmap(processedPic.Image);
            processedPic.Image = await filter.Apply(processedPic.Image);

            statusBarText.Text = @"Ready";
        }

        private async void siftButton_Click(object sender, EventArgs e)
        {
            var siftAlg = new Sift();

            statusBarText.Text = @"Applying SIFT algorithm...";

            sourcePic.Image = new Bitmap(processedPic.Image);

            Image<Gray, float> image = new Image<Gray, float>(new Bitmap(processedPic.Image));

            List<Feature> features = null;

            await Task.Run(() => features = siftAlg.SiftFeatures(image));

            var pairs = new Dictionary<Feature, Feature>();

            foreach (var mfeat in manFeatures)
            {
                var dists = features.Select(f => new {dist = DistDesc(f, mfeat), feat = f}).Where(x => x.dist < 0.2).OrderBy(x => x.dist).FirstOrDefault();
                if (dists != null)
                    pairs.Add(mfeat, dists.feat);
            }

            var points = pairs.ToDictionary(
                p => new Point(Convert.ToInt32(p.Key.x), Convert.ToInt32(p.Key.y)),
                p => new Point(Convert.ToInt32(p.Value.x), Convert.ToInt32(p.Value.y)));

            var trans = Ransac.ApplyRansac(points);

            DrawAllDescOnProcessedPic(pairs);

            statusBarText.Text = @"Ready";
        }

        private float DistEucl(Feature a, Feature b)
        {
            return (float)Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
        }

        private float DistDesc(Feature a, Feature b)
        {
            double sum = 0;
            for (int i = 0; i < 128; i++)
            {
                sum += Math.Pow(a.descr[i] - b.descr[i], 2);
            }
            return (float) Math.Sqrt(sum);
        }

        private void DrawAllDescOnProcessedPic(Dictionary<Feature, Feature> pairs)
        {
            var g = Graphics.FromImage(processedPic.Image);
            var features = pairs.Values.ToList();

            var drawnFeat = new List<Feature>();

            foreach (var feature in features)
            {
                var feature1 = feature;
                if (drawnFeat.All(f => DistEucl(f, feature1) > sz / 1.2))
                    DrawDesc(g, feature);
                else
                    g.DrawEllipse(Pens.Red, (float)feature.x - sz / 30.0f, (float)feature.y - sz / 30.0f, sz / 30.0f, sz / 30.0f);

                drawnFeat.Add(feature);
            }

            processedPic.Refresh();
        }

        private void DrawDesc(Graphics g, Feature feature)
        {
            g.DrawRectangle(Pens.Chartreuse, (float)feature.x - sz / 2.0f, (float)feature.y - sz / 2.0f, sz, sz);

            var startx = (float) feature.x - sz/2.0f;
            var starty = (float) feature.y - sz/2.0f;

            var tr = new[]
            {
                new[] {0.0f, -sz/8.0f},
                new[] {sz/8.0f, -sz/8.0f},
                new[] {sz/8.0f, 0.0f},
                new[] {sz/8.0f, sz/8.0f},
                new[] {0.0f, sz/8.0f},
                new[] {-sz/8.0f, sz/8.0f},
                new[] {-sz/8.0f, 0.0f},
                new[] {-sz/8.0f, -sz/8.0f}
            };

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var cx = startx + sz / 4.0f * j;
                    var cy = starty + sz / 4.0f * i;

                    g.DrawRectangle(Pens.Chartreuse, cx, cy, sz / 4.0f, sz / 4.0f);

                    var ccx = cx + sz/8.0f;
                    var ccy = cy + sz/8.0f;
                    for (int k = 0; k < 8; k++)
                    {
                        var di = (4*j + i)*8 + k;
                        g.DrawLine(
                            Pens.Blue,
                            ccx,
                            ccy,
                            ccx + (float) Math.Floor(tr[k][0]*feature.descr[di]*5),
                            ccy + (float) Math.Floor(tr[k][1]*feature.descr[di]*5));
                    }
                }
                
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            sz = (int) siftDescSize.Value;
        }

        private void samplePic1_Click(object sender, EventArgs e)
        {

        }
    }
}
