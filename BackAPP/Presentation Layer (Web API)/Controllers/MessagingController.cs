using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Interfaces;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Business_Logic_Layer.DTOs;
using Microsoft.AspNetCore.SignalR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirbnbClone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        private readonly IMessagingService _messagingService;

        public MessagingController(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
        {
            if (request == null || request.ListingId == Guid.Empty)
                return BadRequest("Invalid request data.");

            try
            {
                var conversation = await _messagingService.CreateConversationAsync(
                    request.ListingId,
                    request.GuestId,
                    request.HostProfileId
                );
                if (conversation == null) return BadRequest("Failed to start conversation.");

                return Ok(conversation);
            }
            catch (Exception ex)
            {
                // سجل الخطأ هنا باستخدام Logger
                return StatusCode(500, "An error occurred while starting the conversation.");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserConversations(Guid userId)
        {
            var conversations = await _messagingService.GetUserConversationsAsync(userId);
            return Ok(conversations);
        }

        [HttpGet("conversation/{conversationId}")]
        public async Task<IActionResult> GetConversationMessages(Guid conversationId)
        {
            var conversation = await _messagingService.GetConversationByIdAsync(conversationId);
            if (conversation == null) return NotFound("Conversation not found.");

            return Ok(conversation);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                await _messagingService.SendMessageAsync(request.ConversationId, request.SenderId, request.Content);
                return Ok(new { message = "Message sent successfully." });
            }
            
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("mark-read/{conversationId}")]
        public async Task<IActionResult> MarkAsRead(Guid conversationId)
        {
            await _messagingService.MarkAsReadAsync(conversationId);
            return Ok(new { message = "Messages marked as read." });
        }
    }
}
