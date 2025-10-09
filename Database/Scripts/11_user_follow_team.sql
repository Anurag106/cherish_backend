-- 11_user_follow_team.sql
-- Creates the user_follow_team table for user-to-team following

CREATE TABLE IF NOT EXISTS user_follow_team (
    company_id UUID NOT NULL,
    follower_user_id UUID NOT NULL,
    followee_team_id UUID NOT NULL,
    last_modified TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_followed BOOLEAN DEFAULT true,
    
    -- Primary key constraint
    PRIMARY KEY (follower_user_id, followee_team_id, company_id),
    
    -- Foreign key constraints
    FOREIGN KEY (company_id) REFERENCES companies(id),
    FOREIGN KEY (follower_user_id) REFERENCES users(id),
    FOREIGN KEY (followee_team_id) REFERENCES teams(id),
    
    -- Check constraint to prevent following own team
    CHECK (
        NOT EXISTS (
            SELECT 1 FROM users u 
            WHERE u.id = follower_user_id 
            AND u.team_id = followee_team_id
        )
    ),
    
    -- Check constraint to ensure user and team are in the same company
    CHECK (
        EXISTS (
            SELECT 1 FROM users u 
            WHERE u.id = follower_user_id AND u.company_id = company_id
        ) AND
        EXISTS (
            SELECT 1 FROM teams t 
            WHERE t.id = followee_team_id AND t.company_id = company_id
        )
    )
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_user_follow_team_follower ON user_follow_team(follower_user_id, is_followed);
CREATE INDEX IF NOT EXISTS idx_user_follow_team_team ON user_follow_team(followee_team_id, is_followed);
CREATE INDEX IF NOT EXISTS idx_user_follow_team_company ON user_follow_team(company_id);
CREATE INDEX IF NOT EXISTS idx_user_follow_team_last_modified ON user_follow_team(last_modified);

-- Add comments
COMMENT ON TABLE user_follow_team IS 'Tracks user-to-team following relationships with soft delete capability';
COMMENT ON COLUMN user_follow_team.company_id IS 'Company context for the follow relationship';
COMMENT ON COLUMN user_follow_team.follower_user_id IS 'User who is following the team';
COMMENT ON COLUMN user_follow_team.followee_team_id IS 'Team being followed';
COMMENT ON COLUMN user_follow_team.last_modified IS 'When the follow status was last updated';
COMMENT ON COLUMN user_follow_team.is_followed IS 'True if following, false if unfollowed (soft delete)';
