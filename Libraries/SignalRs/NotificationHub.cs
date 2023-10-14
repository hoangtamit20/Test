using Microsoft.AspNetCore.SignalR;

namespace serverapi.Libraries.SignalRs
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationHub : Hub
    {
        /// <summary>
        /// SendNotification là một phương thức bạn có thể gọi từ máy chủ để gửi thông báo đến tất cả các máy khách đã kết nối.
        /// Phương thức Clients.All.SendAsync sẽ gửi tin nhắn đến tất cả các máy khách
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}