using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.MachineLearning;
using Accord.Math;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ObjRec.Core.Algorithms;
using ObjRec.Core.Filters;
using ObjRec.Core.Filters.Sobel;
using SiftLib;

namespace ObjRec.UI
{
    public partial class MainWindow : Form
    {
        private readonly OpenFileDialog fileDialog = new OpenFileDialog { Filter = @"Picture Files (.bmp)|*.bmp|All Files (*.*)|*.*" };
        private readonly List<Image> modelImages = Directory.EnumerateFiles("./models", "*.jpg").Union(Directory.EnumerateFiles("./models", "*.png")).Select(Image.FromFile).ToList();
        private readonly List<Image> rotatedImages = new List<Image>();

        private KMeans kmeans;
        private RBF rbf;

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

            ApplySift(new Bitmap(100, 100), new Bitmap(100, 100)).MakeTransparent();
        }

        private void ModelImages(List<Image> images)
        {
            //listView1.SmallImageList = new ImageList { ImageSize = new Size(200, 200) };
            //listView1.SmallImageList.Images.AddRange(images.ToArray());

            //listView1.Items.Clear();
            //listView1.Items.AddRange(images.Select((image, i) => new ListViewItem { ImageIndex = i}).ToArray());
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
            var processed = new Image<Gray, byte>((Bitmap)processedPic.Image);

            rotatedImages.Clear();
            foreach (var modelImage in modelImages)
            {
                var img = ApplySift(new Image<Gray, byte>((Bitmap) modelImage), processed);
                if (img != null)
                    rotatedImages.Add(img.Bitmap);
            }

            ModelImages(rotatedImages);

            var descs = modelImages.Select(i => GetDescriptors(i.ToImage())).Aggregate((x, d) => d.ConcateVertical(x)).Data.ToDouble().ToJaggedArray();

            kmeans = new KMeans(descs.Length / modelImages.Count / 15);
            kmeans.Compute(descs);

            var picDescs = rotatedImages.Select(i => MakePicDescriptor(i.ToImage())).ToList();
           
            //for (var i = 0; i < listView1.Items.Count; i++)
            //{
            //    listView1.Items[i].Text = string.Join(Environment.NewLine,
            //        picDescs[i].Select(a => string.Join(" ", a.Select(b => $"{b:F0}"))));
            //}

            //listView1.Refresh();

            //DrawAllDescOnProcessedPic(pairs);

            statusBarText.Text = @"Ready";
        }

        private List<float[]> ToFloatsList(List<double[][]> picDescs)
        {
            var traindedData = new List<float[]>();
            foreach (var descr in picDescs)
            {
                IEnumerable<float> a = new List<float>();
                //descr.Select(x => x.Concat(x.ToList()));
                foreach (var item in descr)
                {
                    a = a.Concat(item.Select(x => (float)x));
                }
                traindedData.Add(a.ToArray());
            }
            return traindedData;
        }

        private Dictionary<float, List<float[]>> ReadTrainImages()
        {
            Dictionary<float, List<float[]>> descrForTrainDictionary = new Dictionary<float, List<float[]>>();

            List<Image> bikeImages = Directory.EnumerateFiles("./test/bike", "*.jpg").Union(Directory.EnumerateFiles("./test/bike", "*.png")).Select(Image.FromFile).ToList();
            var bikeDescriptors = bikeImages.Select(i => MakePicDescriptor(i.ToImage())).ToList();
            descrForTrainDictionary.Add(0, ToFloatsList(bikeDescriptors));

            List<Image> crutchImages = Directory.EnumerateFiles("./test/crutch", "*.jpg").Union(Directory.EnumerateFiles("./test/crutch", "*.png")).Select(Image.FromFile).ToList();
            var crutchDescriptors = crutchImages.Select(i => MakePicDescriptor(i.ToImage())).ToList();
            descrForTrainDictionary.Add(1, ToFloatsList(crutchDescriptors));

            List<Image> houseImages = Directory.EnumerateFiles("./test/house", "*.jpg").Union(Directory.EnumerateFiles("./test/house", "*.png")).Select(Image.FromFile).ToList();
            var houseDescriptors = houseImages.Select(i => MakePicDescriptor(i.ToImage())).ToList();
            descrForTrainDictionary.Add(2, ToFloatsList(houseDescriptors));


            return descrForTrainDictionary;
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

        public Image<Bgr, byte> ApplySift(Image<Gray, byte> modelImage, Image<Gray, byte> observedImage)
        {
            HomographyMatrix homography = null;

            var surfCpu = new SURFDetector(500, false);

            const int k = 2;
            const double uniquenessThreshold = 0.8d;

            var modelKeyPoints = surfCpu.DetectKeyPointsRaw(modelImage, null);
            Matrix<float> modelDescriptors = surfCpu.ComputeDescriptorsRaw(modelImage, null, modelKeyPoints);

            var observedKeyPoints = surfCpu.DetectKeyPointsRaw(observedImage, null);
            Matrix<float> observedDescriptors = surfCpu.ComputeDescriptorsRaw(observedImage, null, observedKeyPoints);
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
            if (nonZeroCount >= 19)
            {
                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                if (nonZeroCount >= 19)
                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
            }

            var result = new Image<Bgr, byte>(observedImage.Bitmap);

            //Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
            //    indices, new Bgr(255, 255, 255), new Bgr(255, 255, 255), mask, Features2DToolbox.KeypointDrawType.DEFAULT);

            if (homography != null)
            {
                //Rectangle rect = modelImage.ROI;
                //PointF[] pts = {
                //    new PointF(rect.Left, rect.Bottom),
                //    new PointF(rect.Right, rect.Bottom),
                //    new PointF(rect.Right, rect.Top),
                //    new PointF(rect.Left, rect.Top)
                //};
                //homography.ProjectPoints(pts);

                //var rounded = Array.ConvertAll(pts, Point.Round);

                var invMat = homography.Clone();
                CvInvoke.cvInvert(homography.Ptr, invMat.Ptr, SOLVE_METHOD.CV_LU);
                result = result.WarpPerspective(invMat, INTER.CV_INTER_LINEAR, WARP.CV_WARP_FILL_OUTLIERS, new Bgr(0, 0, 0));
                result = result.GetSubRect(modelImage.ROI);
            }
            else
            {
                return null;
            }

            return result;
        }

        private int GetQuarter(MKeyPoint point, RectangleF roi)
        {
            var q1 = RectangleF.FromLTRB(roi.Left, roi.Top, roi.Right / 4.0f, roi.Bottom / 4.0f);
            var q2 = RectangleF.FromLTRB(roi.Left / 4.0f, roi.Top, roi.Right, roi.Bottom / 4.0f);
            var q3 = RectangleF.FromLTRB(roi.Left, roi.Top / 4.0f, roi.Right / 4.0f, roi.Bottom);
            var q4 = RectangleF.FromLTRB(roi.Left / 4.0f, roi.Top, roi.Right / 4.0f, roi.Bottom);

            if (q1.Contains(point.Point))
                return 1;
            if (q2.Contains(point.Point))
                return 2;
            if (q3.Contains(point.Point))
                return 3;
            if (q4.Contains(point.Point))
                return 4;

            return 0;
        }

        private int GetOctal(MKeyPoint point, RectangleF roi)
        {
            var q1 = RectangleF.FromLTRB(roi.Left, roi.Top, roi.Right / 4.0f, roi.Bottom / 4.0f);
            var q2 = RectangleF.FromLTRB(roi.Left / 4.0f, roi.Top, roi.Right, roi.Bottom / 4.0f);
            var q3 = RectangleF.FromLTRB(roi.Left, roi.Top / 4.0f, roi.Right / 4.0f, roi.Bottom);
            var q4 = RectangleF.FromLTRB(roi.Left / 4.0f, roi.Top, roi.Right / 4.0f, roi.Bottom);

            int q = GetQuarter(point, roi);
            switch (q)
            {
            case 1:
                return 4 + GetQuarter(point, q1);
            case 2:
                return 8 + GetQuarter(point, q2);
            case 3:
                return 12 + GetQuarter(point, q3);
            case 4:
                return 16 + GetQuarter(point, q4);
            default:
                return 0;
            }
        }

        private IEnumerable<IEnumerable<MKeyPoint>> SplitByQuarter(VectorOfKeyPoint ocutKeyPoints, Rectangle roi)
        {
            return Enumerable.Range(0, 21)
                .Select(i => ocutKeyPoints.ToArray().Where(kp => GetPart(i, kp, roi) == i));
        }

        private int GetPart(int i, MKeyPoint ocutKeyPoint, Rectangle roi)
        {
            if (i < 1)
                return 0;

            if (i < 5)
            {
                return GetQuarter(ocutKeyPoint, roi);
            }

            return GetOctal(ocutKeyPoint, roi);
        }

        private double[][] MakePicDescriptor(Image<Bgr, byte> image)
        {
            var surfCpu = new SURFDetector(500, false);

            VectorOfKeyPoint ocutKeyPoints = surfCpu.DetectKeyPointsRaw(new Image<Gray, byte>(image.Bitmap), null);

            Matrix<float> ocutDescriptors = surfCpu.ComputeDescriptorsRaw(new Image<Gray, byte>(image.Bitmap), null, ocutKeyPoints);

            var ar = ocutKeyPoints.ToArray().ToList();

            var m = ocutDescriptors.GetRow(ar.IndexOf(ocutKeyPoints[1]));

            var gr = SplitByQuarter(ocutKeyPoints, image.ROI)
                .Select(kps => kps.Select(kp => ocutDescriptors.GetRow(ar.IndexOf(kp))))
                .Select(kps =>
                kps.Any() ? kps.Aggregate((a, r) =>
                a.ConcateVertical(r)) : new Matrix<float>(1, m.Cols));

            return gr.Select(GetDistribution).ToArray();
        }

        private double[] GetDistribution(Matrix<float> descriptors)
        {
            int[] assign = kmeans.Clusters.Nearest(descriptors.Data.ToDouble().ToJaggedArray());

            return Enumerable.Range(0, kmeans.K)
                    .Select(c => (double)assign.Count(a => a == c))
                    .ToArray();
        }

        private Matrix<float> GetDescriptors(Image<Bgr, byte> image)
        {
            var surfCpu = new SURFDetector(500, false);

            VectorOfKeyPoint ocutKeyPoints = surfCpu.DetectKeyPointsRaw(new Image<Gray, byte>(image.Bitmap), null);

            return surfCpu.ComputeDescriptorsRaw(new Image<Gray, byte>(image.Bitmap), null, ocutKeyPoints);
        }

        private void trainRBFButton_Click(object sender, EventArgs e)
        {
            var traneData = ReadTrainImages();
            rbf = new RBF();
            rbf.RunTraining(traneData);
        }

        private void determineClassButton_Click(object sender, EventArgs e)
        {
            List<Image> testImages = Directory.EnumerateFiles("./test/test", "*.jpg").Union(Directory.EnumerateFiles("./test/test", "*.png")).Select(Image.FromFile).ToList();
            var testDescriptors = testImages.Select(i => MakePicDescriptor(i.ToImage())).ToList();
            var search = ToFloatsList(testDescriptors);
            var className = rbf.DetermineClass(search.First());
            determinedClassText.Text = ToClassNameString(className);
        }

        private string ToClassNameString(float a)
        {
            var i = (int) a;
            switch (i)
            {
                case 0:return "Bike";
                case 1: return "Crutch";
                case 2: return "House";
            }
            return null;
        }

        private void filenameTextbox_TextChanged(object sender, EventArgs e)
        {

        }
    }

    internal static class ExtensionMethods
    {
        internal static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
        {
            int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
            int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
            int numberOfRows = rowsLastIndex + 1;

            int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
            int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
            int numberOfColumns = columnsLastIndex + 1;

            T[][] jaggedArray = new T[numberOfRows][];
            for (int i = rowsFirstIndex; i <= rowsLastIndex; i++)
            {
                jaggedArray[i] = new T[numberOfColumns];

                for (int j = columnsFirstIndex; j <= columnsLastIndex; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i, j];
                }
            }
            return jaggedArray;
        }

        internal static Image<Bgr, byte> ToImage(this Image image)
        {
            return new Image<Bgr, byte>((Bitmap)image);
        }
    }
}
