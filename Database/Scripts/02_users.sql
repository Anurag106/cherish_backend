-- Cherish Database Schema - Users Table
-- This script creates the users table with all required fields

CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    role INTEGER DEFAULT 0, -- 0=Employee, 1=Manager, 2=Admin
    status INTEGER DEFAULT 0, -- 0=Active, 1=Inactive, 2=Suspended, 3=Terminated
    team_id UUID,
    department VARCHAR(100),
    job_title VARCHAR(100),
    date_hired DATE,
    date_of_birth DATE,
    total_points INTEGER DEFAULT 0,
    available_points INTEGER DEFAULT 0,
    company_id UUID NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (company_id) REFERENCES companies(id)
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);
CREATE INDEX IF NOT EXISTS idx_users_company_id ON users(company_id);
CREATE INDEX IF NOT EXISTS idx_users_team_id ON users(team_id);
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_users_role ON users(role);
CREATE INDEX IF NOT EXISTS idx_users_status ON users(status);

COMMENT ON TABLE users IS 'Stores user information and authentication data';
COMMENT ON COLUMN users.id IS 'Unique user identifier';
COMMENT ON COLUMN users.username IS 'Unique username for login';
COMMENT ON COLUMN users.password IS 'Hashed password (plain text for now)';
COMMENT ON COLUMN users.email IS 'User email address';
COMMENT ON COLUMN users.first_name IS 'User first name';
COMMENT ON COLUMN users.last_name IS 'User last name';
COMMENT ON COLUMN users.role IS 'User role: 0=Employee, 1=Manager, 2=Admin';
COMMENT ON COLUMN users.status IS 'User status: 0=Active, 1=Inactive, 2=Suspended, 3=Terminated';
COMMENT ON COLUMN users.team_id IS 'Reference to team (nullable)';
COMMENT ON COLUMN users.department IS 'User department';
COMMENT ON COLUMN users.job_title IS 'User job title';
COMMENT ON COLUMN users.date_hired IS 'Date when user was hired';
COMMENT ON COLUMN users.date_of_birth IS 'User date of birth';
COMMENT ON COLUMN users.total_points IS 'Total points earned by user';
COMMENT ON COLUMN users.available_points IS 'Available points for spending';
COMMENT ON COLUMN users.company_id IS 'Reference to company';
COMMENT ON COLUMN users.created_at IS 'When the user was created';
COMMENT ON COLUMN users.updated_at IS 'When the user was last updated';
