namespace SmartRentalPlatform.Application.AdminApproval.Common;

/// <summary>
/// Constants cho Admin Approval module
/// </summary>
public static class ApprovalConstants
{
    /// <summary>
    /// Thời gian sống của Signed URL (phút)
    /// </summary>
    public const int SIGNED_URL_EXPIRATION_MINUTES = 15;

    /// <summary>
    /// Số ký tự cuối cùng của CCCD được hiển thị trước khi mask
    /// VD: 123456****
    /// </summary>
    public const int CCCD_VISIBLE_DIGITS = 6;

    /// <summary>
    /// Các loại phê duyệt
    /// </summary>
    public static class ApprovalTypes
    {
        public const string KYC = "KYC";
        public const string ROOMING_HOUSE = "RoomingHouse";
    }

    /// <summary>
    /// Các hành động duyệt
    /// </summary>
    public static class ApprovalActions
    {
        public const string APPROVED = "Approved";
        public const string REJECTED = "Rejected";
    }

    /// <summary>
    /// Thông báo lỗi
    /// </summary>
    public static class ErrorMessages
    {
        public const string INVALID_PAGINATION = "Invalid pagination parameters";
        public const string KYC_NOT_FOUND = "KYC not found";
        public const string ROOMING_HOUSE_NOT_FOUND = "Rooming house not found";
        public const string REJECTED_REASON_REQUIRED = "RejectedReason is required";
        public const string APPROVAL_FAILED = "Failed to process approval";
        public const string UNAUTHORIZED = "User is not authorized for this action";
    }

    /// <summary>
    /// Thông báo thành công
    /// </summary>
    public static class SuccessMessages
    {
        public const string KYC_APPROVED = "KYC approved successfully";
        public const string KYC_REJECTED = "KYC rejected successfully";
        public const string ROOMING_HOUSE_APPROVED = "Rooming house approved successfully";
        public const string ROOMING_HOUSE_REJECTED = "Rooming house rejected successfully";
    }
}
