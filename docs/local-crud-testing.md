# Local CRUD Testing — DotNet User Service

This document verifies the complete CRUD lifecycle of the `.NET 9 User Service` using PowerShell.

The test intentionally creates **3 users** so that we can verify:

* Empty database
* Create first user
* Read first user
* Create second user
* Read all users
* Create third user
* Read all 3 users
* Delete the middle/second user
* Try to fetch the deleted single user
* Fetch all remaining users
* Update the third user
* Fetch the updated third user
* Fetch all users
* Delete another user
* Final fetch all
* Confirm exactly **1 user remains**

---

# Terminal Setup

## Terminal 1 — Run the API

Navigate to the project:

```powershell
# Go to the .NET User Service project
cd E:\DevOps-Projects\dotnet-user-service\src\DotNetUserService
```

Start the API:

```powershell
# Start the local .NET User Service
dotnet run
```

The API should be listening on:

```text
http://localhost:5228
```

Keep Terminal 1 running.

---

# Terminal 2 — CRUD Testing

Open a second PowerShell terminal.

Navigate to the project:

```powershell
# Go to the .NET User Service project
cd E:\DevOps-Projects\dotnet-user-service\src\DotNetUserService
```

All CRUD commands below are executed in Terminal 2.

---

# Step 1 — Verify Database Is Empty

### Operation

```text
READ ALL
```

### Purpose

Before creating anything, verify that the database currently contains no users.

### PowerShell

```powershell
# Fetch all users
# Expected result: an empty array because the database is currently empty
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users" `
  -Method GET
```

### Expected Result

```text
[]
```

### Verification

```text
EMPTY DATABASE ✅
```

---

# Step 2 — Create User 1

### Operation

```text
CREATE
POST /api/users
```

### Purpose

Create the first user.

### PowerShell

```powershell
# CREATE User 1
# This inserts the first user into the SQLite database
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"username":"bro-user-one","email":"bro.one@example.com"}'
```

### Expected Result

```text
id = 1
username = bro-user-one
email = bro.one@example.com
```

### Verification

```text
USER 1 CREATED ✅
```

---

# Step 3 — Read User 1

### Operation

```text
READ SINGLE
GET /api/users/1
```

### Purpose

Verify that User 1 can be fetched individually.

### PowerShell

```powershell
# Fetch User 1
# This verifies the first record exists
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users/1" `
  -Method GET
```

### Expected Result

```text
id = 1
username = bro-user-one
email = bro.one@example.com
```

### Verification

```text
USER 1 READ ✅
```

---

# Step 4 — Create User 2

### Operation

```text
CREATE
POST /api/users
```

### Purpose

Create the second user.

### PowerShell

```powershell
# CREATE User 2
# This inserts a second user into the SQLite database
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"username":"bro-user-two","email":"bro.two@example.com"}'
```

### Expected Result

```text
id = 2
username = bro-user-two
email = bro.two@example.com
```

### Verification

```text
USER 2 CREATED ✅
```

---

# Step 5 — Fetch All Users

### Operation

```text
READ ALL
GET /api/users
```

### Purpose

Verify that both User 1 and User 2 exist.

### PowerShell

```powershell
# Fetch all users
# Expected result: 2 users
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users" `
  -Method GET
```

### Expected Result

```text
id  username       email
--  -------------  ---------------------
1   bro-user-one   bro.one@example.com
2   bro-user-two   bro.two@example.com
```

### Verification

```text
2 USERS PRESENT ✅
```

---

# Step 6 — Create User 3

### Operation

```text
CREATE
POST /api/users
```

### Purpose

Create the third user.

### PowerShell

```powershell
# CREATE User 3
# This inserts a third user into the SQLite database
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"username":"bro-user-three","email":"bro.three@example.com"}'
```

### Expected Result

```text
id = 3
username = bro-user-three
email = bro.three@example.com
```

### Verification

```text
USER 3 CREATED ✅
```

---

# Step 7 — Fetch All 3 Users

### Operation

```text
READ ALL
GET /api/users
```

### Purpose

Verify that all three records exist.

### PowerShell

```powershell
# Fetch all users
# Expected result: 3 users
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users" `
  -Method GET
```

### Expected Result

```text
id  username         email
--  ---------------  -----------------------
1   bro-user-one     bro.one@example.com
2   bro-user-two     bro.two@example.com
3   bro-user-three   bro.three@example.com
```

### Verification

```text
3 USERS PRESENT ✅
```

---

# Step 8 — Delete User 2

### Operation

```text
DELETE
DELETE /api/users/2
```

### Purpose

Delete the **middle record**.

This is important because we are not deleting the first or last user.

Before deletion:

```text
1 → User One
2 → User Two
3 → User Three
```

After deletion:

```text
1 → User One
3 → User Three
```

User ID `2` should no longer exist.

### PowerShell

```powershell
# DELETE User 2
# This removes the middle record from the Users table
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users/2" `
  -Method DELETE
```

### Expected

Depending on the controller implementation, PowerShell may show nothing if the API returns `204 No Content`.

### Verification

```text
USER 2 DELETED ✅
```

---

# Step 9 — Fetch Deleted User 2

### Operation

```text
READ SINGLE
GET /api/users/2
```

### Purpose

Confirm that User 2 really no longer exists.

### PowerShell

```powershell
# Try to fetch the deleted User 2
# Expected result: HTTP 404 Not Found
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users/2" `
  -Method GET
```

### Expected Result

The request should fail with a `404 Not Found`.

The API should return something similar to:

```json
{
  "message": "User not found"
}
```

### Verification

```text
USER 2 NO LONGER EXISTS ✅
```

---

# Step 10 — Fetch All Users After Deleting User 2

### Operation

```text
READ ALL
GET /api/users
```

### Purpose

Confirm that User 1 and User 3 remain.

### PowerShell

```powershell
# Fetch all remaining users
# User 2 should be gone
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users" `
  -Method GET
```

### Expected Result

```text
id  username         email
--  ---------------  -----------------------
1   bro-user-one     bro.one@example.com
3   bro-user-three   bro.three@example.com
```

Notice:

```text
User 1 → exists
User 2 → deleted
User 3 → exists
```

The IDs do **not** become `1` and `2`.

The database keeps User 3's original ID of `3`.

### Verification

```text
2 USERS REMAINING ✅
```

---

# Step 11 — Update User 3

### Operation

```text
UPDATE
PUT /api/users/3
```

### Purpose

Update the third user.

Before:

```text
username = bro-user-three
email    = bro.three@example.com
```

After:

```text
username = bro-user-three-updated
email    = bro.three.updated@example.com
```

### PowerShell

```powershell
# UPDATE User 3
# Change the username and email of the third user
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users/3" `
  -Method PUT `
  -ContentType "application/json" `
  -Body '{"username":"bro-user-three-updated","email":"bro.three.updated@example.com"}'
```

### Expected Result

```text
id = 3
username = bro-user-three-updated
email = bro.three.updated@example.com
```

### Verification

```text
USER 3 UPDATED ✅
```

---

# Step 12 — Fetch Updated User 3

### Operation

```text
READ SINGLE
GET /api/users/3
```

### Purpose

Confirm that the update was actually persisted in SQLite.

### PowerShell

```powershell
# Fetch User 3 after the UPDATE
# This verifies that the updated values were persisted
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users/3" `
  -Method GET
```

### Expected Result

```text
id = 3
username = bro-user-three-updated
email = bro.three.updated@example.com
```

### Verification

```text
USER 3 UPDATE VERIFIED ✅
```

---

# Step 13 — Fetch All After Update

### Operation

```text
READ ALL
GET /api/users
```

### Purpose

Verify that both remaining users exist and User 3 contains the updated values.

### PowerShell

```powershell
# Fetch all users
# Expected result: User 1 and updated User 3
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users" `
  -Method GET
```

### Expected Result

```text
id  username               email
--  ---------------------  -----------------------------
1   bro-user-one           bro.one@example.com
3   bro-user-three-updated bro.three.updated@example.com
```

### Verification

```text
2 USERS REMAINING + USER 3 UPDATED ✅
```

---

# Step 14 — Delete User 1

### Operation

```text
DELETE
DELETE /api/users/1
```

### Purpose

Delete one more user.

At this point:

```text
1 → User One
3 → User Three Updated
```

After deleting User 1:

```text
3 → User Three Updated
```

### PowerShell

```powershell
# DELETE User 1
# This removes the first remaining user
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users/1" `
  -Method DELETE
```

### Verification

```text
USER 1 DELETED ✅
```

---

# Step 15 — Final Fetch All

### Operation

```text
READ ALL
GET /api/users
```

### Purpose

Perform the final database verification.

Only User 3 should remain.

### PowerShell

```powershell
# FINAL READ ALL
# Only User 3 should remain in the database
Invoke-RestMethod `
  -Uri "http://localhost:5228/api/users" `
  -Method GET
```

### Expected Result

```text
id  username               email
--  ---------------------  -----------------------------
3   bro-user-three-updated bro.three.updated@example.com
```

### Final Verification

```text
1 USER REMAINS ✅
```

---

# Complete Test Flow

```text
DATABASE STARTS EMPTY
        │
        ▼
GET /api/users
        │
        ▼
[]
        │
        ▼
POST User 1
        │
        ▼
User 1
        │
        ▼
GET User 1
        │
        ▼
User 1 returned
        │
        ▼
POST User 2
        │
        ▼
User 1 + User 2
        │
        ▼
GET ALL
        │
        ▼
2 USERS
        │
        ▼
POST User 3
        │
        ▼
User 1 + User 2 + User 3
        │
        ▼
GET ALL
        │
        ▼
3 USERS
        │
        ▼
DELETE User 2
        │
        ▼
User 1 + User 3
        │
        ▼
GET User 2
        │
        ▼
404 NOT FOUND
        │
        ▼
GET ALL
        │
        ▼
2 USERS
        │
        ▼
UPDATE User 3
        │
        ▼
GET User 3
        │
        ▼
UPDATED USER 3
        │
        ▼
GET ALL
        │
        ▼
2 USERS
        │
        ▼
DELETE User 1
        │
        ▼
GET ALL
        │
        ▼
1 USER REMAINS
```

---

# CRUD Verification Summary

| Step | Operation       | Method | Endpoint       | Expected       |
| ---: | --------------- | ------ | -------------- | -------------- |
|    1 | Initial fetch   | GET    | `/api/users`   | `[]`           |
|    2 | Create User 1   | POST   | `/api/users`   | User 1         |
|    3 | Read User 1     | GET    | `/api/users/1` | User 1         |
|    4 | Create User 2   | POST   | `/api/users`   | User 2         |
|    5 | Fetch all       | GET    | `/api/users`   | 2 users        |
|    6 | Create User 3   | POST   | `/api/users`   | User 3         |
|    7 | Fetch all       | GET    | `/api/users`   | 3 users        |
|    8 | Delete User 2   | DELETE | `/api/users/2` | Deleted        |
|    9 | Fetch User 2    | GET    | `/api/users/2` | 404            |
|   10 | Fetch all       | GET    | `/api/users`   | 2 users        |
|   11 | Update User 3   | PUT    | `/api/users/3` | Updated        |
|   12 | Fetch User 3    | GET    | `/api/users/3` | Updated User 3 |
|   13 | Fetch all       | GET    | `/api/users`   | 2 users        |
|   14 | Delete User 1   | DELETE | `/api/users/1` | Deleted        |
|   15 | Final fetch all | GET    | `/api/users`   | **1 user**     |

---

# Final Expected Database State

At the beginning:

```text
[]
```

After creating three users:

```text
1 → User One
2 → User Two
3 → User Three
```

After deleting User 2:

```text
1 → User One
3 → User Three
```

After updating User 3:

```text
1 → User One
3 → User Three Updated
```

After deleting User 1:

```text
3 → User Three Updated
```

Therefore the **final expected result is exactly one record**:

```text
id = 3
username = bro-user-three-updated
email = bro.three.updated@example.com
```

---

# Important Note About IDs

This test assumes the database is empty before starting and that SQLite assigns IDs:

```text
1
2
3
```

If the database already contains previous records, the new users may receive different IDs.

In that case, use the actual IDs returned by the POST requests instead of assuming `1`, `2`, and `3`.

---

# Success Criteria

The local User Service passes this CRUD test when all of the following are true:

```text
[✓] Empty database confirmed
[✓] User 1 created
[✓] User 1 fetched individually
[✓] User 2 created
[✓] 2 users fetched
[✓] User 3 created
[✓] 3 users fetched
[✓] User 2 deleted
[✓] Deleted User 2 returns 404
[✓] User 1 and User 3 remain
[✓] User 3 updated
[✓] Updated User 3 fetched successfully
[✓] 2 users remain
[✓] User 1 deleted
[✓] Final fetch returns exactly 1 user
[✓] SQLite persistence verified
```

This completes the **local CRUD verification** for the DotNet User Service.
