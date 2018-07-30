using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SubjectClassificationService;
using SubjectClassificationService.Model;

namespace SubjectClassification.Controllers
{
    public class PredictionController : Controller
    {
        private IHostingEnvironment _environment;
        private IConfiguration _configuration;

        public PredictionController(IHostingEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }
        
        public string Prediction(string subjectName)
        {
            string dataPath= _configuration["DataPath"];
            dataPath= Path.Combine(_environment.WebRootPath, dataPath);
            var subjectPrediction=new SubjectPredictionService(dataPath);
            var predictValue= subjectPrediction.Predict(new SubjectData
            {
               SubjectName=subjectName 
            });            
            return predictValue;
        }

        public void AddDataToFile(string subjectName,string predictValue)
        {
            string dataPath = _configuration["DataPath"];
            dataPath = Path.Combine(_environment.WebRootPath, dataPath);
            var subjectPrediction = new SubjectPredictionService(dataPath);
            Task.Run(() =>
            {
                subjectPrediction.AddDataToFile(new SubjectData
                {
                    SubjectName = subjectName
                }, predictValue);
            });
        }

    }
}