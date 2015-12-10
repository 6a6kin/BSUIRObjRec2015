using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace SiftLib
{
    public class Sift
    {
        private const double SiftInitSigma = .5;
        private const int SiftImgBorder = 5;
        private const int SiftMaxInterpSteps = 5;
        private const int SiftOriHistBins = 36;
        private const double SiftOriRadius = 3*SiftOriSigFctr;
        private const double SiftOriSigFctr = 1.5;
        private const int SiftOriSmoothPasses = 2;
        private const double SiftOriPeakRatio = .8;
        private const double SiftDescrSclFctr = 3.0;
        private const double SiftDescrMagThr = .2;
        private const double SiftIntDescrFctr = 512.0;

        private const int SiftIntvls = 3;
        private const double SiftSigma = 1.6;
        private const double SiftContrThr = 0.04;
        private const int SiftCurvThr = 10;
        private const int SiftImgDbl = 1;
        private const int SiftDescrWidth = 4;
        private const int SiftDescrHistBins = 8;

        public List<Feature> SiftFeatures(Image<Gray, float> img)
        {
            return _sift_features(img, SiftIntvls, SiftSigma, SiftContrThr,
                SiftCurvThr, SiftImgDbl, SiftDescrWidth,
                SiftDescrHistBins);
        }

        public List<Feature> _sift_features(Image<Gray, float> img, int intvls,
            double sigma, double contrThr, int curvThr,
            int imgDbl, int descrWidth, int descrHistBins)
        {
            Image<Gray, float> initImg;
            Image<Gray, float>[,] gaussPyr;
            Image<Gray, float>[,] dogPyr;
            initImg = create_init_img(img, imgDbl, sigma);
            List<Feature> feat = new List<Feature>();
            int octvs = (int) (Math.Log(Math.Min(initImg.Width, initImg.Height))/Math.Log(2) - 2);
            gaussPyr = build_gauss_pyr(initImg, octvs, intvls, sigma);
            dogPyr = build_dog_pyr(gaussPyr, octvs, intvls);
            var features = scale_space_extrema(dogPyr, octvs, intvls, contrThr, curvThr);
            calc_feature_scales(ref features, sigma, intvls);
            if (imgDbl != 0)
                adjust_for_img_dbl(ref features);

            calc_feature_oris(ref features, gaussPyr);
            compute_descriptors(ref features, gaussPyr, descrWidth, descrHistBins);
            features.Sort((a, b) =>
            {
                if (a.scl < b.scl) return 1;
                if (a.scl > b.scl) return -1;
                return 0;
            });
            int n = features.Count;
            feat = features;


            return feat;
        }

        private void compute_descriptors(ref List<Feature> features, Image<Gray, float>[,] gaussPyr, int d, int n)
        {
            int i, k = features.Count;

            for (i = 0; i < k; i++)
            {
                var feat = features[i];
                var ddata = feat.feature_data;
                var hist = descr_hist(gaussPyr[ddata.octv, ddata.intvl], ddata.R,
                    ddata.C, feat.ori, ddata.scl_octv, d, n);
                hist_to_descr(hist, d, n, ref feat);
                //release_descr_hist(&hist, d);
            }
        }

        private void hist_to_descr(float[,,] hist, int d, int n, ref Feature feat)
        {
            int i, r, c, o, k = 0;
            feat.descr = new double[d*d*n];

            for (r = 0; r < d; r++)
                for (c = 0; c < d; c++)
                    for (o = 0; o < n; o++)
                        feat.descr[k++] = hist[r, c, o];

            feat.d = k;
            normalize_descr(feat);
            for (i = 0; i < k; i++)
                if (feat.descr[i] > SiftDescrMagThr)
                    feat.descr[i] = SiftDescrMagThr;
            normalize_descr(feat);

            /* convert floating-point descriptor to integer valued descriptor */
            //for (i = 0; i < k; i++)
            //{
            //    var intVal = (int) (SiftIntDescrFctr*feat.descr[i]);
            //    feat.descr[i] = Math.Min(255, intVal);
            //}
        }

        private void normalize_descr(Feature feat)
        {
            double cur, lenInv, lenSq = 0.0;
            int i, d = feat.d;

            for (i = 0; i < d; i++)
            {
                cur = feat.descr[i];
                lenSq += cur*cur;
            }
            lenInv = 1.0/Math.Sqrt(lenSq);
            for (i = 0; i < d; i++)
                feat.descr[i] *= lenInv;
        }

        private float[,,] descr_hist(Image<Gray, float> img, int r, int c, double ori,
            double scl, int d, int n)
        {
            double histWidth,
                expDenom,
                rRot,
                cRot,
                gradMag,
                gradOri,
                w,
                rbin,
                cbin,
                obin,
                binsPerRad,
                pi2 = 2.0*Math.PI;
            int i, j;

            var hist = new float[d, d, n];

            var cosT = Math.Cos(ori);
            var sinT = Math.Sin(ori);
            binsPerRad = n/pi2;
            expDenom = d*d*0.5;
            histWidth = SiftDescrSclFctr*scl;
            var radius = (int) (histWidth*Math.Sqrt(2)*(d + 1.0)*0.5 + 0.5);
            for (i = -radius; i <= radius; i++)
                for (j = -radius; j <= radius; j++)
                {
                    /*
                    Calculate sample's histogram array coords rotated relative to ori.
                    Subtract 0.5 so samples that fall e.g. in the center of row 1 (i.e.
                    r_rot = 1.5) have full weight placed in row 1 after interpolation.
                    */
                    cRot = (j*cosT - i*sinT)/histWidth;
                    rRot = (j*sinT + i*cosT)/histWidth;
                    rbin = rRot + d/2 - 0.5;
                    cbin = cRot + d/2 - 0.5;

                    if (rbin > -1.0 && rbin < d && cbin > -1.0 && cbin < d)
                        if (calc_grad_mag_ori(img, r + i, c + j, out gradMag, out gradOri) != 0)
                        {
                            gradOri -= ori;
                            while (gradOri < 0.0)
                                gradOri += pi2;
                            while (gradOri >= pi2)
                                gradOri -= pi2;

                            obin = gradOri*binsPerRad;
                            w = Math.Exp(-(cRot*cRot + rRot*rRot)/expDenom);
                            interp_hist_entry(ref hist, rbin, cbin, obin, gradMag*w, d, n);
                        }
                }

            return hist;
        }

        private void interp_hist_entry(ref float[,,] hist, double rbin, double cbin,
            double obin, double mag, int d, int n)
        {
            float dR, dC, dO, vR, vC, vO;

            int r0, c0, o0, rb, cb, ob, r, c, o;

            r0 = (int) Math.Floor(rbin);
            c0 = (int) Math.Floor(cbin);
            o0 = (int) Math.Floor(obin);
            dR = (float) rbin - r0;
            dC = (float) cbin - c0;
            dO = (float) obin - o0;

            /*
            The entry is distributed into up to 8 bins.  Each entry into a bin
            is multiplied by a weight of 1 - d for each dimension, where d is the
            distance from the center value of the bin measured in bin units.
            */
            for (r = 0; r <= 1; r++)
            {
                rb = r0 + r;
                if (rb >= 0 && rb < d)
                {
                    vR = (float) mag*((r == 0) ? 1.0F - dR : dR);

                    for (c = 0; c <= 1; c++)
                    {
                        cb = c0 + c;
                        if (cb >= 0 && cb < d)
                        {
                            vC = vR*((c == 0) ? 1.0F - dC : dC);

                            for (o = 0; o <= 1; o++)
                            {
                                ob = (o0 + o)%n;
                                vO = vC*((o == 0) ? 1.0F - dO : dO);
                                hist[rb, cb, ob] += vO;
                            }
                        }
                    }
                }
            }
        }

        private void calc_feature_oris(ref List<Feature> features, Image<Gray, float>[,] gaussPyr)
        {
            Feature feat;
            DetectionData ddata;
            double[] hist;
            double omax;
            int i, j, n = features.Count;

            for (i = 0; i < n; i++)
            {
                feat = features[0];
                features.RemoveAt(0);
                ddata = feat.feature_data;
                hist = ori_hist(gaussPyr[ddata.octv, ddata.intvl],
                    ddata.R, ddata.C, SiftOriHistBins,
                    (int) Math.Round(SiftOriRadius*ddata.scl_octv),
                    SiftOriSigFctr*ddata.scl_octv);
                for (j = 0; j < SiftOriSmoothPasses; j++)
                    smooth_ori_hist(ref hist, SiftOriHistBins);
                omax = dominant_ori(ref hist, SiftOriHistBins);
                add_good_ori_features(ref features, hist, SiftOriHistBins,
                    omax*SiftOriPeakRatio, feat);
                //free( ddata );
                //free( feat );
                //free( hist );
            }
        }

        private void add_good_ori_features(ref List<Feature> features, double[] hist, int n,
            double magThr, Feature feat)
        {
            double bin, pi2 = Math.PI*2.0;
            int l, r, i;

            for (i = 0; i < n; i++)
            {
                l = (i == 0) ? n - 1 : i - 1;
                r = (i + 1)%n;

                if (hist[i] > hist[l] && hist[i] > hist[r] && hist[i] >= magThr)
                {
                    bin = i + interp_hist_peak(hist[l], hist[i], hist[r]);
                    bin = (bin < 0) ? n + bin : (bin >= n) ? bin - n : bin;
                    var newFeat = (Feature) feat.Clone();
                    newFeat.ori = ((pi2*bin)/n) - Math.PI;
                    features.Add(newFeat);
                }
            }
        }

        private double interp_hist_peak(double l, double c, double r)
        {
            return 0.5*((l) - (r))/((l) - 2.0*(c) + (r));
        }

        private double dominant_ori(ref double[] hist, int n)
        {
            double omax;
            int maxbin, i;

            omax = hist[0];
            maxbin = 0;
            for (i = 1; i < n; i++)
                if (hist[i] > omax)
                {
                    omax = hist[i];
                    maxbin = i;
                }
            return omax;
        }

        private void smooth_ori_hist(ref double[] hist, int n)
        {
            double prev, tmp, h0 = hist[0];
            int i;

            prev = hist[n - 1];
            for (i = 0; i < n; i++)
            {
                tmp = hist[i];
                hist[i] = 0.25*prev + 0.5*hist[i] +
                          0.25*((i + 1 == n) ? h0 : hist[i + 1]);
                prev = tmp;
            }
        }

        private double[] ori_hist(Image<Gray, float> img, int r, int c, int n, int rad, double sigma)
        {
            double[] hist;
            double mag, ori, w, expDenom, pi2 = Math.PI*2.0;
            int bin, i, j;

            hist = new double[n];
            expDenom = 2.0*sigma*sigma;
            for (i = -rad; i <= rad; i++)
                for (j = -rad; j <= rad; j++)
                    if (calc_grad_mag_ori(img, r + i, c + j, out mag, out ori) == 1)
                    {
                        w = Math.Exp(-(i*i + j*j)/expDenom);
                        bin = (int) Math.Round(n*(ori + Math.PI)/pi2);
                        bin = (bin < n) ? bin : 0;
                        hist[bin] += w*mag;
                    }

            return hist;
        }

        private int calc_grad_mag_ori(Image<Gray, float> img, int r, int c, out double mag, out double ori)
        {
            double dx, dy;

            if (r > 0 && r < img.Height - 1 && c > 0 && c < img.Width - 1)
            {
                dx = img[r, c + 1].Intensity - img[r, c - 1].Intensity;
                dy = img[r - 1, c].Intensity - img[r + 1, c].Intensity;
                mag = Math.Sqrt(dx*dx + dy*dy);
                ori = Math.Atan2(dy, dx);
                return 1;
            }

            else
            {
                mag = 0;
                ori = 0;
                return 0;
            }
        }

        private void adjust_for_img_dbl(ref List<Feature> features)
        {
            Feature feat;
            int i, n;

            n = features.Count;
            for (i = 0; i < n; i++)
            {
                feat = features[i];
                feat.x /= 2.0;
                feat.y /= 2.0;
                feat.scl /= 2.0;
                feat.img_pt.X /= 2.0F;
                feat.img_pt.Y /= 2.0F;
            }
        }

        private void calc_feature_scales(ref List<Feature> features, double sigma, int intvls)
        {
            Feature feat;
            double intvl;
            int i, n;

            n = features.Count;
            for (i = 0; i < n; i++)
            {
                feat = features[i];
                intvl = feat.feature_data.intvl + feat.feature_data.subintvl;
                feat.scl = sigma*Math.Pow(2.0, feat.feature_data.octv + intvl/intvls);
                feat.feature_data.scl_octv = sigma*Math.Pow(2.0, intvl/intvls);
            }
        }


        private Image<Gray, float> create_init_img(Image<Gray, float> img, int imgDbl, double sigma)
        {
            Image<Gray, float> gray;
            Image<Gray, float> dbl;
            float sigDiff;

            gray = convert_to_gray32(img);
            if (imgDbl != 0)
            {
                sigDiff = (float) Math.Sqrt(sigma*sigma - SiftInitSigma*SiftInitSigma*4);
                dbl = new Image<Gray, float>(new Size(img.Width*2, img.Height*2));
                dbl = gray.Resize(dbl.Width, dbl.Height, INTER.CV_INTER_CUBIC);
                dbl = dbl.SmoothGaussian(0, 0, sigDiff, sigDiff);
                return dbl;
            }
            else
            {
                sigDiff = (float) Math.Sqrt(sigma*sigma - SiftInitSigma*SiftInitSigma);
                gray.SmoothGaussian(0, 0, sigDiff, sigDiff);
                return gray;
            }
        }

        private Image<Gray, float> convert_to_gray32(Image<Gray, float> img)
        {
            Image<Gray, Byte> gray8;
            Image<Gray, float> gray32;

            gray32 = new Image<Gray, float>(img.Width, img.Height);

            using (gray8 = img.Convert<Gray, Byte>())
            {
                gray32 = gray8.ConvertScale<float>(1.0/255.0, 0);
            }


            return gray32;
        }

        private Image<Gray, float>[,] build_gauss_pyr(Image<Gray, float> basepic, int octvs,
            int intvls, double sigma)
        {
            Image<Gray, float>[,] gaussPyr = new Image<Gray, float>[octvs, intvls + 3];
            double[] sig = new double[intvls + 3];
            double sigTotal, sigPrev, k;
            int i, o;


            /*
                precompute Gaussian sigmas using the following formula:

                \sigma_{total}^2 = \sigma_{i}^2 + \sigma_{i-1}^2
            */
            sig[0] = sigma;
            k = Math.Pow(2.0, 1.0/intvls);
            for (i = 1; i < intvls + 3; i++)
            {
                sigPrev = Math.Pow(k, i - 1)*sigma;
                sigTotal = sigPrev*k;
                sig[i] = Math.Sqrt(sigTotal*sigTotal - sigPrev*sigPrev);
            }

            for (o = 0; o < octvs; o++)
                for (i = 0; i < intvls + 3; i++)
                {
                    if (o == 0 && i == 0)
                        gaussPyr[o, i] = basepic.Clone();

                    /* base of new octvave is halved image from end of previous octave */
                    else if (i == 0)
                        gaussPyr[o, i] = Downsample(gaussPyr[o - 1, intvls]);

                    /* blur the current octave's last image to create the next one */
                    else
                    {
                        gaussPyr[o, i] = gaussPyr[o, i - 1].SmoothGaussian(0, 0, sig[i], sig[i]);
                    }
                }


            return gaussPyr;
        }

        private Image<Gray, float> Downsample(Image<Gray, float> img)
        {
            return img.Resize(img.Width / 2, img.Height / 2, INTER.CV_INTER_NN);
        }

        private Image<Gray, float>[,] build_dog_pyr(Image<Gray, float>[,] gaussPyr, int octvs, int intvls)
        {
            Image<Gray, float>[,] dogPyr;
            int i, o;

            dogPyr = new Image<Gray, float>[octvs, intvls + 2];

            for (o = 0; o < octvs; o++)
                for (i = 0; i < intvls + 2; i++)
                {
                    dogPyr[o, i] = gaussPyr[o, i + 1].Sub(gaussPyr[o, i]);
                }

            return dogPyr;
        }

        private List<Feature> scale_space_extrema(Image<Gray, float>[,] dogPyr, int octvs, int intvls,
            double contrThr, int curvThr)
        {
            List<Feature> features = new List<Feature>();
            double prelimContrThr = 0.5*contrThr/intvls;
            Feature feat;
            DetectionData ddata;
            int o, i, r, c;


            for (o = 0; o < octvs; o++)
                for (i = 1; i <= intvls; i++)
                    for (r = SiftImgBorder; r < dogPyr[o, 0].Height - SiftImgBorder; r++)
                        for (c = SiftImgBorder; c < dogPyr[o, 0].Width - SiftImgBorder; c++)
                            /* perform preliminary check on contrast */
                            if (Math.Abs(dogPyr[o, i][r, c].Intensity) > prelimContrThr)
                                if (is_extremum(dogPyr, o, i, r, c) == 1)
                                {
                                    feat = interp_extremum(dogPyr, o, i, r, c, intvls, contrThr);
                                    if (feat != null)
                                    {
                                        ddata = feat.feature_data;
                                        if ((is_too_edge_like(dogPyr[ddata.octv, ddata.intvl],
                                            ddata.R, ddata.C, curvThr) == 0))
                                        {
                                            features.Insert(0, feat); //cvSeqPush( features, feat );
                                        }
                                    }
                                }

            return features;
        }

        private int is_extremum(Image<Gray, float>[,] dogPyr, int octv, int intvl, int r, int c)
        {
            float val = (float) dogPyr[octv, intvl][r, c].Intensity;
            int i, j, k;

            /* check for maximum */
            if (val > 0)
            {
                for (i = -1; i <= 1; i++)
                    for (j = -1; j <= 1; j++)
                        for (k = -1; k <= 1; k++)
                            if (val < dogPyr[octv, intvl + i][r + j, c + k].Intensity)
                                return 0;
            }

            /* check for minimum */
            else
            {
                for (i = -1; i <= 1; i++)
                    for (j = -1; j <= 1; j++)
                        for (k = -1; k <= 1; k++)
                            if (val > dogPyr[octv, intvl + i][r + j, c + k].Intensity)
                                return 0;
            }

            return 1;
        }

        private Feature interp_extremum(Image<Gray, float>[,] dogPyr, int octv, int intvl,
            int r, int c, int intvls, double contrThr)
        {
            Feature feat;
            DetectionData ddata;
            double xi = 0, xr = 0, xc = 0, contr;
            int i = 0;

            while (i < SiftMaxInterpSteps)
            {
                interp_step(dogPyr, octv, intvl, r, c, out xi, out xr, out xc);
                if (Math.Abs(xi) < 0.5 && Math.Abs(xr) < 0.5 && Math.Abs(xc) < 0.5)
                    break;

                c += (int) Math.Round(xc);
                r += (int) Math.Round(xr);
                intvl += (int) Math.Round(xi);

                if (intvl < 1 ||
                    intvl > intvls ||
                    c < SiftImgBorder ||
                    r < SiftImgBorder ||
                    c >= dogPyr[octv, 0].Width - SiftImgBorder ||
                    r >= dogPyr[octv, 0].Height - SiftImgBorder)
                {
                    return null;
                }

                i++;
            }

            /* ensure convergence of interpolation */
            if (i >= SiftMaxInterpSteps)
                return null;

            contr = interp_contr(dogPyr, octv, intvl, r, c, xi, xr, xc);
            if (Math.Abs(contr) < contrThr/intvls)
                return null;

            feat = new_feature();
            ddata = feat.feature_data;
            feat.img_pt.X = (float) (feat.x = (c + xc)*Math.Pow(2.0, octv));
            feat.img_pt.Y = (float) (feat.y = (double) ((r + xr)*Math.Pow(2.0, octv)));
            ddata.R = r;
            ddata.C = c;
            ddata.octv = octv;
            ddata.intvl = intvl;
            ddata.subintvl = xi;
            feat.feature_data = ddata;
            return feat;
        }

        private Feature new_feature()
        {
            Feature feat = new Feature();
            DetectionData ddata = new DetectionData();


            feat.feature_data = ddata;
            feat.type = FeatureType.FeatureLowe;

            return feat;
        }

        private void interp_step(Image<Gray, float>[,] dogPyr, int octv, int intvl, int r, int c,
            out double xi, out double xr, out double xc)
        {
            Matrix<Double> dD, h, hInv, X = new Matrix<double>(3, 1);
            double[] x = new double[] {0, 0, 0};

            dD = deriv_3D(dogPyr, octv, intvl, r, c);
            h = hessian_3D(dogPyr, octv, intvl, r, c);
            hInv = h.Clone();


            CvInvoke.cvInvert(h, hInv.Ptr, SOLVE_METHOD.CV_SVD);
            unsafe
            {
                fixed (double* a = &x[0])
                {
                    CvInvoke.cvInitMatHeader(X.Ptr, 3, 1, MAT_DEPTH.CV_64F, new IntPtr(a), 0x7fffffff);
                }
            }
            CvInvoke.cvGEMM(hInv.Ptr, dD.Ptr, -1, IntPtr.Zero, 0, X.Ptr, 0);

            //cvReleaseMat( &dD );
            //cvReleaseMat( &H );
            //cvReleaseMat( &H_inv );

            xi = x[2];
            xr = x[1];
            xc = x[0];
        }

        private Matrix<Double> deriv_3D(Image<Gray, float>[,] dogPyr, int octv, int intvl, int r, int c)
        {
            Matrix<Double> dI;
            double dx, dy, ds;

            dx = (dogPyr[octv, intvl][r, c + 1].Intensity -
                  dogPyr[octv, intvl][r, c - 1].Intensity)/2.0;
            dy = (dogPyr[octv, intvl][r + 1, c].Intensity -
                  dogPyr[octv, intvl][r - 1, c].Intensity)/2.0;
            ds = (dogPyr[octv, intvl + 1][r, c].Intensity -
                  dogPyr[octv, intvl - 1][r, c].Intensity)/2.0;

            dI = new Matrix<Double>(3, 1);
            dI[0, 0] = dx;
            dI[1, 0] = dy;
            dI[2, 0] = ds;

            return dI;
        }

        private Matrix<Double> hessian_3D(Image<Gray, float>[,] dogPyr, int octv, int intvl, int r, int c)
        {
            Matrix<Double> h;
            double v, dxx, dyy, dss, dxy, dxs, dys;

            v = dogPyr[octv, intvl][r, c].Intensity;
            dxx = dogPyr[octv, intvl][r, c + 1].Intensity + dogPyr[octv, intvl][r, c - 1].Intensity - 2*v;
            dyy = dogPyr[octv, intvl][r + 1, c].Intensity +
                  dogPyr[octv, intvl][r - 1, c].Intensity - 2*v;
            dss = dogPyr[octv, intvl + 1][r, c].Intensity +
                  dogPyr[octv, intvl - 1][r, c].Intensity - 2*v;
            dxy = (dogPyr[octv, intvl][r + 1, c + 1].Intensity -
                   dogPyr[octv, intvl][r + 1, c - 1].Intensity -
                   dogPyr[octv, intvl][r - 1, c + 1].Intensity +
                   dogPyr[octv, intvl][r - 1, c - 1].Intensity)/4.0;
            dxs = (dogPyr[octv, intvl + 1][r, c + 1].Intensity -
                   dogPyr[octv, intvl + 1][r, c - 1].Intensity -
                   dogPyr[octv, intvl - 1][r, c + 1].Intensity +
                   dogPyr[octv, intvl - 1][r, c - 1].Intensity)/4.0;
            dys = (dogPyr[octv, intvl + 1][r + 1, c].Intensity -
                   dogPyr[octv, intvl + 1][r - 1, c].Intensity -
                   dogPyr[octv, intvl - 1][r + 1, c].Intensity +
                   dogPyr[octv, intvl - 1][r - 1, c].Intensity)/4.0;

            h = new Matrix<double>(3, 3)
            {
                [0, 0] = dxx,
                [0, 1] = dxy,
                [0, 2] = dxs,
                [1, 0] = dxy,
                [1, 1] = dyy,
                [1, 2] = dys,
                [2, 0] = dxs,
                [2, 1] = dys,
                [2, 2] = dss
            };

            return h;
        }

        private double interp_contr(Image<Gray, float>[,] dogPyr, int octv, int intvl, int r,
            int c, double xi, double xr, double xc)
        {
            Matrix<double> X = new Matrix<double>(3, 1), T = new Matrix<double>(1, 1);
            double[] t = new double[1];
            double[] x = new double[3] {xc, xr, xi};

            unsafe
            {
                fixed (double* a = &x[0])
                {
                    CvInvoke.cvInitMatHeader(X.Ptr, 3, 1, MAT_DEPTH.CV_64F, new IntPtr(a), 0x7fffffff);
                }
            }
            unsafe
            {
                fixed (double* a = &t[0])
                {
                    CvInvoke.cvInitMatHeader(T.Ptr, 1, 1, MAT_DEPTH.CV_64F, new IntPtr(a), 0x7fffffff);
                }
            }
            var dD = deriv_3D(dogPyr, octv, intvl, r, c);
            CvInvoke.cvGEMM(dD.Ptr, X.Ptr, 1, IntPtr.Zero, 0, T.Ptr, GEMM_TYPE.CV_GEMM_A_T);
            //cvReleaseMat( &dD );

            return dogPyr[octv, intvl][r, c].Intensity + t[0]*0.5;
        }

        private int is_too_edge_like(Image<Gray, float> dogImg, int r, int c, int curvThr)
        {
            double d, dxx, dyy, dxy, tr, det;
            /*
             * BT ADDED
             *
             * */
            if ((c == 0) || (r == 0))
                return 1;
            /*
             * BT ENDED
             * /
            /* principal curvatures are computed using the trace and det of Hessian */
            d = dogImg[r, c].Intensity;
            dxx = dogImg[r, c + 1].Intensity + dogImg[r, c - 1].Intensity - 2*d;
            dyy = dogImg[r + 1, c].Intensity + dogImg[r - 1, c].Intensity - 2*d;
            dxy = (dogImg[r + 1, c + 1].Intensity - dogImg[r + 1, c - 1].Intensity -
                   dogImg[r - 1, c + 1].Intensity + dogImg[r - 1, c - 1].Intensity)/4.0;
            tr = dxx + dyy;
            det = dxx*dyy - dxy*dxy;

            /* negative determinant -> curvatures have different signs; reject Feature */
            if (det <= 0)
                return 1;

            if (tr*tr/det < (curvThr + 1.0)*(curvThr + 1.0)/curvThr)
                return 0;
            return 1;
        }
    }
}