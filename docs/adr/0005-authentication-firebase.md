# 5. Authentication: Firebase Authentication

Date: 2026-06-11

## Status

Accepted

## Context

We need authentication that:

- Is friendly to **parents** (email/password + optionally Google sign-in).
- Lets parents create **child sub-profiles** *without* each child needing their own email — children pick a profile + (optional) PIN on the device.
- Is COPPA/GDPR-aware (we do not collect emails for children under 13).
- Has SDKs for React (web) and works server-side with .NET for ID token verification.
- Is operationally cheap for a small project.

The user explicitly chose **Firebase Authentication**.

## Decision

Use **Firebase Authentication** as the identity provider for **parents only**. Children are domain entities in our own database, *not* Firebase users.

### Flow

1. **Parent signs up / signs in** via Firebase (email/password, Google).
2. Frontend obtains a Firebase **ID token** (JWT) and sends it as `Authorization: Bearer <token>` to the .NET API.
3. .NET backend validates the token using the **FirebaseAdmin** SDK (verifies signature against Google's JWKs, audience = our Firebase project ID, issuer matches, not expired).
4. Backend looks up / creates a `Parent` row keyed by Firebase UID (`Parents.FirebaseUid`).
5. **Children** are created by the authenticated parent through `POST /api/children`. Children have no Firebase identity.
6. **Child "login"** on the device: the parent selects a child profile from a list scoped to that parent's UID. Optional 4-digit PIN per child stored hashed in our DB.
7. All child-scoped API requests still carry the parent's Firebase token; the backend authorizes by checking `Child.ParentId == AuthenticatedParent.Id`.

### What we will not do

- We will not create real Firebase user accounts for children (avoids COPPA email/consent issues).
- We will not store passwords or auth secrets in our own DB beyond optional hashed child PINs.

## Consequences

- Cheap, well-supported, multi-provider auth out of the box.
- Children are protected: no email, no Firebase identity, no third-party tracking.
- We depend on Google/Firebase availability for parent login.
- Backend must keep a small `FirebaseAdmin` integration (service account JSON loaded from env/Key Vault, never committed).
- Migration away from Firebase later would require swapping the token-verification middleware and re-mapping `Parents.FirebaseUid` to a new identity provider's subject claim.
