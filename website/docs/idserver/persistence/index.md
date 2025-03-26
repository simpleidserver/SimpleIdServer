# Persistence

Relational databases are essential for managing data in identity servers. This article outlines the critical data types stored, including identity-related concepts, authentication elements, and cryptographic keys for scalable systems.

## 1. Identity Server Data

* **Users**: User IDs, usernames, emails, and hashed passwords.
* **Groups**: Group names and user memberships (via junction tables).
* **Clients**: Client IDs, secrets, and redirect URIs for apps interacting with the server.

## 2. Authentication Components

* **Form Elements**: Login field details (e.g., username, password) and validation rules.
* **Registration/Authentication Processes**: Workflow steps, policies (e.g., MFA), and event logs.

## 3. Cryptographic Keys

* **Key Storage**: Keys from DataProtectionProvider (e.g., key ID, encrypted key, dates) in a "DataProtectionKeys" table.
* **Purpose**: Encrypt/decrypt authentication cookies for secure sessions.
* **Scalability**: Centralized storage ensures consistency across distributed servers.