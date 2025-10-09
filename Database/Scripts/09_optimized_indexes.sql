-- 09_optimized_indexes.sql
-- Optimized indexes for post filtering queries

-- Core post indexes
CREATE INDEX IF NOT EXISTS idx_posts_company_created_deleted 
ON posts(company_id, created_at DESC, id DESC) 
WHERE deleted = false;

CREATE INDEX IF NOT EXISTS idx_posts_user_created 
ON posts(user_id, created_at DESC, id DESC) 
WHERE deleted = false;

-- User mention indexes (for JSONB array searches)
CREATE INDEX IF NOT EXISTS idx_posts_user_mentioned_gin 
ON posts USING GIN (user_mentioned) 
WHERE deleted = false;

-- Hashtag indexes (for JSONB array searches)
CREATE INDEX IF NOT EXISTS idx_posts_hashtags_gin 
ON posts USING GIN (hashtags) 
WHERE deleted = false;

-- Reaction indexes
CREATE INDEX IF NOT EXISTS idx_reactions_user_post 
ON reactions(user_id, post_id, last_modified_at DESC);

CREATE INDEX IF NOT EXISTS idx_reactions_post_user 
ON reactions(post_id, user_id);

-- Comment indexes
CREATE INDEX IF NOT EXISTS idx_comments_user_post 
ON comments(user_id, post_id, created_at DESC) 
WHERE deleted = false;

CREATE INDEX IF NOT EXISTS idx_comments_post_user 
ON comments(post_id, user_id) 
WHERE deleted = false;

-- User team indexes
CREATE INDEX IF NOT EXISTS idx_users_team_id 
ON users(team_id) 
WHERE status = 0; -- Only active users

CREATE INDEX IF NOT EXISTS idx_users_company_team 
ON users(company_id, team_id) 
WHERE status = 0;

-- Composite indexes for specific query patterns
CREATE INDEX IF NOT EXISTS idx_posts_company_created_id 
ON posts(company_id, created_at, id) 
WHERE deleted = false;

-- Partial indexes for better performance
CREATE INDEX IF NOT EXISTS idx_posts_company_recent 
ON posts(company_id, created_at DESC, id DESC) 
WHERE deleted = false 
  AND created_at > (CURRENT_TIMESTAMP - INTERVAL '30 days');

-- Index for cursor-based pagination
CREATE INDEX IF NOT EXISTS idx_posts_cursor_pagination 
ON posts(company_id, created_at DESC, id DESC) 
WHERE deleted = false;

COMMENT ON INDEX idx_posts_company_created_deleted IS 'Primary index for company post filtering with cursor pagination';
COMMENT ON INDEX idx_posts_user_mentioned_gin IS 'GIN index for fast JSONB user mention searches';
COMMENT ON INDEX idx_posts_hashtags_gin IS 'GIN index for fast JSONB hashtag searches';
COMMENT ON INDEX idx_reactions_user_post IS 'Index for user reaction queries';
COMMENT ON INDEX idx_comments_user_post IS 'Index for user comment queries';
COMMENT ON INDEX idx_users_team_id IS 'Index for team member lookups';
COMMENT ON INDEX idx_posts_company_recent IS 'Partial index for recent posts (last 30 days)';
COMMENT ON INDEX idx_posts_cursor_pagination IS 'Optimized index for cursor-based pagination';
