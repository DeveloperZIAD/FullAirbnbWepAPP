using System;
using System.Collections.Generic;

public class Conversation
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public Guid GuestId { get; set; } // User
    public Guid HostProfileId { get; set; } // HostProfile
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Listing Listing { get; set; }
    public User Guest { get; set; }
    public HostProfile HostProfile { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class Message
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; } // User
    public string Content { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Conversation Conversation { get; set; }
    public User Sender { get; set; }
}