
namespace Business_Logic_Layer.DTOs
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public Guid ListingId { get; set; }
        public string ListingTitle { get; set; }
        public Guid GuestId { get; set; }
        public string GuestName { get; set; }
        public Guid HostProfileId { get; set; }
        public string HostName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MessageDto> Messages { get; set; }
    }

    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
    public class StartConversationRequest
    {
        public Guid ListingId { get; set; }
        public Guid GuestId { get; set; }
        public Guid HostProfileId { get; set; }
    }

    public class SendMessageRequest
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; }
    }
}