namespace SmartRentalPlatform.Domain.Enums;

/// <summary>
/// Trạng thái của phòng trong khu trọ
/// </summary>
public enum RoomStatus
{
    /// <summary>Trống - có sẵn để cho thuê</summary>
    Available = 0,
    
    /// <summary>Đang có người ở</summary>
    Occupied = 1,
    
    /// <summary>Đang bảo trì</summary>
    Maintenance = 2,
    
    /// <summary>Ẩn - không hiển thị</summary>
    Hidden = 3
}
