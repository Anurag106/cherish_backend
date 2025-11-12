-- ============================================================
-- Schema Alignment Migration for backendv4
-- Aligns backendv4 schema with yogi_code/backend as source of truth
-- ============================================================

-- CRITICAL: This migration removes authentication fields from backendv4
-- Authentication is now exclusively handled by yogi_code/backend

-- ============================================================
-- STEP 1: Backup existing data (IMPORTANT!)
-- ============================================================
CREATE TABLE IF NOT EXISTS users_backup_pre_alignment AS 
SELECT * FROM users;

CREATE TABLE IF NOT EXISTS companies_backup_pre_alignment AS 
SELECT * FROM companies;

-- ============================================================
-- STEP 2: Update users table structure
-- ============================================================

-- Change user ID type to match ASP.NET Identity (string/VARCHAR)
-- NOTE: This requires data migration if UUIDs exist
-- WARNING: This will break existing foreign key relationships!
DO $$
BEGIN
    -- Check if id column is UUID type
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'users' 
        AND column_name = 'id' 
        AND udt_name = 'uuid'
    ) THEN
        -- Add new VARCHAR id column
        ALTER TABLE users ADD COLUMN id_new VARCHAR(450);
        
        -- Copy data (convert UUID to string)
        UPDATE users SET id_new = id::TEXT;
        
        -- Drop old id column and rename
        ALTER TABLE users DROP COLUMN id CASCADE;
        ALTER TABLE users RENAME COLUMN id_new TO id;
        
        -- Add primary key constraint
        ALTER TABLE users ADD PRIMARY KEY (id);
        
        RAISE NOTICE 'User ID type changed from UUID to VARCHAR(450)';
    END IF;
END $$;

-- Remove authentication-related columns (CRITICAL!)
ALTER TABLE users DROP COLUMN IF EXISTS password;
ALTER TABLE users DROP COLUMN IF EXISTS username;  -- Will use email or get from JWT

COMMENT ON TABLE users IS 'User reference table - NO AUTH DATA. Auth handled by yogi_code/backend. This table caches user info for social features.';

-- Remove role and status (use JWT claims instead)
ALTER TABLE users DROP COLUMN IF EXISTS role;
ALTER TABLE users DROP COLUMN IF EXISTS status;

-- Add new columns to align with yogi_code/backend
ALTER TABLE users ADD COLUMN IF NOT EXISTS profile_picture_url VARCHAR(2000);
ALTER TABLE users ADD COLUMN IF NOT EXISTS is_active BOOLEAN DEFAULT true;
ALTER TABLE users ADD COLUMN IF NOT EXISTS user_mode INTEGER DEFAULT 0;
-- 0=Normal, 1=Benefactor, 2=Receiver, 3=Observer (matches Employee.UserMode)

ALTER TABLE users ADD COLUMN IF NOT EXISTS preferred_first_name VARCHAR(100);
ALTER TABLE users ADD COLUMN IF NOT EXISTS employee_status INTEGER DEFAULT 0;
-- 0=Active, 1=Inactive, 2=OnLeave, 3=Terminated, 4=Retired, 5=PendingActivation

-- Add sync tracking
ALTER TABLE users ADD COLUMN IF NOT EXISTS last_synced_at TIMESTAMP;
ALTER TABLE users ADD COLUMN IF NOT EXISTS sync_source VARCHAR(50) DEFAULT 'jwt_claims';
-- 'jwt_claims' = from JWT token, 'api_sync' = from API call, 'manual' = manual entry

-- Update indexes
DROP INDEX IF EXISTS idx_users_username;  -- No longer needed
CREATE INDEX IF NOT EXISTS idx_users_is_active ON users(is_active);
CREATE INDEX IF NOT EXISTS idx_users_employee_status ON users(employee_status);
CREATE INDEX IF NOT EXISTS idx_users_last_synced ON users(last_synced_at);

-- ============================================================
-- STEP 3: Update companies table (align with Tenants)
-- ============================================================

-- Change company ID to match Tenant ID format (already UUID, keep it)
ALTER TABLE companies ADD COLUMN IF NOT EXISTS logo_url VARCHAR(2000);
ALTER TABLE companies ADD COLUMN IF NOT EXISTS is_active BOOLEAN DEFAULT true;
ALTER TABLE companies ADD COLUMN IF NOT EXISTS last_synced_at TIMESTAMP;

COMMENT ON TABLE companies IS 'Company/Tenant reference table - synced from yogi_code/backend Tenants';

-- Update indexes
CREATE INDEX IF NOT EXISTS idx_companies_is_active ON companies(is_active);
CREATE INDEX IF NOT EXISTS idx_companies_last_synced ON companies(last_synced_at);

-- ============================================================
-- STEP 4: Add new sync tracking table
-- ============================================================

CREATE TABLE IF NOT EXISTS sync_operations (
    id SERIAL PRIMARY KEY,
    entity_type VARCHAR(50) NOT NULL,  -- 'user', 'company', 'team', etc.
    entity_id VARCHAR(450) NOT NULL,
    operation VARCHAR(20) NOT NULL,    -- 'create', 'update', 'delete'
    source VARCHAR(50) NOT NULL,       -- 'jwt_claims', 'api_sync', 'webhook'
    sync_status VARCHAR(20) NOT NULL,  -- 'success', 'failed', 'partial'
    error_message TEXT,
    synced_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_sync_operations_entity ON sync_operations(entity_type, entity_id);
CREATE INDEX idx_sync_operations_synced_at ON sync_operations(synced_at);

COMMENT ON TABLE sync_operations IS 'Tracks all sync operations from yogi_code/backend';

-- ============================================================
-- STEP 5: Update foreign key constraints
-- ============================================================

-- Drop and recreate posts foreign key to users
-- (Handle ID type change from UUID to VARCHAR)
ALTER TABLE posts DROP CONSTRAINT IF EXISTS posts_user_id_fkey;
ALTER TABLE posts ALTER COLUMN user_id TYPE VARCHAR(450);
ALTER TABLE posts ADD CONSTRAINT posts_user_id_fkey 
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;

-- Update comments foreign key
ALTER TABLE comments DROP CONSTRAINT IF EXISTS comments_user_id_fkey;
ALTER TABLE comments ALTER COLUMN user_id TYPE VARCHAR(450);
ALTER TABLE comments ADD CONSTRAINT comments_user_id_fkey 
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;

-- Update reactions foreign key
ALTER TABLE reactions DROP CONSTRAINT IF EXISTS reactions_user_id_fkey;
ALTER TABLE reactions ALTER COLUMN user_id TYPE VARCHAR(450);
ALTER TABLE reactions ADD CONSTRAINT reactions_user_id_fkey 
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;

-- Update transactions foreign keys
ALTER TABLE transactions DROP CONSTRAINT IF EXISTS transactions_sender_id_fkey;
ALTER TABLE transactions DROP CONSTRAINT IF EXISTS transactions_recipient_id_fkey;
ALTER TABLE transactions ALTER COLUMN sender_id TYPE VARCHAR(450);
ALTER TABLE transactions ALTER COLUMN recipient_id TYPE VARCHAR(450);
ALTER TABLE transactions ADD CONSTRAINT transactions_sender_id_fkey 
    FOREIGN KEY (sender_id) REFERENCES users(id) ON DELETE CASCADE;
ALTER TABLE transactions ADD CONSTRAINT transactions_recipient_id_fkey 
    FOREIGN KEY (recipient_id) REFERENCES users(id) ON DELETE CASCADE;

-- Update user_follow_user foreign keys
ALTER TABLE user_follow_user DROP CONSTRAINT IF EXISTS user_follow_user_follower_id_fkey;
ALTER TABLE user_follow_user DROP CONSTRAINT IF EXISTS user_follow_user_following_id_fkey;
ALTER TABLE user_follow_user ALTER COLUMN follower_id TYPE VARCHAR(450);
ALTER TABLE user_follow_user ALTER COLUMN following_id TYPE VARCHAR(450);
ALTER TABLE user_follow_user ADD CONSTRAINT user_follow_user_follower_id_fkey 
    FOREIGN KEY (follower_id) REFERENCES users(id) ON DELETE CASCADE;
ALTER TABLE user_follow_user ADD CONSTRAINT user_follow_user_following_id_fkey 
    FOREIGN KEY (following_id) REFERENCES users(id) ON DELETE CASCADE;

-- ============================================================
-- STEP 6: Add helper function for user sync
-- ============================================================

CREATE OR REPLACE FUNCTION upsert_user_from_jwt(
    p_user_id VARCHAR(450),
    p_email VARCHAR(255),
    p_first_name VARCHAR(100),
    p_last_name VARCHAR(100),
    p_tenant_id UUID,
    p_employee_id UUID DEFAULT NULL,
    p_department VARCHAR(100) DEFAULT NULL,
    p_user_mode INTEGER DEFAULT 0,
    p_employee_status INTEGER DEFAULT 0
) RETURNS VOID AS $$
BEGIN
    INSERT INTO users (
        id, email, first_name, last_name, company_id, 
        department, user_mode, employee_status, 
        last_synced_at, sync_source, is_active
    ) VALUES (
        p_user_id, p_email, p_first_name, p_last_name, p_tenant_id,
        p_department, p_user_mode, p_employee_status,
        CURRENT_TIMESTAMP, 'jwt_claims', true
    )
    ON CONFLICT (id) DO UPDATE SET
        email = EXCLUDED.email,
        first_name = EXCLUDED.first_name,
        last_name = EXCLUDED.last_name,
        department = COALESCE(EXCLUDED.department, users.department),
        user_mode = EXCLUDED.user_mode,
        employee_status = EXCLUDED.employee_status,
        last_synced_at = CURRENT_TIMESTAMP,
        updated_at = CURRENT_TIMESTAMP;
    
    -- Log sync operation
    INSERT INTO sync_operations (
        entity_type, entity_id, operation, source, sync_status
    ) VALUES (
        'user', p_user_id, 'upsert', 'jwt_claims', 'success'
    );
END;
$$ LANGUAGE plpgsql;

-- ============================================================
-- STEP 7: Verification queries
-- ============================================================

-- Check updated schema
SELECT 
    column_name, 
    data_type, 
    character_maximum_length,
    is_nullable
FROM information_schema.columns 
WHERE table_name = 'users' 
ORDER BY ordinal_position;

-- Verify no auth data exists
SELECT COUNT(*) as "Users Count", 
       COUNT(DISTINCT company_id) as "Companies Count"
FROM users;

COMMENT ON COLUMN users.id IS 'User ID from yogi_code/backend IdentityUsers (ASP.NET Identity string ID)';
COMMENT ON COLUMN users.password IS 'REMOVED - No auth data in this table';
COMMENT ON COLUMN users.username IS 'REMOVED - Use email or get from JWT';
COMMENT ON COLUMN users.last_synced_at IS 'Last time user data was synced from yogi_code/backend';
COMMENT ON COLUMN users.sync_source IS 'Source of last sync: jwt_claims, api_sync, or manual';

