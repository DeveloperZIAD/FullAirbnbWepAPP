using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore; // لضمان عمل ToListAsync
using Business_Logic_Layer.DTOs;
using Microsoft.AspNetCore.SignalR;
using Business_Logic_Layer.Hubs;

namespace Business_Logic_Layer.Services { 

public class MessagingService : IMessagingService
{
        private readonly IHubContext<ChatHub> _hubContext; // الحقن هنا

        private readonly UnitOfWork _unitOfWork;


    public MessagingService(UnitOfWork unitOfWork, IHubContext<ChatHub> hubContext)
    {
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }
    
        // 1. جلب محادثات المستخدم (ضيف أو مضيف)
        public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(Guid userId)
        {
            return await _unitOfWork.Conversations.GetQueryable(
                    c => c.Listing,
                    c => c.Guest,
                    c => c.HostProfile.User,
                    c => c.Messages // تأكد من تضمين الرسائل هنا
                )
                .Where(c => c.GuestId == userId || c.HostProfile.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    ListingId = c.ListingId,
                    ListingTitle = c.Listing.Title,
                    GuestId = c.GuestId,
                    GuestName = c.Guest.FullName,
                    HostProfileId = c.HostProfileId,
                    HostName = c.HostProfile.User.FullName,
                    CreatedAt = c.CreatedAt,
                    // نقوم بعمل Projection للرسائل داخل المحادثة
                    Messages = c.Messages.OrderBy(m => m.CreatedAt).Select(m => new MessageDto
                    {
                        Id = m.Id,
                        ConversationId = m.ConversationId,
                        SenderId = m.SenderId,
                        Content = m.Content,
                        CreatedAt = m.CreatedAt,
                        IsRead = m.IsRead
                    }).ToList()
                })
                .ToListAsync();
        }

        // 2. جلب محادثة واحدة بكامل رسائلها
        public async Task<ConversationDto> GetConversationByIdAsync(Guid conversationId)
        {
            // 1. إضافة الـ Includes اللازمة لجلب بيانات الضيف والمضيف والعقار
            var conversation = await _unitOfWork.Conversations.GetQueryable(
                    c => c.Listing,
                    c => c.Guest,
                    c => c.HostProfile.User, // ربط المضيف بالمستخدم لجلب اسمه
                    c => c.Messages // جلب الرسائل
                )
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null) return null;

            return new ConversationDto
            {
                Id = conversation.Id,
                ListingId = conversation.ListingId,
                ListingTitle = conversation.Listing?.Title ?? "Unknown Property",

                GuestId = conversation.GuestId,
                GuestName = conversation.Guest?.FullName ?? "Unknown Guest",

                HostProfileId = conversation.HostProfileId,
                HostName = conversation.HostProfile?.User?.FullName ?? "Unknown Host",

                CreatedAt = conversation.CreatedAt,

                Messages = conversation.Messages.OrderBy(m => m.CreatedAt).Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId, // إضافة هذا الحقل المفقود
                    SenderId = m.SenderId,
                    // لجلب اسم المرسل، نحتاج أن يكون الـ Sender متاحاً أو البحث عنه
                    SenderName = m.SenderId == conversation.GuestId ? conversation.Guest?.FullName : conversation.HostProfile?.User?.FullName,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    IsRead = m.IsRead
                }).ToList()
            };
        }

        // 3. إرسال رسالة جديدة
        public async Task SendMessageAsync(Guid conversationId, Guid senderId, string content)
        {
            // 1. جلب المحادثة مع Include لـ HostProfile والـ User الخاص به للتحقق من الـ UserId
            var conversation = await _unitOfWork.Conversations.GetQueryable(
                    c => c.HostProfile,
                    c => c.Guest
                )
                .FirstOrDefaultAsync(c => c.Id == conversationId); // استخدام Async هنا ضروري

            if (conversation == null)
                throw new Exception("Conversation not found.");

            // 2. التحقق من صلاحية المرسل (تأكد أن Entity المحادثة بها UserId للمضيف)
            bool isGuest = conversation.GuestId == senderId;
            bool isHost = conversation.HostProfile.UserId == senderId;

            if (!isGuest && !isHost)
            {
                throw new Exception("You are not allowed to send messages in this conversation.");
            }

            // 3. إضافة الرسالة
            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _unitOfWork.Messages.Add(message);
            _unitOfWork.Complete(); // حفظ في الداتابيز أولاً

            // 4. إرسال عبر SignalR (تأكد من استخدام الترتيب الصحيح)
            await _hubContext.Clients.Group(conversationId.ToString().ToLower()) // توحيد حالة الحروف
                .SendAsync("ReceiveMessage", new MessageDto
                {
                    Id = message.Id,
                    ConversationId = message.ConversationId,
                    SenderId = message.SenderId,
                    SenderName = isGuest ? conversation.Guest?.FullName : "Host", // تبسيط لجلب الاسم
                    Content = message.Content,
                    CreatedAt = message.CreatedAt,
                    IsRead = message.IsRead
                });
        }
        // 4. تمييز الرسائل كمقروءة
        public async Task MarkAsReadAsync(Guid conversationId)
    {
        var messages = _unitOfWork.Messages.GetQueryable()
            .Where(m => m.ConversationId == conversationId && !m.IsRead)
            .ToList();

        foreach (var msg in messages)
        {
            msg.IsRead = true;
            _unitOfWork.Messages.Update(msg);
        }

        _unitOfWork.Complete();
    }

        public async Task<ConversationDto> CreateConversationAsync(Guid listingId, Guid guestId, Guid hostProfileId)
        {
            // 1. التحقق من وجود المحادثة مسبقاً
            // is the listing hosted by the hostProfileId? if not, return null or throw an exception
            var listing = _unitOfWork.Listings.GetById(listingId);
            if (listing == null || listing.HostId != hostProfileId)
            {
                return null;
            }

            var existingConversation = _unitOfWork.Conversations.GetQueryable(c => c.Listing, c => c.Guest, c => c.HostProfile.User)
                .FirstOrDefault(c => c.ListingId == listingId && c.GuestId == guestId && c.HostProfileId == hostProfileId);

            if (existingConversation != null)
            {
                return MapToDto(existingConversation);
            }

            // 2. إنشاء محادثة جديدة
            var newConversation = new Conversation
            {
                ListingId = listingId,
                GuestId = guestId,
                HostProfileId = hostProfileId,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.Conversations.Add(newConversation);
            _unitOfWork.Complete();

            // 3. الخطوة الأهم: جلب المحادثة من قاعدة البيانات مع بياناتها المرتبطة (Include)
            // نستخدم GetById مع الـ Includes لجلب العقار والأسماء
            var createdConversation = _unitOfWork.Conversations.GetQueryable(c => c.Listing, c => c.Guest, c => c.HostProfile.User)
                                                                .FirstOrDefault(c => c.Id == newConversation.Id);

            return MapToDto(createdConversation);
        }

        // دالة مساعدة للتحويل (Mapper)
        private ConversationDto MapToDto(Conversation c)
        {
            return new ConversationDto
            {
                Id = c.Id,
                ListingId = c.ListingId,
                ListingTitle = c.Listing?.Title ?? "Unknown Property",
                GuestId = c.GuestId,
                GuestName = c.Guest?.FullName ?? "Unknown Guest",
                HostProfileId = c.HostProfileId,
                HostName = c.HostProfile?.User?.FullName ?? "Unknown Host",
                CreatedAt = c.CreatedAt,
                Messages = new List<MessageDto>()
            };
        }
    }
}