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
            /// <summary>
            /// RANSAC : recherche de la meilleur transformation parmis un ensemble de mise en correspondance
            /// </summary>
            /// <param name="c">Dictionnaire de mise en correspondance des points</param>
            /// <param name="nbMaxIteration">Condition d'arrêt : nombre maximal de tests</param>
            /// <param name="nbSuffisantInliers">Condition d'arrêt : si on trouve suffisament d'inliers on arrête la recherche</param>
            /// <returns></returns>
            public static Transformation ApplyRansac(Dictionary<Point, Point> c, int nbMaxIteration = 200, int nbSuffisantInliers = 10000)
            {
                int nbTentative = 0;
                int maxInliers = 0;
                var best = new Transformation();

                //création d'un sous-dictionnaire contenant les match des inliers
                Dictionary<Point, Point> matchs = new Dictionary<Point, Point>();
                int nbMatch = c.Count;
                // le nombre de combinaisons possible est : (matchedFeatures.Length * matchedFeatures.Length -1) / 2
                int nbCombi = (nbMatch * nbMatch - 1) / 2;
                // si le nombre de combinaison est plus petit que la limite du nombre d'itération fixé on ne fera que nbCombi tests
                int nbMin = Math.Min(nbCombi, nbMaxIteration);

                //Liste stockant les tirages aléatoires
                List<Point> tirages = new List<Point>();

                //si nbCombi n'est pas trop grand on enumère toute les possibilité puis on en tirera au hasard
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

                //si nbCombi est très suppérieur à 1000 on peut tirer des valeurs au hasard 
                else
                {
                    Random rand = new Random();
                    int rand1, rand2;
                    while (tirages.Count < 1000)
                    {
                        rand1 = rand.Next(nbMatch);
                        rand2 = rand.Next(nbMatch);
                        Point randCoord = new Point(rand1, rand2);
                        if (!tirages.Contains(randCoord))
                            tirages.Add(randCoord);
                    }
                }

                //on cherche la meilleurs trasformation (celle qui a le plus d'inliers), on arrête de chercher après nbMin tentatives 
                while (nbTentative < nbMin)
                {
                    var nbInliers = 0;
                    matchs.Clear();

                    //on prend deux points au hasard ainsi que leurs correspondances
                    Random rand = new Random();
                    int rando = rand.Next(tirages.Count);
                    int rand1 = tirages[rando].X;
                    int rand2 = tirages[rando].Y;
                    tirages.RemoveAt(rando);
                    nbTentative++;
                    Point p1 = c.Keys.ElementAt(rand1);
                    Point p2 = c.Keys.ElementAt(rand2);
                    LineSegment2DF ligneRequete = new LineSegment2DF(p1, p2);
                    LineSegment2DF ligneModel = new LineSegment2DF(c[p1], c[p2]);

                    //test optionnel : si la longeur des vecteur est trop petite, cela risque de donner un scale et une rot faussés
                    if (ligneModel.Length < 5 || ligneRequete.Length < 5)
                    {
                        nbTentative++;
                        continue;
                    }

                    matchs.Add(p1, c[p1]);
                    matchs.Add(p2, c[p2]);

                    //calculer translation, Rotation et scale
                    float translationX = ligneRequete.P1.X - ligneModel.P1.X;
                    float translationY = ligneRequete.P1.Y - ligneModel.P1.Y;
                    double scale = ligneRequete.Length / ligneModel.Length;

                    //test optionnel : si le scale est trop petit ou trop grand je saute la possibilité car dans mon cas ça ne peut pas arriver
                    if (scale > 4 || scale < 0.2)
                    {
                        nbTentative++;
                        continue;
                    }

                    double rotation = ligneRequete.GetExteriorAngleDegree(ligneModel);

                    //création de la matrice de transformation
                    Matrix<float> matRot = new Matrix<float>(2, 3);
                    CvInvoke.cv2DRotationMatrix(ligneModel.P1, rotation, scale, matRot.Ptr);

                    //pour chaque mise en correspondance
                    for (int i = 0; i < nbMatch; i++)
                    {
                        if (i == rand1 || i == rand2)
                        {
                            continue;
                        }

                        // test optionnel : si le point que l'on souhaite valider est trop proche d'un point déjà validé du modèle, il n'apporte pas d'information supplémentaire on le saute
                        // idem pour sa mise en correspondance 
                        bool skip = false;
                        foreach (Point match in matchs.Keys)
                        {
                            LineSegment2DF distValid1 = new LineSegment2DF(match, c.Keys.ElementAt(i));
                            LineSegment2DF distValid2 = new LineSegment2DF(c[match], c[c.Keys.ElementAt(i)]);
                            //distance min entre 2 points : 5 pixels (sift prend parfois des points très proches)
                            if (distValid1.Length < 5 || distValid2.Length < 5)
                            {
                                skip = true;
                                break;
                            }
                        }
                        if (skip) continue;

                        //on transforme le point du modèle
                        Point pi = c.Keys.ElementAt(i);
                        Matrix<float> point1 = new Matrix<float>(3, 1);
                        point1[0, 0] = pi.X;
                        point1[1, 0] = pi.Y;
                        point1[2, 0] = 1;

                        Matrix<float> point2 = matRot * point1;
                        point2[0, 0] += translationX;
                        point2[1, 0] += translationY;

                        Matrix<float> point3 = new Matrix<float>(2, 1);
                        point3[0, 0] = c[pi].X;
                        point3[1, 0] = c[pi].Y;

                        //on calcul la distance entre le point transformé et sa correspondance
                        double dist = Math.Sqrt((point2[0, 0] - point3[0, 0]) * (point2[0, 0] - point3[0, 0]) + (point2[1, 0] - point3[1, 0]) * (point2[1, 0] - point3[1, 0]));

                        //s'il tombe suffisament près (ici à moins de 10 pixels) du point attendu, c'est un inliers
                        //on peut faire varier ce seuil suivant la précision souhaité
                        if (dist < 10)
                        {
                            nbInliers++;
                            matchs.Add(pi, c[pi]);

                            //on stock le max (correspond à la meilleur transformation)
                            if (nbInliers > maxInliers)
                            {
                                maxInliers = nbInliers;
                                best.MatRotScale = matRot;
                                best.InlinersCount = nbInliers;
                                best.Rotation = rotation;
                                best.Scale = scale;
                                best.TranslationX = translationX;
                                best.TranslationY = translationY;
                                best.matchs = matchs.Keys.ToDictionary(p => p, p => matchs[p]); ;
                            }
                        }

                        //condition d'arrêt supplémentaire : si le nombre d'inliers est supérieur à un seuil fixé
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

        public Dictionary<Point, Point> matchs { get; set; }
    }
}
