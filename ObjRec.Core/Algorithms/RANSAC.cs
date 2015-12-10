using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ObjRec.Core.Algorithms
{
    public class Ransac
    {
        public static Transformation ApplyRansac(Dictionary<PointF, PointF> c, int nbMaxIteration = 200, int nbSuffisantInliers = 10000)
        {
            int nbTentative = 0;
            int maxInliers = 0;
            var best = new Transformation();

            var matchs = new Dictionary<PointF, PointF>();
            int nbMatch = c.Count;
            int nbCombi = (nbMatch * nbMatch - 1) / 2;
            int nbMin = Math.Min(nbCombi, nbMaxIteration);

            var tirages = new List<Point>();

            if (nbCombi < 200000)
            {
                for (int i = 0; i < nbMatch; i++)
                {
                    for (int j = 0; j < nbMatch; j++)
                    {
                        if (i > j)
                        {
                            tirages.Add(new Point(i, j));
                        }
                    }
                }
            }
            else
            {
                var rand = new Random();
                while (tirages.Count < 1000)
                {
                    var rand1 = rand.Next(nbMatch);
                    var rand2 = rand.Next(nbMatch);
                    var randCoord = new Point(rand1, rand2);
                    if (!tirages.Contains(randCoord))
                        tirages.Add(randCoord);
                }
            }

            while (nbTentative < nbMin)
            {
                var nbInliers = 0;
                matchs.Clear();

                var rand = new Random();
                int rando = rand.Next(tirages.Count);
                int rand1 = tirages[rando].X;
                int rand2 = tirages[rando].Y;
                tirages.RemoveAt(rando);
                nbTentative++;
                PointF p1 = c.Keys.ElementAt(rand1);
                PointF p2 = c.Keys.ElementAt(rand2);
                var lineFromSource = new LineSegment2DF(p1, p2);
                var lineFromPattern = new LineSegment2DF(c[p1], c[p2]);

                if (lineFromPattern.Length < 5 || lineFromSource.Length < 5)
                {
                    nbTentative++;
                    continue;
                }

                matchs.Add(p1, c[p1]);
                matchs.Add(p2, c[p2]);
                
                float translationX = lineFromSource.P1.X - lineFromPattern.P1.X;
                float translationY = lineFromSource.P1.Y - lineFromPattern.P1.Y;
                double scale = lineFromSource.Length / lineFromPattern.Length;

                if (scale > 4 || scale < 0.2)
                {
                    nbTentative++;
                    continue;
                }

                double rotation = lineFromSource.GetExteriorAngleDegree(lineFromPattern);

                var matRot = new Matrix<float>(2, 3);
                CvInvoke.cv2DRotationMatrix(lineFromPattern.P1, rotation, scale, matRot.Ptr);

                for (int i = 0; i < nbMatch; i++)
                {
                    if (i == rand1 || i == rand2)
                    {
                        continue;
                    }

                    bool skip = false;
                    foreach (var match in matchs.Keys)
                    {
                        var distValid1 = new LineSegment2DF(match, c.Keys.ElementAt(i));
                        var distValid2 = new LineSegment2DF(c[match], c[c.Keys.ElementAt(i)]);

                        if (distValid1.Length < 5 || distValid2.Length < 5)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip)
                        continue;

                    PointF pi = c.Keys.ElementAt(i);
                    var point1 = new Matrix<float>(3, 1)
                    {
                        [0, 0] = pi.X,
                        [1, 0] = pi.Y,
                        [2, 0] = 1
                    };

                    Matrix<float> point2 = matRot * point1;
                    point2[0, 0] += translationX;
                    point2[1, 0] += translationY;

                    var point3 = new Matrix<float>(2, 1)
                    {
                        [0, 0] = c[pi].X,
                        [1, 0] = c[pi].Y
                    };

                    double dist = Math.Sqrt(Math.Pow(point2[0, 0] - point3[0, 0], 2) + Math.Pow(point2[1, 0] - point3[1, 0], 2));

                    if (dist < 10)
                    {
                        nbInliers++;
                        matchs.Add(pi, c[pi]);

                        if (nbInliers > maxInliers)
                        {
                            maxInliers = nbInliers;
                            best.MatRotScale = matRot;
                            best.InlinersCount = nbInliers;
                            best.Rotation = rotation;
                            best.Scale = scale;
                            best.TranslationX = translationX;
                            best.TranslationY = translationY;
                            best.Matches = matchs.Keys.ToDictionary(p => p, p => matchs[p]);
                        }
                    }

                    if (nbInliers > nbSuffisantInliers)
                    {
                        break;
                    }
                }

                nbTentative++;
            }

            tirages.Clear();

            return best;
        }
    }

    public class Transformation
    {
        public Matrix<float> MatRotScale { get; set; }
        public int InlinersCount { get; set; }
        public double Rotation { get; set; }
        public double Scale { get; set; }
        public double TranslationX { get; set; }
        public double TranslationY { get; set; }

        public Dictionary<PointF, PointF> Matches { get; set; }
    }
}
