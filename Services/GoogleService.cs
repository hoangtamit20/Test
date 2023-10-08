using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using serverapi.Models;
using serverapi.Services.Iservice;

namespace serverapi.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class GoogleService : IGoogleService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public GoogleService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        // Hàm này sẽ gửi một yêu cầu POST đến Google với token nhận được từ client
        // và trả về một đối tượng GoogleResponseModel chứa thông tin người dùng
        public async Task<GoogleResponseModel> GetUserInfoAsync(string token)
        {
            // Tạo một request message với phương thức POST và đường dẫn của Google API
            // var request = new HttpRequestMessage(HttpMethod.Post, "https://www.googleapis.com/oauth2/v2/userinfo");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://openidconnect.googleapis.com/v1/userinfo");
            // Thêm token vào header của request
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gửi request và nhận response
            var response = await _httpClient.SendAsync(request);

            // Kiểm tra nếu response thành công thì đọc nội dung trả về
            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung trả về dưới dạng chuỗi JSON
                var content = await response.Content.ReadAsStringAsync();

                // Chuyển đổi chuỗi JSON thành đối tượng GoogleResponseModel
                var a = JsonConvert.DeserializeObject<object>(content);
                var userInfo = JsonConvert.DeserializeObject<GoogleResponseModel>(content);

                // Trả về đối tượng GoogleResponseModel
                return userInfo!;
            }

            // Nếu response không thành công thì ném ra một ngoại lệ
            throw new Exception("Không thể lấy thông tin người dùng từ Google");
        }
    }
}