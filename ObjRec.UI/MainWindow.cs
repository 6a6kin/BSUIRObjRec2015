using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using ObjRec.Core.Algorithms;
using ObjRec.Core.Filters;
using ObjRec.Core.Filters.Sobel;
using SiftLib;

namespace ObjRec.UI
{
    public partial class MainWindow : Form
    {
        private readonly OpenFileDialog fileDialog = new OpenFileDialog { Filter = @"Picture Files (.bmp)|*.bmp|All Files (*.*)|*.*" };
        private readonly List<Image> modelImages = Directory.EnumerateFiles("./models", "*.jpg").Select(Image.FromFile).ToList();
        private readonly List<Image> rotatedImages = new List<Image>();

        private int sz = 60;

        public MainWindow()
        {
            InitializeComponent();

            fileDialog.FileOk += (sender, args) =>
            {
                filenameTextbox.Text = fileDialog.FileName;

                LoadFile(filenameTextbox.Text);
            };

            ModelImages(modelImages);
        }

        private void ModelImages(List<Image> images)
        {
            listView1.SmallImageList = new ImageList { ImageSize = new Size(200, 200) };
            listView1.SmallImageList.Images.AddRange(images.ToArray());

            listView1.Items.Clear();
            listView1.Items.AddRange(images.Select((image, i) => new ListViewItem { ImageIndex = i }).ToArray());
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

        private void loadFileButton_Click(object sender, EventArgs e)
        {
            modelImages.Clear();
            modelImages.AddRange(Directory.EnumerateFiles("./models", "*.jpg").Select(Image.FromFile));

            ModelImages(modelImages);

            LoadFile(filenameTextbox.Text);
        }

        private void filenameTextbox_Click(object sender, EventArgs e)
        {
            fileDialog.Reset();
            fileDialog.ShowDialog();
        }

        private async void OtsuFilter_Click(object sender, EventArgs e)
        {
            var filter = new ThresholdOtsuFilter();

            statusBarText.Text = @"Applying Otsu filter...";

            sourcePic.Image = new Bitmap(processedPic.Image);
            processedPic.Image = await filter.Apply(processedPic.Image);

            statusBarText.Text = $"Ready (Computed threshold : {filter.Threshold})";
        }

        private async void medianButton_Click(object sender, EventArgs e)
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

        private void siftButton_Click(object sender, EventArgs e)
        {
            statusBarText.Text = @"Applying SIFT algorithm...";

            sourcePic.Image = new Bitmap(processedPic.Image);
            var processed = new Image<Gray, byte>((Bitmap) processedPic.Image);

            rotatedImages.Clear();
            foreach (var modelImage in modelImages)
            {
                rotatedImages.Add(ApplySift(new Image<Gray, byte>((Bitmap)modelImage), processed).Bitmap);
            }

            listView1.SmallImageList.Images.Clear();

            listView1.SmallImageList.Images.AddRange(rotatedImages.ToArray());
            listView1.Refresh();
            //DrawAllDescOnProcessedPic(pairs);

            statusBarText.Text = @"Ready";
        }

        private Bitmap ApplySift(Bitmap bigPic, Bitmap modelPic)
        {
            Image<Gray, float> bigImage = new Image<Gray, float>(new Bitmap(bigPic));
            Image<Gray, float> modelImage = new Image<Gray, float>(new Bitmap(modelPic));

            var siftAlg = new Sift();
            var bigPicFeatures = siftAlg.SiftFeatures(bigImage);
            var modelFeatures = siftAlg.SiftFeatures(modelImage);

            var pairs = new Dictionary<Feature, Feature>();

            foreach (var mfeat in modelFeatures)
            {
                var dists = bigPicFeatures
                    .Select(f => new { dist = DistDesc(f, mfeat), feat = f })
                    .Where(x => x.dist < 0.2)
                    .OrderBy(x => x.dist)
                    .FirstOrDefault();

                if (dists != null)
                    pairs.Add(mfeat, dists.feat);
            }

            var points = new Dictionary<PointF, PointF>();
            foreach (var pair in pairs)
            {
                var src = new PointF(Convert.ToSingle(pair.Key.x), Convert.ToSingle(pair.Key.y));
                var dst = new PointF(Convert.ToSingle(pair.Value.x), Convert.ToSingle(pair.Value.y));
                if (!points.ContainsKey(dst))
                    points.Add(dst, src);
            }

            var transform = Ransac.ApplyRansac(points);

            return bigImage.Rotate(transform.Rotation, new Gray(0)).Bitmap;
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
            return (float)Math.Sqrt(sum);
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

            var startx = (float)feature.x - sz / 2.0f;
            var starty = (float)feature.y - sz / 2.0f;

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

                    var ccx = cx + sz / 8.0f;
                    var ccy = cy + sz / 8.0f;
                    for (int k = 0; k < 8; k++)
                    {
                        var di = (4 * j + i) * 8 + k;
                        g.DrawLine(
                            Pens.Blue,
                            ccx,
                            ccy,
                            ccx + (float)Math.Floor(tr[k][0] * feature.descr[di] * 5),
                            ccy + (float)Math.Floor(tr[k][1] * feature.descr[di] * 5));
                    }
                }

            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            sz = (int)siftDescSize.Value;
        }

        public static Image<Bgr, byte> ApplySift(Image<Gray, byte> modelImage, Image<Gray, byte> observedImage)
        {
            HomographyMatrix homography = null;

            var surfCPU = new SURFDetector(500, false);

            const int k = 2;
            const double uniquenessThreshold = 0.8d;
            {
                //extract features from the object image
                var modelKeyPoints = surfCPU.DetectKeyPointsRaw(modelImage, null);
                Matrix<float> modelDescriptors = surfCPU.ComputeDescriptorsRaw(modelImage, null, modelKeyPoints);

                // extract features from the observed image
                var observedKeyPoints = surfCPU.DetectKeyPointsRaw(observedImage, null);
                Matrix<float> observedDescriptors = surfCPU.ComputeDescriptorsRaw(observedImage, null, observedKeyPoints);
                var matcher = new BruteForceMatcher<float>(DistanceType.L2);
                matcher.Add(modelDescriptors);

                var indices = new Matrix<int>(observedDescriptors.Rows, k);
                Matrix<byte> mask;
                using (var dist = new Matrix<float>(observedDescriptors.Rows, k))
                {
                    matcher.KnnMatch(observedDescriptors, indices, dist, k, null);
                    mask = new Matrix<byte>(dist.Rows, 1);
                    mask.SetValue(255);
                    Features2DToolbox.VoteForUniqueness(dist, uniquenessThreshold, mask);
                }

                int nonZeroCount = CvInvoke.cvCountNonZero(mask);
                if (nonZeroCount >= 4)
                {
                    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                    if (nonZeroCount >= 4)
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
                }
            }

            //Draw the matched keypoints
            var result = new Image<Bgr, byte>(observedImage.Bitmap);

            //Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
            //    indices, new Bgr(255, 255, 255), new Bgr(255, 255, 255), mask, Features2DToolbox.KeypointDrawType.DEFAULT);

            if (homography != null)
            {

                //draw a rectangle along the projected model
                Rectangle rect = modelImage.ROI;
                PointF[] pts = {
                    new PointF(rect.Left, rect.Bottom),
                    new PointF(rect.Right, rect.Bottom),
                    new PointF(rect.Right, rect.Top),
                    new PointF(rect.Left, rect.Top)
                };
                homography.ProjectPoints(pts);

                //var rounded = Array.ConvertAll(pts, Point.Round);
                
                var invMat = homography.Clone();
                CvInvoke.cvInvert(homography.Ptr, invMat.Ptr, SOLVE_METHOD.CV_LU);
                result = result.WarpPerspective(invMat, INTER.CV_INTER_LINEAR, WARP.CV_WARP_FILL_OUTLIERS, new Bgr(0, 0, 0));
                result = result.GetSubRect(modelImage.ROI);
            }

            return result;
        }
    }
}
