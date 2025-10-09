-- Cherish Database Schema - Transactions Table
-- This script creates the transactions table

CREATE TABLE IF NOT EXISTS transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    company_id UUID NOT NULL,
    from_user_id UUID NOT NULL,
    to_user_id UUID NOT NULL,
    points INTEGER NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (company_id) REFERENCES companies(id),
    FOREIGN KEY (from_user_id) REFERENCES users(id),
    FOREIGN KEY (to_user_id) REFERENCES users(id)
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_transactions_company_id ON transactions(company_id);
CREATE INDEX IF NOT EXISTS idx_transactions_from_user_id ON transactions(from_user_id);
CREATE INDEX IF NOT EXISTS idx_transactions_to_user_id ON transactions(to_user_id);
CREATE INDEX IF NOT EXISTS idx_transactions_created_at ON transactions(created_at);
CREATE INDEX IF NOT EXISTS idx_transactions_points ON transactions(points);

COMMENT ON TABLE transactions IS 'Stores point transactions between users';
COMMENT ON COLUMN transactions.id IS 'Unique transaction identifier';
COMMENT ON COLUMN transactions.company_id IS 'Reference to company';
COMMENT ON COLUMN transactions.from_user_id IS 'Reference to user sending points';
COMMENT ON COLUMN transactions.to_user_id IS 'Reference to user receiving points';
COMMENT ON COLUMN transactions.points IS 'Number of points transferred';
COMMENT ON COLUMN transactions.description IS 'Optional description of the transaction';
COMMENT ON COLUMN transactions.created_at IS 'When the transaction was created';
