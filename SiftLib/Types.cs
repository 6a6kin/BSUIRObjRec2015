//SIFT LICENSE CONDITIONS
//Copyright (2005), University of British Columbia.


//This software for the detection of invariant keypoints is being made

//available for individual research use only.  Any commercial use or any

//redistribution of this software requires a license from the University

//of British Columbia.



//The following patent has been issued for methods embodied in this

//software: "Method and apparatus for identifying scale invariant

//features in an image and use of same for locating an object in an

//image," David G. Lowe, US Patent 6,711,293 (March 23,

//2004). Provisional application filed March 8, 1999. Asignee: The

//University of British Columbia.



//For further details on obtaining a commercial license, contact David

//Lowe (lowe@cs.ubc.ca) or the University-Industry Liaison Office of the

//University of British Columbia. 



//THE UNIVERSITY OF BRITISH COLUMBIA MAKES NO REPRESENTATIONS OR

//WARRANTIES OF ANY KIND CONCERNING THIS SOFTWARE.



//This license file must be retained with all copies of the software,

//including any modified or derivative versions.


using System;
using System.Drawing;
using Emgu.CV.Structure;

namespace SiftLib
{
    public class Feature : ICloneable
    {
        public const int FeatureMaxD = 128;
        public double x;                      /* x coord */
        public double y;                      /* y coord */
        public double a;                      /* Oxford-type affine region parameter */
        public double b;                      /* Oxford-type affine region parameter */
        public double c;                      /* Oxford-type affine region parameter */
        public double scl;                    /* scale of a Lowe-style feature */
        public double ori;                    /* orientation of a Lowe-style feature */
        public int d;                         /* descriptor length */
        public double[] descr;                /* descriptor */
        public FeatureType type;              /* feature type, OXFD or LOWE */
        public int category;                  /* all-purpose feature category */
        public Feature fwd_match;             /* matching feature from forward image */
        public Feature bck_match;             /* matching feature from backmward image */
        public Feature mdl_match;             /* matching feature from model */
        public PointF img_pt;                 /* location in image */
        public MCvPoint2D64f mdl_pt;          /* location in model */
        public DetectionData feature_data;    /* user-definable data */
        public Feature()
        {
            descr = new double[FeatureMaxD];
        }
        
        #region ICloneable Members

        public object Clone()
        {
            Feature feat = new Feature();
            feat.a = a;
            feat.b = b;
            feat.bck_match = bck_match;
            feat.c = c;
            feat.category = category;
            feat.d = d;
            feat.descr = (double[]) descr.Clone();
            feat.feature_data = (DetectionData)feature_data.Clone();
            feat.fwd_match = fwd_match;
            feat.img_pt = img_pt;
            feat.mdl_match = mdl_match;
            feat.mdl_pt = mdl_pt;
            feat.ori = ori;
            feat.scl = scl;
            feat.type = type;
            feat.x = x;
            feat.y = y;
            return feat;
        }

        #endregion
    }

    public enum FeatureType
    {
        FeatureOxfd,
        FeatureLowe,
    }

    public struct DetectionData : ICloneable
    {
        public int R { get; set; }
        public int C { get; set; }
        public int octv;
        public int intvl;
        public double subintvl;
        public double scl_octv;
        
        public object Clone()
        {
            DetectionData dat = new DetectionData
            {
                C = C,
                intvl = intvl,
                octv = octv,
                R = R,
                scl_octv = scl_octv,
                subintvl = subintvl
            };
            return dat;
        }
    }
}
