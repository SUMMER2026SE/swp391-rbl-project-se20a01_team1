namespace SmartRentalPlatform.Domain.Enums;

/// <summary>
/// Trạng thái hiển thị công khai của khu trọ
/// </summary>
public enum RoomingHouseVisibility
{
    /// <summary>Ẩn - không hiển thị trên công khai</summary>
    Hidden = 0,
    
    /// <summary>Công khai - hiển thị trên danh sách public listing</summary>
    Visible = 1
}
