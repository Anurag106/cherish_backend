-- Cherish Database Schema - Posts Table
-- This script creates the posts table

CREATE TABLE IF NOT EXISTS posts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    company_id UUID NOT NULL,
    context TEXT NOT NULL,
    user_mentioned JSONB DEFAULT '[]'::jsonb,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    hashtags JSONB DEFAULT '[]'::jsonb,
    metadata JSONB DEFAULT '{}'::jsonb,
    total_points INTEGER DEFAULT 0,
    visibility INTEGER DEFAULT 0,
    deleted BOOLEAN DEFAULT false,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (company_id) REFERENCES companies(id)
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_posts_user_id ON posts(user_id);
CREATE INDEX IF NOT EXISTS idx_posts_company_id ON posts(company_id);
CREATE INDEX IF NOT EXISTS idx_posts_created_at ON posts(created_at);
CREATE INDEX IF NOT EXISTS idx_posts_visibility ON posts(visibility);
CREATE INDEX IF NOT EXISTS idx_posts_total_points ON posts(total_points);
CREATE INDEX IF NOT EXISTS idx_posts_user_mentioned ON posts USING GIN(user_mentioned);
CREATE INDEX IF NOT EXISTS idx_posts_hashtags ON posts USING GIN(hashtags);
CREATE INDEX IF NOT EXISTS idx_posts_metadata ON posts USING GIN(metadata);

COMMENT ON TABLE posts IS 'Stores user posts/feeds with mentions, hashtags, and points';
COMMENT ON COLUMN posts.id IS 'Unique post identifier';
COMMENT ON COLUMN posts.user_id IS 'Reference to user who created the post';
COMMENT ON COLUMN posts.company_id IS 'Reference to company';
COMMENT ON COLUMN posts.context IS 'Post content/context';
COMMENT ON COLUMN posts.user_mentioned IS 'JSON array of mentioned user IDs';
COMMENT ON COLUMN posts.created_at IS 'When the post was created';
COMMENT ON COLUMN posts.hashtags IS 'JSON array of hashtag IDs';
COMMENT ON COLUMN posts.metadata IS 'Additional JSON metadata';
COMMENT ON COLUMN posts.total_points IS 'Total points allocated in this post';
COMMENT ON COLUMN posts.visibility IS 'Post visibility: 0=Public, 1=Team, 2=Private';
