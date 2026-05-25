using Microsoft.EntityFrameworkCore;
using SmartRentalPlatform.Application.AdminApproval.DTOs;
using SmartRentalPlatform.Contracts.Storage;
using SmartRentalPlatform.Infrastructure.Persistence;
using SmartRentalPlatform.Domain.Enums;

namespace SmartRentalPlatform.Application.AdminApproval.Services;

public class KYCApprovalService : IKYCApprovalService
{
    private readonly AppDbContext _dbContext;
    private readonly ISignedUrlGenerator _signedUrlGenerator;

    public KYCApprovalService(AppDbContext dbContext, ISignedUrlGenerator signedUrlGenerator)
    {
        _dbContext = dbContext;
        _signedUrlGenerator = signedUrlGenerator;
    }

    public async Task<KYCListResponseDto> GetPendingKYCsAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.KYCVerifications
            .Where(k => k.Status == KYCStatus.PendingAdminReview);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(k => k.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Join(
                _dbContext.Users,
                k => k.UserId,
                u => u.Id,
                (k, u) => new KYCListDto
                {
                    Id = k.Id,
                    UserId = k.UserId,
                    UserEmail = u.Email,
                    UserDisplayName = u.DisplayName,
                    FullName = k.FullName,
                    Status = k.Status.ToString(),
                    CreatedAt = k.CreatedAt
                })
            .ToListAsync(cancellationToken);

        return new KYCListResponseDto
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = pageSize,
            PageNumber = pageNumber
        };
    }

    public async Task<KYCDetailDto?> GetKYCDetailAsync(Guid kycId, CancellationToken cancellationToken)
    {
        var row = await (
            from k in _dbContext.KYCVerifications
            join u in _dbContext.Users on k.UserId equals u.Id
            where k.Id == kycId
            select new { k, u }
        ).FirstOrDefaultAsync(cancellationToken);

        if (row == null)
            return null;

        var kyc = row.k;
        var user = row.u;

        return new KYCDetailDto
        {
            Id = kyc.Id,
            UserId = kyc.UserId,
            UserEmail = user.Email,
            UserDisplayName = user.DisplayName,
            FullName = kyc.FullName,
            DateOfBirth = kyc.DateOfBirth,
            IdNumber = MaskCCCD(kyc.IdNumber),
            Address = kyc.Address,
            FaceMatchScore = kyc.FaceMatchScore,
            LivenessScore = kyc.LivenessScore,
            IdImageUrl = _signedUrlGenerator.GenerateReadUrl(kyc.IdImageObjectKey, cancellationToken),
            FaceImageUrl = _signedUrlGenerator.GenerateReadUrl(kyc.FaceImageObjectKey, cancellationToken),
            Status = kyc.Status.ToString(),
            RejectedReason = kyc.RejectedReason,
            ReviewedByAdminId = kyc.ReviewedByAdminId,
            ReviewedAt = kyc.ReviewedAt,
            CreatedAt = kyc.CreatedAt
        };
    }

    public async Task<bool> ApproveKYCAsync(Guid kycId, Guid adminId, CancellationToken cancellationToken)
    {
        var kyc = await _dbContext.KYCVerifications
            .FirstOrDefaultAsync(k => k.Id == kycId, cancellationToken);

        if (kyc == null || kyc.Status != KYCStatus.PendingAdminReview)
            return false;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == kyc.UserId, cancellationToken);
        if (user == null)
            return false;

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            kyc.Status = KYCStatus.Approved;
            kyc.ReviewedByAdminId = adminId;
            kyc.ReviewedAt = DateTime.UtcNow;
            kyc.UpdatedAt = DateTime.UtcNow;

            user.OnboardingStatus = OnboardingStatus.Completed;
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.KYCVerifications.Update(kyc);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> RejectKYCAsync(
        Guid kycId, string rejectedReason, Guid adminId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rejectedReason))
            return false;

        var kyc = await _dbContext.KYCVerifications
            .FirstOrDefaultAsync(k => k.Id == kycId, cancellationToken);

        if (kyc == null || kyc.Status != KYCStatus.PendingAdminReview)
            return false;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == kyc.UserId, cancellationToken);
        if (user == null)
            return false;

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            kyc.Status = KYCStatus.Rejected;
            kyc.RejectedReason = rejectedReason;
            kyc.ReviewedByAdminId = adminId;
            kyc.ReviewedAt = DateTime.UtcNow;
            kyc.UpdatedAt = DateTime.UtcNow;

            user.OnboardingStatus = OnboardingStatus.NeedKyc;
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.KYCVerifications.Update(kyc);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static string? MaskCCCD(string? cccd)
    {
        if (string.IsNullOrEmpty(cccd) || cccd.Length < 6)
            return cccd;
        return cccd[..6] + new string('*', cccd.Length - 6);
    }
}
