using FilesUpload.AzureFunction.Models;
using FilesUpload.AzureService;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FilesUploap.AzureFunction
{
    public static class Function1
    {

        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload")] HttpRequestMessage req,
            TraceWriter log)
        {

            log.Info("C# HTTP trigger function processed a request.");
    
            var provider = new MultipartMemoryStreamProvider();
            await req.Content.ReadAsMultipartAsync(provider);
            var files = provider.Contents;

            List<InfoFile> infoFileList = new List<InfoFile>();
            
            foreach (HttpContent file in files)
            {
                var fileData = await file.ReadAsStreamAsync();
                var response = AzureService.SaveFile(fileData, file.Headers.ContentType.ToString(), log);
                log.Info(response.Url);
                infoFileList.Add(response);
            }           

            return req.CreateResponse(HttpStatusCode.OK, infoFileList);
        }
    }

}
