using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNameTemplate.Common.Helper
{
    public class HttpHelper
    {
        public static readonly HttpHelper Instance;
        static HttpHelper()
        {
            Instance = new HttpHelper();
        }

        private HttpClient httpClient;
        public HttpHelper()
        {
            httpClient = new HttpClient();
        }

        /// <summary>
        /// 设置基址
        /// </summary>
        /// <param name="baseUrl"></param>
        public void SetBaseAddress(string baseUrl)
        {
            httpClient.BaseAddress = new Uri(baseUrl);
        }

        /// <summary>
        /// 是否已经设置了BaseAddress(true 为未设置)
        /// </summary>
        public bool IsBaseAddress
        {
            get
            {
                return httpClient.BaseAddress == null;
            }
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="jsonString">请求参数（Json字符串）</param>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, string jsonString)
        {
            StringContent content = new StringContent(jsonString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var httpResponseMessage = await httpClient.PostAsync(url, content);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url地址</param>
        /// <param name="content">请求参数</param>
        /// <returns></returns>
        public async Task<string> PostAsync<T>(string url, T content) where T : class
        {
            return await PostAsync(url, JsonConvert.SerializeObject(content));
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string url)
        {
            var httpResponseMessage = await httpClient.GetAsync(url);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="jsonString">请求参数（Json字符串）</param>
        /// <returns></returns>
        public async Task<string> PutAsync(string url, string jsonString)
        {
            StringContent content = new StringContent(jsonString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var httpResponseMessage = await httpClient.PutAsync(url, content);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url地址</param>
        /// <param name="content">请求参数</param>
        /// <returns></returns>
        public async Task<string> PutAsync<T>(string url, T content)
        {
            return await PutAsync(url, JsonConvert.SerializeObject(content));
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns></returns>
        public async Task<string> DeleteAsync(string url)
        {
            var httpResponseMessage = await httpClient.DeleteAsync(url);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }
    }
}
