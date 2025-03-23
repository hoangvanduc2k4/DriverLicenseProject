using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;
using DriverLicenseApp.DAL.Repository;

namespace DriverLicenseApp.BLL.Service
{
    public class ResultsService
    {
        public List<Result> GetResults(int examId)
        {
            return ResultsRepository.GetResultsByExamId(examId);
        }

        public bool SaveResult(Result result)
        {
            return ResultsRepository.SaveResult(result);
        }

        public List<Result> GetAllResults()
        {
            return ResultsRepository.GetResultsDetail();
        }
    }
}
