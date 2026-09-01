using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Util
{
    public static class HttpUtil
    {
        public static readonly RestClient client = new RestClient();

        public static async Task<string> DoPost(string url, Dictionary<string, object> data)
        {
            try
            {
                var request = new RestRequest(url, Method.Post);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                foreach (var item in data)
                {
                    request.AddParameter(item.Key, item.Value.ToString());
                }
                var response = await client.ExecuteAsync(request);
                return response.Content;//返回结果
            }
            catch (Exception ex)
            {
                return "{'code':'400','data':'}" + ex.Message + "'}";
            }

        }

        public static async Task<string> DoPostDataAndFile(string url, Dictionary<string, object> data,string filePath)
        {
            try
            {
                var request = new RestRequest(url, Method.Post);
                request.AddHeader("Content-Type", "multipart/form-data");
                foreach (var item in data)
                {
                    if (item.Value == null)
                    {
                        request.AddParameter(item.Key, "");
                    }
                    else
                    {
                        request.AddParameter(item.Key, item.Value.ToString());
                    }
                }
                request.AddFile("file", filePath);
                var response = await client.ExecuteAsync(request);
                return response.Content;//返回结果
            }
            catch (Exception ex)
            {
                return "{'code':'400','data':'}" + ex.Message + "'}";
            }

        }

    }
}
