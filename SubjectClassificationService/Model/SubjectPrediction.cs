using Microsoft.ML.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubjectClassificationService.Model
{
    public class SubjectPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabels;
    }
}
