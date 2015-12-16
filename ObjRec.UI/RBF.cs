using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Emgu.CV;
using Emgu.CV.ML;
using Emgu.CV.Structure;

namespace ObjRec.UI
{
    public class RBF
    {
        private SVM model;

        public SVM RunTraining(Dictionary<float, List<float[]>> classData)
        {
            var totalCount = classData.Values.Sum(x => x.Count);
            var vectorSize = classData.First().Value.First().Length;
            Matrix<float> trainData = new Matrix<float>(totalCount, vectorSize);
            Matrix<float> trainClasses = new Matrix<float>(totalCount, 1);


            int i = 0;
            foreach (var classDescrs in classData)
            {
                foreach (var descr in classDescrs.Value)
                {
                    trainData.Data.SetRow(i, descr);
                    trainClasses.GetRow(i).SetValue(classDescrs.Key);
                    i++;
                }
            }
            model = new SVM();
            SVMParams p = new SVMParams();
            p.KernelType = Emgu.CV.ML.MlEnum.SVM_KERNEL_TYPE.RBF;
            p.SVMType = Emgu.CV.ML.MlEnum.SVM_TYPE.C_SVC;
            p.C = 1;
            p.Gamma = 64;
            p.C = 8;
            p.Nu = 0.5;
            p.TermCrit = new MCvTermCriteria(100, 0.00001);
            //bool trained = model.Train(trainData, trainClasses, null, null, p);
            try
            {
                bool trained = model.TrainAuto(trainData, trainClasses, null, null, p.MCvSVMParams, totalCount);
            }
            catch (Exception q)
            {
                var a = 5;
                return null;
            }
            return model;

        }

        public float DetermineClass(float[] search)
        {
            Matrix<float> sample = new Matrix<float>(1, search.Length);
            sample.Data.SetRow(0, search);
            float response = model.Predict(sample);
            return response;
        }

        public float Run(Dictionary<float, List<float[]>> classData, float[] search)
        {
            var totalCount = classData.Values.Sum(x => x.Count);
            var vectorSize = classData.First().Value.First().Length;
            Matrix<float> trainData = new Matrix<float>(totalCount, vectorSize);
            Matrix<float> trainClasses = new Matrix<float>(totalCount, 1);


            int i = 0;
            foreach (var classDescrs in classData)
            {
                foreach (var descr in classDescrs.Value)
                {
                    trainData.Data.SetRow(i, descr);
                    trainClasses.GetRow(i).SetValue(classDescrs.Key);
                    i++;
                }
            }

            using (SVM model = new SVM())
            {
                SVMParams p = new SVMParams();
                p.KernelType = Emgu.CV.ML.MlEnum.SVM_KERNEL_TYPE.RBF;
                p.SVMType = Emgu.CV.ML.MlEnum.SVM_TYPE.C_SVC;
                p.C = 1;
                p.Gamma = 64;
                p.C = 8;
                p.Nu = 0.5;
                p.TermCrit = new MCvTermCriteria(100, 0.00001);
                //bool trained = model.Train(trainData, trainClasses, null, null, p);
                try
                {
                    bool trained = model.TrainAuto(trainData, trainClasses, null, null, p.MCvSVMParams, totalCount);
                }
                catch (Exception q)
                {
                    var a = 5;
                    throw;
                }




                Matrix<float> sample = new Matrix<float>(1, vectorSize);
                sample.Data.SetRow(0, search);
                float response = model.Predict(sample);


                sample.Data.SetRow(0, classData[0].First());
                response = model.Predict(sample);

                sample.Data.SetRow(0, classData[1].First());
                response = model.Predict(sample);

                sample.Data.SetRow(0, classData[2].First());
                response = model.Predict(sample);

                return response;
            }
        }
    }
}
