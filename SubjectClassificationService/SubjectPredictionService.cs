using Microsoft.Extensions.FileProviders;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using SubjectClassificationService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SubjectClassificationService
{
    public class SubjectPredictionService
    {

        public SubjectPredictionService(string dataPath)
        {
            this.dataPath = dataPath;
        }

        private static LearningPipeline pipeline;
        private static PredictionModel<SubjectData, SubjectPrediction> model;
        private string dataPath;

        public static LearningPipeline Init()
        {
            if (pipeline == null)
            {
                pipeline = new LearningPipeline();
            }
            return pipeline;
        }

        public void Train()
        {
            if (pipeline == null)
            {
                Init();
            }
            pipeline.Add(new TextLoader(dataPath).CreateFrom<SubjectData>(separator: ','));
            // STEP 3: Transform your data
            // Assign numeric values to text in the "Label" column, because only
            // numbers can be processed during model training
            pipeline.Add(new Dictionarizer("Label"));

            // Puts all features into a vector
            pipeline.Add(new TextFeaturizer("Features", "SubjectName"));

            // STEP 4: Add learner
            // Add a learning algorithm to the pipeline. 
            // This is a classification scenario (What type of iris is this?)
            pipeline.Add(new StochasticDualCoordinateAscentClassifier());

            // Convert the Label back into original text (after converting to number in step 3)
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter() { PredictedLabelColumn = "PredictedLabel" });

            // STEP 5: Train your model based on the data set
            model = pipeline.Train<SubjectData, SubjectPrediction>();
        }

        public string Predict(SubjectData subjectData)
        {
            if (model == null)
            {
                Train();
            }

            var prediction = model.Predict(subjectData);
            return prediction.PredictedLabels;
        }

        public void AddDataToFile(SubjectData subjectData,string subjectClass)
        {
            try
            {
                string line = "";

                using (var fs=new FileStream(this.dataPath, FileMode.Open,  FileAccess.Read))
                {
                    using (StreamReader r = new StreamReader(fs))
                    {
                        while ((line = r.ReadLine()) != null)
                        {
                            if (line.Contains(subjectClass) && line.Contains(subjectData.SubjectName))
                            {
                                r.Close();
                                fs.Close();
                                return;
                            }
                        }
                        r.Close();
                    }
                    fs.Close();
                }

                using (var fs = new FileStream(this.dataPath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter w = new StreamWriter(fs))
                    {
                        line = "0," + subjectClass + "," + subjectData.SubjectName;
                        w.WriteLine(line);
                        w.Flush();
                        w.Close();
                    }
                    fs.Close();
                }
                Task.Run(() =>
                {
                    Train();
                });
            }
            catch(Exception x)
            {
                throw x;
            }
           
        }

    }
}
