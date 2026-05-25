# QA Interval 1 — chạy khi API tại http://localhost:5000
$base = "http://localhost:5000"
$failed = 0

function Test-Case($name, $script) {
    Write-Host "`n== $name ==" -ForegroundColor Cyan
    try {
        & $script
        Write-Host "PASS" -ForegroundColor Green
    } catch {
        Write-Host "FAIL: $_" -ForegroundColor Red
        $script:failed++
    }
}

Test-Case "Health" {
    $r = Invoke-WebRequest -Uri "$base/api/health" -UseBasicParsing
    if ($r.StatusCode -ne 200) { throw "health not 200" }
}

Test-Case "Public list excludes Pending Hidden houses" {
    $list = Invoke-RestMethod -Uri ($base + '/api/public/rooming-houses?pageNumber=1&pageSize=50')
    $bad = $list | Where-Object { $_.name -like '*Pending*' -or $_.name -like '*Hidden*' }
    if ($bad) { throw "Found pending/hidden: $($bad.name -join ', ')" }
    if ($list.Count -lt 1) { throw "Expected at least An Binh" }
}

Test-Case "Public detail — chỉ Available rooms" {
    $id = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0001"
    $d = Invoke-RestMethod -Uri "$base/api/public/rooming-houses/$id"
    if ($d.availableRooms.Count -lt 1) { throw "No available rooms" }
    if ($d.availableRooms.roomNumber -contains "A102") { throw "Maintenance room leaked" }
}

Test-Case "Tenant 403 on admin approve" {
    $id = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0002"
    try {
        Invoke-WebRequest -Uri "$base/api/admin/rooming-houses/$id/approve" -Method POST -Headers @{ "X-Dev-Role" = "Tenant" } -UseBasicParsing
        throw "Expected 403"
    } catch {
        if ($_.Exception.Response.StatusCode.value__ -ne 403) { throw "Expected 403 got $($_.Exception.Response.StatusCode)" }
    }
}

Test-Case "Admin approve pending house + grant Landlord role" {
    $houseId = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0002"
    $userId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0005"
    Invoke-RestMethod -Uri "$base/api/admin/rooming-houses/$houseId/approve" -Method POST | Out-Null
    $sql = "SELECT `"RoleId`" FROM user_roles WHERE `"UserId`" = '$userId';"
    $roles = docker exec amp-postgres psql -U postgres -d smart-rental_platform -t -c $sql
    if ($roles -notlike '*33333333*') { throw "Landlord role not granted: $roles" }
}

Test-Case "KYC list has user email" {
    $r = Invoke-RestMethod -Uri ($base + '/api/admin/kyc/pending?pageNumber=1&pageSize=10')
    if ($r.items.Count -lt 1) { throw "No pending KYC" }
    if (-not $r.items[0].userEmail) { throw "Missing userEmail" }
}

Write-Host "`n=== Done: $failed failed ===" -ForegroundColor $(if ($failed) { "Red" } else { "Green" })
