-- 10_user_follow_user.sql
-- Creates the user_follow_user table for user-to-user following

CREATE TABLE IF NOT EXISTS user_follow_user (
    company_id UUID NOT NULL,
    follower_user_id UUID NOT NULL,
    followee_user_id UUID NOT NULL,
    last_modified TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_followed BOOLEAN DEFAULT true,
    
    -- Primary key constraint
    PRIMARY KEY (follower_user_id, followee_user_id, company_id),
    
    -- Foreign key constraints
    FOREIGN KEY (company_id) REFERENCES companies(id),
    FOREIGN KEY (follower_user_id) REFERENCES users(id),
    FOREIGN KEY (followee_user_id) REFERENCES users(id),
    
    -- Check constraint to prevent self-following
    CHECK (follower_user_id <> followee_user_id),
    
    -- Check constraint to ensure users are in the same company
    CHECK (
        EXISTS (
            SELECT 1 FROM users u1 
            WHERE u1.id = follower_user_id AND u1.company_id = company_id
        ) AND
        EXISTS (
            SELECT 1 FROM users u2 
            WHERE u2.id = followee_user_id AND u2.company_id = company_id
        )
    )
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_user_follow_user_follower ON user_follow_user(follower_user_id, is_followed);
CREATE INDEX IF NOT EXISTS idx_user_follow_user_followee ON user_follow_user(followee_user_id, is_followed);
CREATE INDEX IF NOT EXISTS idx_user_follow_user_company ON user_follow_user(company_id);
CREATE INDEX IF NOT EXISTS idx_user_follow_user_last_modified ON user_follow_user(last_modified);

-- Add comments
COMMENT ON TABLE user_follow_user IS 'Tracks user-to-user following relationships with soft delete capability';
COMMENT ON COLUMN user_follow_user.company_id IS 'Company context for the follow relationship';
COMMENT ON COLUMN user_follow_user.follower_user_id IS 'User who is following';
COMMENT ON COLUMN user_follow_user.followee_user_id IS 'User being followed';
COMMENT ON COLUMN user_follow_user.last_modified IS 'When the follow status was last updated';
COMMENT ON COLUMN user_follow_user.is_followed IS 'True if following, false if unfollowed (soft delete)';
