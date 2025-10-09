-- Cherish Database Schema - Hashtags Table
-- This script creates the hashtags table

CREATE TABLE IF NOT EXISTS hashtags (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    company_id UUID NOT NULL,
    created_by UUID NOT NULL,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modified_by UUID,
    modified_date TIMESTAMP,
    FOREIGN KEY (company_id) REFERENCES companies(id),
    FOREIGN KEY (created_by) REFERENCES users(id),
    FOREIGN KEY (modified_by) REFERENCES users(id),
    UNIQUE(name, company_id)
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_hashtags_name ON hashtags(name);
CREATE INDEX IF NOT EXISTS idx_hashtags_company_id ON hashtags(company_id);
CREATE INDEX IF NOT EXISTS idx_hashtags_created_by ON hashtags(created_by);
CREATE INDEX IF NOT EXISTS idx_hashtags_created_date ON hashtags(created_date);

COMMENT ON TABLE hashtags IS 'Stores hashtag information for posts';
COMMENT ON COLUMN hashtags.id IS 'Unique hashtag identifier (auto-increment)';
COMMENT ON COLUMN hashtags.name IS 'Hashtag name (unique within company)';
COMMENT ON COLUMN hashtags.description IS 'Optional description of the hashtag';
COMMENT ON COLUMN hashtags.company_id IS 'Reference to company (hashtags are company-specific)';
COMMENT ON COLUMN hashtags.created_by IS 'Reference to user who created the hashtag';
COMMENT ON COLUMN hashtags.created_date IS 'When the hashtag was created';
COMMENT ON COLUMN hashtags.modified_by IS 'Reference to user who last modified the hashtag';
COMMENT ON COLUMN hashtags.modified_date IS 'When the hashtag was last modified';
