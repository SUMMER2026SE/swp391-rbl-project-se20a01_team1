/** Đồng bộ với server/.../Persistence/Seed/SeedIds.cs */
export const SeedIds = {
  roles: {
    admin: '11111111-1111-1111-1111-111111111111',
    tenant: '22222222-2222-2222-2222-222222222222',
    landlord: '33333333-3333-3333-3333-333333333333',
  },
  users: {
    admin: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0001',
    tenantDone: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0002',
    tenantKyc: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0003',
    landlordDone: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0004',
    landlordKyc: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0005',
  },
  roomingHouses: {
    anBinh: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0001',
    pendingLandlordKyc: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0002',
  },
  rooms: {
    a101: 'cccccccc-cccc-cccc-cccc-cccccccc0001',
  },
  kyc: {
    tenantPending: 'eeeeeeee-eeee-eeee-eeee-eeeeeeee0001',
    landlordPending: 'eeeeeeee-eeee-eeee-eeee-eeeeeeee0002',
  },
} as const;

export const SEED_CREDENTIALS = {
  adminEmail: 'admin@gmail.com',
  defaultPassword: 'Password123!',
} as const;
