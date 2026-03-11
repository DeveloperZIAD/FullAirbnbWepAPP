using Data_Access_Layer.Repositories;

public class AdminActivityLogService
{
    private readonly UnitOfWork _unitOfWork;

    public AdminActivityLogService()
    {
        _unitOfWork = new UnitOfWork();
    }

    public void LogActivity(Guid adminId, string action, string entityName)
    {
        var log = new AdminActivityLog
        {
            Id = Guid.NewGuid(),
            AdminId = adminId,
            Action = action,
            EntityName = entityName,
            Timestamp = DateTime.UtcNow
        };

        _unitOfWork.AdminActivityLogs.Add(log);
        _unitOfWork.Complete();
    }

    public IEnumerable<AdminActivityLogDto> GetLogs()
    {
        return _unitOfWork.AdminActivityLogs.GetAll()
            .OrderByDescending(l => l.Timestamp)
            .Select(l => new AdminActivityLogDto
            {
                AdminId = (Guid)l.AdminId,
                Action = l.Action,
                EntityName = l.EntityName,
                Timestamp = l.Timestamp
            }).ToList();
    }
}