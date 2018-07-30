using Microsoft.ML.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubjectClassificationService.Model
{
    public class SubjectData
    {
        [Column("0")]
        public float SubjectClassNumber;

        [Column("1")]
        [ColumnName("Label")]
        public string Label;

        [Column("2")]
        public string SubjectName;

    }
}
