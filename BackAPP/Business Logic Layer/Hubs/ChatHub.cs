using Microsoft.AspNetCore.SignalR;

namespace Business_Logic_Layer.Hubs
{
    public class ChatHub : Hub
    {
        // هذه الدالة تسمى عندما ينضم المستخدم لمحادثة معينة (الغرفة)
        // في ChatHub.cs
        public ChatHub()
        {
            // جرب أن تترك الـ Constructor فارغاً تماماً مؤقتاً
        }
        public override async Task OnConnectedAsync()
        {
            // علق هذا السطر مؤقتاً
            // await base.OnConnectedAsync(); 
            Console.WriteLine("Connection arrived at Hub");
        }
        public async Task JoinConversation(string conversationId)
        {
            try
            {
                if (string.IsNullOrEmpty(conversationId)) return;

                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
                //Console.WriteLine($"Successfully joined group: {conversationId}");
            }
            catch (Exception ex)
            {
                // هذا السطر في الـ Output هو الذي سيعطيك الحل النهائي
                Console.WriteLine($"CRITICAL HUB ERROR: {ex.Message}");
            }
        }
        // هذه الدالة تسمى عندما يغادر المستخدم
        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }
    }
}