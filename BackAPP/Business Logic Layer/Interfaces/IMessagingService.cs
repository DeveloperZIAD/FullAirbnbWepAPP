using Business_Logic_Layer.DTOs;

public interface IMessagingService
{
    Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(Guid userId);
    Task<ConversationDto> GetConversationByIdAsync(Guid conversationId);
    Task SendMessageAsync(Guid conversationId, Guid senderId, string content);
    Task MarkAsReadAsync(Guid conversationId);
    Task<ConversationDto> CreateConversationAsync(Guid listingId, Guid guestId, Guid hostProfileId);
}