-- Cherish Database Schema - Companies Table
-- This script creates the companies table

CREATE TABLE IF NOT EXISTS companies (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_companies_name ON companies(name);
CREATE INDEX IF NOT EXISTS idx_companies_created_at ON companies(created_at);

COMMENT ON TABLE companies IS 'Stores company information';
COMMENT ON COLUMN companies.id IS 'Unique company identifier';
COMMENT ON COLUMN companies.name IS 'Company name (must be unique)';
COMMENT ON COLUMN companies.created_at IS 'When the company was created';
COMMENT ON COLUMN companies.updated_at IS 'When the company was last updated';
