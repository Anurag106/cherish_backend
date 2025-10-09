-- 08_reactions.sql
-- Creates the reactions table

CREATE TABLE IF NOT EXISTS reactions (
    id BIGSERIAL PRIMARY KEY,
    company_id UUID NOT NULL,
    user_id UUID NOT NULL,
    post_id UUID NOT NULL,
    emoji_type INTEGER NOT NULL CHECK (emoji_type >= 1 AND emoji_type <= 5),
    last_modified_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (company_id) REFERENCES companies(id),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (post_id) REFERENCES posts(id),
    UNIQUE(user_id, post_id) -- One user can only have one reaction per post
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_reactions_company_id ON reactions(company_id);
CREATE INDEX IF NOT EXISTS idx_reactions_user_id ON reactions(user_id);
CREATE INDEX IF NOT EXISTS idx_reactions_post_id ON reactions(post_id);
CREATE INDEX IF NOT EXISTS idx_reactions_emoji_type ON reactions(emoji_type);
CREATE INDEX IF NOT EXISTS idx_reactions_last_modified_at ON reactions(last_modified_at);

COMMENT ON TABLE reactions IS 'Stores user reactions to posts with emoji types.';
COMMENT ON COLUMN reactions.id IS 'Unique identifier for the reaction (BIGSERIAL).';
COMMENT ON COLUMN reactions.company_id IS 'Reference to company.';
COMMENT ON COLUMN reactions.user_id IS 'Reference to user who created the reaction.';
COMMENT ON COLUMN reactions.post_id IS 'Reference to the post being reacted to.';
COMMENT ON COLUMN reactions.emoji_type IS 'Type of reaction emoji (1: Like, 2: Love, 3: Laugh, 4: Angry, 5: Sad).';
COMMENT ON COLUMN reactions.last_modified_at IS 'When the reaction was last modified.';
