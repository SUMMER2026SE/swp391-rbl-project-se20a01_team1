namespace SmartRentalPlatform.Application.AdminApproval.Exceptions;

/// <summary>
/// Exception khi duyệt thất bại
/// </summary>
public class ApprovalException : Exception
{
    public ApprovalException(string message) : base(message) { }
    public ApprovalException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception khi không tìm thấy entity
/// </summary>
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, Guid id) 
        : base($"{entityName} with ID {id} not found") { }
}

/// <summary>
/// Exception khi không có quyền
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
