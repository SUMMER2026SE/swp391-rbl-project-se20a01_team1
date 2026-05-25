UPDATE admin_approval.rooming_houses SET "ApprovalStatus" = 0 WHERE "Id" IN ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0002', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0003');
DELETE FROM user_roles WHERE "UserId" = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0005' AND "RoleId" = '33333333-3333-3333-3333-333333333333';
UPDATE admin_approval.kyc_verifications SET "Status" = 0 WHERE "Status" <> 1;
