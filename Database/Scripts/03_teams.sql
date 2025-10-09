-- Cherish Database Schema - Teams Table
-- This script creates the teams table

CREATE TABLE IF NOT EXISTS teams (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    manager_id UUID NOT NULL,
    company_id UUID NOT NULL,
    employee_ids JSONB DEFAULT '[]'::jsonb,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (manager_id) REFERENCES users(id),
    FOREIGN KEY (company_id) REFERENCES companies(id),
    UNIQUE(name, company_id)
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_teams_name ON teams(name);
CREATE INDEX IF NOT EXISTS idx_teams_company_id ON teams(company_id);
CREATE INDEX IF NOT EXISTS idx_teams_manager_id ON teams(manager_id);
CREATE INDEX IF NOT EXISTS idx_teams_employee_ids ON teams USING GIN(employee_ids);

COMMENT ON TABLE teams IS 'Stores team information with employee lists';
COMMENT ON COLUMN teams.id IS 'Unique team identifier';
COMMENT ON COLUMN teams.name IS 'Team name (unique within company)';
COMMENT ON COLUMN teams.manager_id IS 'Reference to team manager (user)';
COMMENT ON COLUMN teams.company_id IS 'Reference to company';
COMMENT ON COLUMN teams.employee_ids IS 'JSON array of employee user IDs';
COMMENT ON COLUMN teams.created_at IS 'When the team was created';
COMMENT ON COLUMN teams.updated_at IS 'When the team was last updated';
