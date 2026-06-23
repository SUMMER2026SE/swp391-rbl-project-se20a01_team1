-- Reset data user thuong, giu lai admin
-- Thu tu xoa: con truoc, cha sau (tranh vi pham FK)

-- Chi xoa du lieu seed (demo/mock), KHONG xoa nha tro user tu tao
-- User seed: LandlordUserId=10000000-0000-0000-0000-000000000002,
--            DummyLandlordUserId=10000000-0000-0000-0000-000000009999,
--            Phase2 landlord=90000000-0000-0000-0000-000000000002
DO $$
DECLARE
    seed_landlord_ids uuid[] := ARRAY[
        '10000000-0000-0000-0000-000000000002',
        '10000000-0000-0000-0000-000000009999',
        '90000000-0000-0000-0000-000000000002'
    ];
BEGIN
    DELETE FROM kyc_verifications;
    DELETE FROM login_logs;
    DELETE FROM user_tokens;
    DELETE FROM external_logins;
    DELETE FROM approval_audit_logs;

    -- Chi xoa property_images cua nha tro seed
    DELETE FROM property_images
    WHERE rooming_house_id IN (
        SELECT id FROM rooming_houses WHERE landlord_user_id = ANY(seed_landlord_ids)
    ) OR room_id IN (
        SELECT r.id FROM rooms r
        JOIN rooming_houses h ON h.id = r.rooming_house_id
        WHERE h.landlord_user_id = ANY(seed_landlord_ids)
    );

    DELETE FROM lease_policies
    WHERE rooming_house_id IN (SELECT id FROM rooming_houses WHERE landlord_user_id = ANY(seed_landlord_ids));

    DELETE FROM rooming_house_amenities
    WHERE rooming_house_id IN (SELECT id FROM rooming_houses WHERE landlord_user_id = ANY(seed_landlord_ids));

    DELETE FROM room_amenities
    WHERE room_id IN (SELECT r.id FROM rooms r JOIN rooming_houses h ON h.id = r.rooming_house_id WHERE h.landlord_user_id = ANY(seed_landlord_ids));

    DELETE FROM room_price_tiers
    WHERE room_id IN (SELECT r.id FROM rooms r JOIN rooming_houses h ON h.id = r.rooming_house_id WHERE h.landlord_user_id = ANY(seed_landlord_ids));

    DELETE FROM rooms
    WHERE rooming_house_id IN (SELECT id FROM rooming_houses WHERE landlord_user_id = ANY(seed_landlord_ids));

    DELETE FROM rooming_house_legal_documents
    WHERE rooming_house_id IN (SELECT id FROM rooming_houses WHERE landlord_user_id = ANY(seed_landlord_ids));

    -- Chi xoa nha tro seed
    DELETE FROM rooming_houses
    WHERE landlord_user_id = ANY(seed_landlord_ids);
END $$;

-- Xoa user_roles cua user seed (giu lai user tao bang tay)
DELETE FROM user_roles
WHERE user_id IN (
    '10000000-0000-0000-0000-000000000001',
    '10000000-0000-0000-0000-000000000002',
    '10000000-0000-0000-0000-000000000003',
    '10000000-0000-0000-0000-000000009999',
    '10000000-0000-0000-0000-000000000099',
    '90000000-0000-0000-0000-000000000002'
);

-- Xoa user seed (giu lai user tao bang tay)
DELETE FROM users
WHERE id IN (
    '10000000-0000-0000-0000-000000000001',
    '10000000-0000-0000-0000-000000000002',
    '10000000-0000-0000-0000-000000000003',
    '10000000-0000-0000-0000-000000009999',
    '10000000-0000-0000-0000-000000000099',
    '90000000-0000-0000-0000-000000000002'
);

-- Ket qua
SELECT 'DONE - Da xoa xong seed data, giu lai du lieu user tao!' AS result;
SELECT 'So nha tro con lai: ' || COUNT(*)::text FROM rooming_houses;
SELECT u.email, r.name AS role FROM users u
JOIN user_roles ur ON ur.user_id = u.id
JOIN roles r ON r.id = ur.role_id;
