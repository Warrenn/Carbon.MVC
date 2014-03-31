using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using CarbonKnown.DAL;
using CarbonKnown.FileReaders.Generic;
using CarbonKnown.GenericFile.Properties;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace CarbonKnown.GenericFile
{
    public class GenericExcelHandler : IHttpHandler
    {
        public const string ExceptionPolicyName = "GenericExcelFilePolicy";

        public bool IsReusable
        {
            get { return true; }
        }

        public IEnumerable<string> CreateCostCodes()
        {
            using (var context = new DataContext())
            {
                foreach (var centre in context.CostCentres)
                {
                    var descriptions = context.CostCentreTreeWalk(centre.CostCode).Select(c => c.Name);
                    var costCodeString = CreateCostCodeString(centre.CostCode, descriptions);
                    yield return costCodeString;
                } 
            }
        }

        public string CreateCostCodeString(string costCode, IEnumerable<string> descriptions)
        {
            var builder = new StringBuilder();
            foreach (var description in descriptions.Reverse())
            {
                builder.AppendFormat("{0} ->", description);
            }
            var path = builder.ToString();
            path = path.Substring(0, path.Length - 3);
            return string.Format("{0} : {1}", costCode, path);
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var filePath = context.Server.MapPath(Settings.Default.GenericFilePath);
                if (!File.Exists(filePath)) return;
                var excelStream = File.OpenRead(filePath);
                var excelfile = new GenericExcelFile(excelStream);
                var costCodes = CreateCostCodes();
                var consumptionTypes = GenericHandler.Mappings.Select(pair => pair.Key);
                var package = excelfile.CreatePackage(costCodes, consumptionTypes);
                var response = context.Response;
                response.BinaryWrite(package.GetAsByteArray());
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.AddHeader("content-disposition",
                                   "attachment;  filename=" + Settings.Default.GenericDownloadName);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicyName);
            }
        }
    }
}