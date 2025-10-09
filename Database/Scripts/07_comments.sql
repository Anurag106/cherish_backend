-- Cherish Database Schema - Comments Table
-- This script creates the comments table

CREATE TABLE IF NOT EXISTS comments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    posted_by_added BOOLEAN NOT NULL DEFAULT false,
    company_id UUID NOT NULL,
    content TEXT NOT NULL,
    points INTEGER DEFAULT 0,
    post_id UUID NOT NULL,
    hashtags JSONB DEFAULT '[]'::jsonb,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    metadata JSONB DEFAULT '{}'::jsonb,
    deleted BOOLEAN DEFAULT false,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (company_id) REFERENCES companies(id),
    FOREIGN KEY (post_id) REFERENCES posts(id)
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_comments_user_id ON comments(user_id);
CREATE INDEX IF NOT EXISTS idx_comments_company_id ON comments(company_id);
CREATE INDEX IF NOT EXISTS idx_comments_post_id ON comments(post_id);
CREATE INDEX IF NOT EXISTS idx_comments_created_at ON comments(created_at);
CREATE INDEX IF NOT EXISTS idx_comments_points ON comments(points);
CREATE INDEX IF NOT EXISTS idx_comments_hashtags ON comments USING GIN(hashtags);
CREATE INDEX IF NOT EXISTS idx_comments_metadata ON comments USING GIN(metadata);

COMMENT ON TABLE comments IS 'Stores user comments on posts with mentions, hashtags, and points';
COMMENT ON COLUMN comments.id IS 'Unique comment identifier';
COMMENT ON COLUMN comments.user_id IS 'Reference to user who created the comment';
COMMENT ON COLUMN comments.posted_by_added IS 'Whether the commenter included the post creator in points allocation';
COMMENT ON COLUMN comments.company_id IS 'Reference to company';
COMMENT ON COLUMN comments.content IS 'Comment content/context';
COMMENT ON COLUMN comments.points IS 'Total points allocated in this comment';
COMMENT ON COLUMN comments.post_id IS 'Reference to the post being commented on';
COMMENT ON COLUMN comments.hashtags IS 'JSON array of hashtag IDs mentioned in comment';
COMMENT ON COLUMN comments.created_at IS 'When the comment was created';
COMMENT ON COLUMN comments.metadata IS 'Additional JSON metadata';
