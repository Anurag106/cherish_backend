-- =====================================================
-- Comprehensive Demo Data for Cherish Database
-- =====================================================
-- Companies already exist:
-- Company 1: ec81c1ed-0568-4c5b-8cea-6591ec9ea628
-- Company 2: b057c22e-5332-4326-9216-f3af7a77828f
-- =====================================================

-- =====================================================
-- STEP 1: Create 10 Users per Company (20 total)
-- Note: Creating without team_id first to avoid circular dependency
-- =====================================================

-- Users for Company 1 (ec81c1ed-0568-4c5b-8cea-6591ec9ea628)
INSERT INTO users (id, username, password, email, first_name, last_name, role, status, department, job_title, date_hired, date_of_birth, total_points, available_points, company_id)
VALUES 
    ('11111111-1111-1111-1111-111111111101', 'sarah.johnson', 'password123', 'sarah.johnson@company1.com', 'Sarah', 'Johnson', 1, 0, 'Engineering', 'Engineering Manager', '2022-01-15', '1988-03-20', 5000, 2500, 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628'),
    ('11111111-1111-1111-1111-111111111102', 'alex.brown', 'password123', 'alex.brown@company1.com', 'Alex', 'Brown', 0, 0, 'Engineering', 'Senior Software Engineer', '2022-04-01', '1990-07-15', 3500, 1500, 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628'),
    ('11111111-1111-1111-1111-111111111103', 'sophia.lee', 'password123', 'sophia.lee@company1.com', 'Sophia', 'Lee', 0, 0, 'Engineering', 'Software Engineer', '2023-01-10', '1992-11-08', 2800, 1200, 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628'),
    ('11111111-1111-1111-1111-111111111104', 'mike.chen', 'password123', 'mike.chen@company1.com', 'Mike', 'Chen', 1, 0, 'Sales', 'Sales Director', '2021-06-20', '1985-05-12', 4500, 2000, 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628'),
    ('11111111-1111-1111-1111-111111111105', 'nathan.white', 'password123', 'nathan.white@company1.com', 'Nathan', 'White', 0, 0, 'Sales', 'Senior Account Executive', '2022-05-20', '1989-09-25', 4000, 1800, 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628'),
    ('11111111-1111-1111-1111-111111111106', 'emma.davis', 'password123', 'emma.davis@company1.com', 'Emma', 'Davis', 1, 0, 'Marketing', 'Marketing Lead', '2022-03-10', '1987-12-03', 4000, 1800, 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628'),
    ('11111111-1111-1111-1111-111111111107', 'liam.walker', 'password123', 'liam.walker@company1.com', 'Liam', 'Walker', 0, 0, 'Marketing', 'Content Marketing Manager', '2022-06-05', '1991-04-18', 3300, 1600, 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628'),
    ('11111111-1111-1111-1111-111111111108', 'olivia.garcia', 'password123', 'olivia.garcia@company1.com', 'Olivia', 'Garcia', 0, 0, 'HR', 'HR Manager', '2021-08-15', '1986-02-28', 3800, 1700, 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628'),
    ('11111111-1111-1111-1111-111111111109', 'ryan.taylor', 'password123', 'ryan.taylor@company1.com', 'Ryan', 'Taylor', 0, 0, 'Product', 'Product Manager', '2023-02-15', '1993-06-10', 2500, 1000, 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628'),
    ('11111111-1111-1111-1111-111111111110', 'ava.hall', 'password123', 'ava.hall@company1.com', 'Ava', 'Hall', 0, 0, 'Design', 'UI/UX Designer', '2023-01-20', '1994-08-22', 2600, 1100, 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628');

-- Users for Company 2 (b057c22e-5332-4326-9216-f3af7a77828f)
INSERT INTO users (id, username, password, email, first_name, last_name, role, status, department, job_title, date_hired, date_of_birth, total_points, available_points, company_id)
VALUES 
    ('22222222-2222-2222-2222-222222222201', 'james.wilson', 'password123', 'james.wilson@company2.com', 'James', 'Wilson', 1, 0, 'Engineering', 'Engineering Manager', '2021-09-01', '1987-01-14', 5500, 3000, 'b057c22e-5332-4326-9216-f3af7a77828f'),
    ('22222222-2222-2222-2222-222222222202', 'william.king', 'password123', 'william.king@company2.com', 'William', 'King', 0, 0, 'Engineering', 'Senior Frontend Engineer', '2022-03-15', '1991-05-08', 3800, 1700, 'b057c22e-5332-4326-9216-f3af7a77828f'),
    ('22222222-2222-2222-2222-222222222203', 'amelia.wright', 'password123', 'amelia.wright@company2.com', 'Amelia', 'Wright', 0, 0, 'Engineering', 'Frontend Engineer', '2022-09-01', '1993-10-12', 3000, 1300, 'b057c22e-5332-4326-9216-f3af7a77828f'),
    ('22222222-2222-2222-2222-222222222204', 'lisa.anderson', 'password123', 'lisa.anderson@company2.com', 'Lisa', 'Anderson', 1, 0, 'Sales', 'Sales Manager', '2022-02-14', '1986-08-20', 4200, 2100, 'b057c22e-5332-4326-9216-f3af7a77828f'),
    ('22222222-2222-2222-2222-222222222205', 'lucas.scott', 'password123', 'lucas.scott@company2.com', 'Lucas', 'Scott', 0, 0, 'Sales', 'Regional Sales Lead', '2022-04-10', '1990-03-15', 4200, 2000, 'b057c22e-5332-4326-9216-f3af7a77828f'),
    ('22222222-2222-2222-2222-222222222206', 'david.martinez', 'password123', 'david.martinez@company2.com', 'David', 'Martinez', 1, 0, 'Operations', 'Operations Manager', '2021-11-05', '1988-11-28', 3800, 1900, 'b057c22e-5332-4326-9216-f3af7a77828f'),
    ('22222222-2222-2222-2222-222222222207', 'jackson.nelson', 'password123', 'jackson.nelson@company2.com', 'Jackson', 'Nelson', 0, 0, 'Operations', 'Operations Analyst', '2022-07-01', '1992-07-07', 3000, 1400, 'b057c22e-5332-4326-9216-f3af7a77828f'),
    ('22222222-2222-2222-2222-222222222208', 'evelyn.green', 'password123', 'evelyn.green@company2.com', 'Evelyn', 'Green', 0, 0, 'Marketing', 'Marketing Specialist', '2022-10-05', '1994-01-30', 3400, 1600, 'b057c22e-5332-4326-9216-f3af7a77828f'),
    ('22222222-2222-2222-2222-222222222209', 'benjamin.lopez', 'password123', 'benjamin.lopez@company2.com', 'Benjamin', 'Lopez', 0, 0, 'Product', 'Product Designer', '2023-02-20', '1995-04-17', 2700, 1200, 'b057c22e-5332-4326-9216-f3af7a77828f'),
    ('22222222-2222-2222-2222-222222222210', 'harper.hill', 'password123', 'harper.hill@company2.com', 'Harper', 'Hill', 0, 0, 'Customer Success', 'Customer Success Manager', '2023-07-15', '1993-09-05', 1700, 850, 'b057c22e-5332-4326-9216-f3af7a77828f');

-- =====================================================
-- STEP 2: Create 3 Teams per Company (6 total)
-- =====================================================

-- Teams for Company 1
INSERT INTO teams (id, name, manager_id, company_id, employee_ids)
VALUES 
    ('f1111111-1111-1111-1111-111111111111', 'Backend Development', '11111111-1111-1111-1111-111111111101', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '["11111111-1111-1111-1111-111111111102", "11111111-1111-1111-1111-111111111103"]'::jsonb),
    ('f1111111-1111-1111-1111-111111111112', 'Enterprise Sales', '11111111-1111-1111-1111-111111111104', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '["11111111-1111-1111-1111-111111111105"]'::jsonb),
    ('f1111111-1111-1111-1111-111111111113', 'Digital Marketing', '11111111-1111-1111-1111-111111111106', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '["11111111-1111-1111-1111-111111111107"]'::jsonb);

-- Teams for Company 2
INSERT INTO teams (id, name, manager_id, company_id, employee_ids)
VALUES 
    ('f2222222-2222-2222-2222-222222222221', 'Frontend Development', '22222222-2222-2222-2222-222222222201', 'b057c22e-5332-4326-9216-f3af7a77828f', '["22222222-2222-2222-2222-222222222202", "22222222-2222-2222-2222-222222222203"]'::jsonb),
    ('f2222222-2222-2222-2222-222222222222', 'Regional Sales', '22222222-2222-2222-2222-222222222204', 'b057c22e-5332-4326-9216-f3af7a77828f', '["22222222-2222-2222-2222-222222222205"]'::jsonb),
    ('f2222222-2222-2222-2222-222222222223', 'Operations Support', '22222222-2222-2222-2222-222222222206', 'b057c22e-5332-4326-9216-f3af7a77828f', '["22222222-2222-2222-2222-222222222207"]'::jsonb);

-- =====================================================
-- STEP 3: Update Users with Team Assignments
-- =====================================================

-- Update Company 1 users with team_id
UPDATE users SET team_id = 'f1111111-1111-1111-1111-111111111111' WHERE id IN ('11111111-1111-1111-1111-111111111101', '11111111-1111-1111-1111-111111111102', '11111111-1111-1111-1111-111111111103');
UPDATE users SET team_id = 'f1111111-1111-1111-1111-111111111112' WHERE id IN ('11111111-1111-1111-1111-111111111104', '11111111-1111-1111-1111-111111111105');
UPDATE users SET team_id = 'f1111111-1111-1111-1111-111111111113' WHERE id IN ('11111111-1111-1111-1111-111111111106', '11111111-1111-1111-1111-111111111107');

-- Update Company 2 users with team_id
UPDATE users SET team_id = 'f2222222-2222-2222-2222-222222222221' WHERE id IN ('22222222-2222-2222-2222-222222222201', '22222222-2222-2222-2222-222222222202', '22222222-2222-2222-2222-222222222203');
UPDATE users SET team_id = 'f2222222-2222-2222-2222-222222222222' WHERE id IN ('22222222-2222-2222-2222-222222222204', '22222222-2222-2222-2222-222222222205');
UPDATE users SET team_id = 'f2222222-2222-2222-2222-222222222223' WHERE id IN ('22222222-2222-2222-2222-222222222206', '22222222-2222-2222-2222-222222222207');

-- =====================================================
-- STEP 4: Create 10 Hashtags per Company (20 total)
-- =====================================================

-- Hashtags for Company 1
INSERT INTO hashtags (name, description, company_id, created_by)
VALUES 
    ('teamwork', 'Celebrating collaboration and teamwork', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111101'),
    ('innovation', 'Innovative ideas and solutions', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111101'),
    ('milestone', 'Project milestones and achievements', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111102'),
    ('kudos', 'Recognition and appreciation', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111103'),
    ('leadership', 'Leadership excellence', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111104'),
    ('customerwin', 'Customer success stories', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111105'),
    ('creativity', 'Creative solutions and ideas', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111106'),
    ('growth', 'Personal and professional growth', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111107'),
    ('excellence', 'Excellence in execution', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111108'),
    ('community', 'Building our community', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111109');

-- Hashtags for Company 2
INSERT INTO hashtags (name, description, company_id, created_by)
VALUES 
    ('teamwork', 'Celebrating collaboration and teamwork', 'b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201'),
    ('innovation', 'Innovative ideas and solutions', 'b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201'),
    ('achievement', 'Celebrating achievements', 'b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222202'),
    ('appreciation', 'Showing appreciation', 'b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222203'),
    ('success', 'Success stories', 'b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222204'),
    ('quality', 'Quality work and excellence', 'b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222205'),
    ('initiative', 'Taking initiative', 'b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222206'),
    ('collaboration', 'Working together', 'b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222207'),
    ('impact', 'Making an impact', 'b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222208'),
    ('dedication', 'Dedication and commitment', 'b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222209');

-- =====================================================
-- STEP 5: Create 10 Posts per Company (20 total)
-- =====================================================

-- Posts for Company 1
INSERT INTO posts (id, user_id, company_id, context, user_mentioned, hashtags, total_points, visibility)
VALUES 
    ('aaaaaaaa-1111-1111-1111-000000000001', '11111111-1111-1111-1111-111111111101', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Big shoutout to @alex.brown for the amazing work on the new API! Your dedication to quality is inspiring! 🚀', 
     '["11111111-1111-1111-1111-111111111102"]'::jsonb, '[1, 2]'::jsonb, 100, 0),
    
    ('aaaaaaaa-1111-1111-1111-000000000002', '11111111-1111-1111-1111-111111111102', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Excited to share that we hit our Q4 milestone! Thanks to the entire engineering team for the incredible teamwork! 🎯', 
     '["11111111-1111-1111-1111-111111111103"]'::jsonb, '[1, 3]'::jsonb, 150, 0),
    
    ('aaaaaaaa-1111-1111-1111-000000000003', '11111111-1111-1111-1111-111111111104', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Huge congratulations to @nathan.white for closing the biggest deal of the quarter! Your persistence paid off! 💪', 
     '["11111111-1111-1111-1111-111111111105"]'::jsonb, '[4, 6]'::jsonb, 200, 0),
    
    ('aaaaaaaa-1111-1111-1111-000000000004', '11111111-1111-1111-1111-111111111106', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'The marketing campaign results are in and they are phenomenal! Thanks @liam.walker for your creative genius! 🎨', 
     '["11111111-1111-1111-1111-111111111107"]'::jsonb, '[7, 8]'::jsonb, 120, 0),
    
    ('aaaaaaaa-1111-1111-1111-000000000005', '11111111-1111-1111-1111-111111111108', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Want to recognize @sophia.lee and @ryan.taylor for their outstanding collaboration on the product roadmap. Great teamwork! 👏', 
     '["11111111-1111-1111-1111-111111111103", "11111111-1111-1111-1111-111111111109"]'::jsonb, '[1, 4]'::jsonb, 150, 0),
    
    ('aaaaaaaa-1111-1111-1111-000000000006', '11111111-1111-1111-1111-111111111103', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Just deployed the new feature to production! Thanks to everyone who contributed to this milestone! 🎉', 
     '[]'::jsonb, '[3, 9]'::jsonb, 80, 0),
    
    ('aaaaaaaa-1111-1111-1111-000000000007', '11111111-1111-1111-1111-111111111105', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Grateful to work with such an amazing team! @ava.hall your design work continues to blow me away! ✨', 
     '["11111111-1111-1111-1111-111111111110"]'::jsonb, '[4, 7]'::jsonb, 100, 0),
    
    ('aaaaaaaa-1111-1111-1111-000000000008', '11111111-1111-1111-1111-111111111107', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Our content strategy is paying off - 300% increase in engagement! Team effort all the way! 📈', 
     '[]'::jsonb, '[8, 10]'::jsonb, 100, 0),
    
    ('aaaaaaaa-1111-1111-1111-000000000009', '11111111-1111-1111-1111-111111111109', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Product launch was a huge success! Thanks @sarah.johnson for your leadership and vision! 🌟', 
     '["11111111-1111-1111-1111-111111111101"]'::jsonb, '[5, 9]'::jsonb, 180, 0),
    
    ('aaaaaaaa-1111-1111-1111-000000000010', '11111111-1111-1111-1111-111111111110', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Celebrating our team culture! @olivia.garcia has done an incredible job fostering growth and community! 🎊', 
     '["11111111-1111-1111-1111-111111111108"]'::jsonb, '[8, 10]'::jsonb, 140, 0);

-- Posts for Company 2
INSERT INTO posts (id, user_id, company_id, context, user_mentioned, hashtags, total_points, visibility)
VALUES 
    ('bbbbbbbb-2222-2222-2222-000000000001', '22222222-2222-2222-2222-222222222201', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Incredible work by @william.king on the UI refactoring! The attention to detail is outstanding! 🎯', 
     '["22222222-2222-2222-2222-222222222202"]'::jsonb, '[11, 12]'::jsonb, 120, 0),
    
    ('bbbbbbbb-2222-2222-2222-000000000002', '22222222-2222-2222-2222-222222222202', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Major achievement unlocked! Our app now has 1M+ users! Thanks @amelia.wright for the amazing frontend work! 🚀', 
     '["22222222-2222-2222-2222-222222222203"]'::jsonb, '[13, 15]'::jsonb, 200, 0),
    
    ('bbbbbbbb-2222-2222-2222-000000000003', '22222222-2222-2222-2222-222222222204', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Sales team crushed it this month! @lucas.scott led by example with exceptional results! 💪', 
     '["22222222-2222-2222-2222-222222222205"]'::jsonb, '[15, 16]'::jsonb, 180, 0),
    
    ('bbbbbbbb-2222-2222-2222-000000000004', '22222222-2222-2222-2222-222222222206', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Operations excellence! @jackson.nelson optimized our processes and saved us 20 hours/week! ⚡', 
     '["22222222-2222-2222-2222-222222222207"]'::jsonb, '[16, 17]'::jsonb, 150, 0),
    
    ('bbbbbbbb-2222-2222-2222-000000000005', '22222222-2222-2222-2222-222222222208', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Marketing campaign exceeded all expectations! Great collaboration between @evelyn.green and @benjamin.lopez! 🎨', 
     '["22222222-2222-2222-2222-222222222208", "22222222-2222-2222-2222-222222222209"]'::jsonb, '[18, 19]'::jsonb, 160, 0),
    
    ('bbbbbbbb-2222-2222-2222-000000000006', '22222222-2222-2222-2222-222222222203', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Shipped 5 major features this sprint! Team collaboration at its finest! 🎉', 
     '[]'::jsonb, '[11, 18]'::jsonb, 100, 0),
    
    ('bbbbbbbb-2222-2222-2222-000000000007', '22222222-2222-2222-2222-222222222205', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Customer feedback has been amazing! Thanks @harper.hill for your dedication to customer success! 🌟', 
     '["22222222-2222-2222-2222-222222222210"]'::jsonb, '[14, 19]'::jsonb, 130, 0),
    
    ('bbbbbbbb-2222-2222-2222-000000000008', '22222222-2222-2222-2222-222222222209', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Design system launch was a success! This will improve our product quality across the board! 🎨', 
     '[]'::jsonb, '[16, 19]'::jsonb, 110, 0),
    
    ('bbbbbbbb-2222-2222-2222-000000000009', '22222222-2222-2222-2222-222222222207', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Process improvement initiative complete! Thanks @david.martinez for the leadership! 📊', 
     '["22222222-2222-2222-2222-222222222206"]'::jsonb, '[17, 20]'::jsonb, 140, 0),
    
    ('bbbbbbbb-2222-2222-2222-000000000010', '22222222-2222-2222-2222-222222222210', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Customer satisfaction score at all-time high! Proud of this team and their dedication! 🏆', 
     '[]'::jsonb, '[19, 20]'::jsonb, 150, 0);

-- =====================================================
-- STEP 6: Create Comments on Posts (30 comments)
-- =====================================================

-- Comments for Company 1 Posts
INSERT INTO comments (id, user_id, company_id, content, points, post_id, hashtags)
VALUES 
    ('cccccccc-1111-1111-1111-000000000001', '11111111-1111-1111-1111-111111111103', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Totally deserved! Alex is a rockstar! 🌟', 50, 'aaaaaaaa-1111-1111-1111-000000000001', '[4]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000002', '11111111-1111-1111-1111-111111111104', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Congrats team! This is what great collaboration looks like!', 0, 'aaaaaaaa-1111-1111-1111-000000000002', '[1]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000003', '11111111-1111-1111-1111-111111111105', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Thank you! It was a team effort, couldn''t have done it without support from everyone! 🙏', 0, 'aaaaaaaa-1111-1111-1111-000000000003', '[1, 4]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000004', '11111111-1111-1111-1111-111111111107', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Thanks Emma! Great to work with you on this campaign! 🎉', 0, 'aaaaaaaa-1111-1111-1111-000000000004', '[7]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000005', '11111111-1111-1111-1111-111111111102', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Amazing work everyone! So proud to be part of this team!', 0, 'aaaaaaaa-1111-1111-1111-000000000005', '[1]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000006', '11111111-1111-1111-1111-111111111101', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Excellent execution Sophia! Keep up the great work! 👏', 80, 'aaaaaaaa-1111-1111-1111-000000000006', '[9]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000007', '11111111-1111-1111-1111-111111111110', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Thank you so much! Love collaborating with you! ❤️', 0, 'aaaaaaaa-1111-1111-1111-000000000007', '[7]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000008', '11111111-1111-1111-1111-111111111106', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Fantastic results! This is the power of great content! 🚀', 0, 'aaaaaaaa-1111-1111-1111-000000000008', '[8]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000009', '11111111-1111-1111-1111-111111111101', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Thank you Ryan! The whole team made this happen! 🎊', 0, 'aaaaaaaa-1111-1111-1111-000000000009', '[1, 5]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000010', '11111111-1111-1111-1111-111111111109', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Olivia you are amazing! Thanks for everything you do! 💖', 60, 'aaaaaaaa-1111-1111-1111-000000000010', '[8, 10]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000011', '11111111-1111-1111-1111-111111111108', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'So proud of this team! Let''s keep this momentum going! 💪', 0, 'aaaaaaaa-1111-1111-1111-000000000002', '[1]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000012', '11111111-1111-1111-1111-111111111106', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Great collaboration all around! This is what makes us special!', 0, 'aaaaaaaa-1111-1111-1111-000000000005', '[1, 10]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000013', '11111111-1111-1111-1111-111111111107', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Team effort for the win! 🏆', 0, 'aaaaaaaa-1111-1111-1111-000000000006', '[1]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000014', '11111111-1111-1111-1111-111111111109', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'Love working with talented people like you all! ✨', 0, 'aaaaaaaa-1111-1111-1111-000000000008', '[8]'::jsonb),
    
    ('cccccccc-1111-1111-1111-000000000015', '11111111-1111-1111-1111-111111111102', 'ec81c1ed-0568-4c5b-8cea-6591ec9ea628', 
     'This is inspiring! Great work everyone! 🌟', 0, 'aaaaaaaa-1111-1111-1111-000000000009', '[9]'::jsonb);

-- Comments for Company 2 Posts
INSERT INTO comments (id, user_id, company_id, content, points, post_id, hashtags)
VALUES 
    ('dddddddd-2222-2222-2222-000000000001', '22222222-2222-2222-2222-222222222203', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'William is amazing! So lucky to work with him! 🎨', 70, 'bbbbbbbb-2222-2222-2222-000000000001', '[14]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000002', '22222222-2222-2222-2222-222222222201', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Milestone achievement! Proud of everyone! 🎉', 0, 'bbbbbbbb-2222-2222-2222-000000000002', '[13]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000003', '22222222-2222-2222-2222-222222222205', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Thanks Lisa! Great leadership as always! 🙌', 0, 'bbbbbbbb-2222-2222-2222-000000000003', '[15]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000004', '22222222-2222-2222-2222-222222222207', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Thanks David! Always happy to help optimize our processes! ⚙️', 0, 'bbbbbbbb-2222-2222-2222-000000000004', '[17]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000005', '22222222-2222-2222-2222-222222222209', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Thank you! Collaboration makes everything better! 🤝', 0, 'bbbbbbbb-2222-2222-2222-000000000005', '[18]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000006', '22222222-2222-2222-2222-222222222201', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Incredible sprint! Keep up the excellent work team! 💪', 90, 'bbbbbbbb-2222-2222-2222-000000000006', '[11]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000007', '22222222-2222-2222-2222-222222222210', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Thank you Lucas! It''s all about the customers! ❤️', 0, 'bbbbbbbb-2222-2222-2222-000000000007', '[19]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000008', '22222222-2222-2222-2222-222222222202', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Design system is a game changer! Great work! 🎨', 0, 'bbbbbbbb-2222-2222-2222-000000000008', '[16]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000009', '22222222-2222-2222-2222-222222222206', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Thanks Jackson! Your work makes a real difference! 🌟', 0, 'bbbbbbbb-2222-2222-2222-000000000009', '[17]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000010', '22222222-2222-2222-2222-222222222204', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Outstanding work Harper! Customer satisfaction is everything! 🏆', 100, 'bbbbbbbb-2222-2222-2222-000000000010', '[19, 20]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000011', '22222222-2222-2222-2222-222222222208', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'This is what teamwork looks like! 💯', 0, 'bbbbbbbb-2222-2222-2222-000000000002', '[11]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000012', '22222222-2222-2222-2222-222222222203', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Great results everyone! Let''s keep it up! 🚀', 0, 'bbbbbbbb-2222-2222-2222-000000000003', '[15]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000013', '22222222-2222-2222-2222-222222222206', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Efficiency improvements are paying off! Nice work team! ⚡', 0, 'bbbbbbbb-2222-2222-2222-000000000004', '[17]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000014', '22222222-2222-2222-2222-222222222207', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Marketing and product working together perfectly! 🎯', 0, 'bbbbbbbb-2222-2222-2222-000000000005', '[18]'::jsonb),
    
    ('dddddddd-2222-2222-2222-000000000015', '22222222-2222-2222-2222-222222222209', 'b057c22e-5332-4326-9216-f3af7a77828f', 
     'Design quality is top notch! Proud of this work! ✨', 0, 'bbbbbbbb-2222-2222-2222-000000000008', '[16]'::jsonb);

-- =====================================================
-- STEP 7: Create Reactions on Posts (60 reactions)
-- =====================================================

-- Reactions for Company 1 Posts (multiple reactions per post)
INSERT INTO reactions (company_id, user_id, post_id, emoji_type)
VALUES 
    -- Post 1 reactions
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111103', 'aaaaaaaa-1111-1111-1111-000000000001', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111104', 'aaaaaaaa-1111-1111-1111-000000000001', 2),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111105', 'aaaaaaaa-1111-1111-1111-000000000001', 1),
    -- Post 2 reactions
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111101', 'aaaaaaaa-1111-1111-1111-000000000002', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111104', 'aaaaaaaa-1111-1111-1111-000000000002', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111106', 'aaaaaaaa-1111-1111-1111-000000000002', 2),
    -- Post 3 reactions
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111101', 'aaaaaaaa-1111-1111-1111-000000000003', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111102', 'aaaaaaaa-1111-1111-1111-000000000003', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111103', 'aaaaaaaa-1111-1111-1111-000000000003', 2),
    -- Post 4 reactions
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111101', 'aaaaaaaa-1111-1111-1111-000000000004', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111107', 'aaaaaaaa-1111-1111-1111-000000000004', 2),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111108', 'aaaaaaaa-1111-1111-1111-000000000004', 1),
    -- Post 5 reactions
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111101', 'aaaaaaaa-1111-1111-1111-000000000005', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111102', 'aaaaaaaa-1111-1111-1111-000000000005', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111104', 'aaaaaaaa-1111-1111-1111-000000000005', 2),
    -- Post 6 reactions
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111101', 'aaaaaaaa-1111-1111-1111-000000000006', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111102', 'aaaaaaaa-1111-1111-1111-000000000006', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111105', 'aaaaaaaa-1111-1111-1111-000000000006', 3),
    -- Post 7 reactions
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111106', 'aaaaaaaa-1111-1111-1111-000000000007', 2),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111107', 'aaaaaaaa-1111-1111-1111-000000000007', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111110', 'aaaaaaaa-1111-1111-1111-000000000007', 2),
    -- Post 8 reactions
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111101', 'aaaaaaaa-1111-1111-1111-000000000008', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111106', 'aaaaaaaa-1111-1111-1111-000000000008', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111108', 'aaaaaaaa-1111-1111-1111-000000000008', 1),
    -- Post 9 reactions
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111102', 'aaaaaaaa-1111-1111-1111-000000000009', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111103', 'aaaaaaaa-1111-1111-1111-000000000009', 2),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111104', 'aaaaaaaa-1111-1111-1111-000000000009', 1),
    -- Post 10 reactions
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111101', 'aaaaaaaa-1111-1111-1111-000000000010', 1),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111106', 'aaaaaaaa-1111-1111-1111-000000000010', 2),
    ('ec81c1ed-0568-4c5b-8cea-6591ec9ea628', '11111111-1111-1111-1111-111111111108', 'aaaaaaaa-1111-1111-1111-000000000010', 1);

-- Reactions for Company 2 Posts
INSERT INTO reactions (company_id, user_id, post_id, emoji_type)
VALUES 
    -- Post 1 reactions
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222202', 'bbbbbbbb-2222-2222-2222-000000000001', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222203', 'bbbbbbbb-2222-2222-2222-000000000001', 2),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222204', 'bbbbbbbb-2222-2222-2222-000000000001', 1),
    -- Post 2 reactions
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201', 'bbbbbbbb-2222-2222-2222-000000000002', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222204', 'bbbbbbbb-2222-2222-2222-000000000002', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222205', 'bbbbbbbb-2222-2222-2222-000000000002', 2),
    -- Post 3 reactions
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201', 'bbbbbbbb-2222-2222-2222-000000000003', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222202', 'bbbbbbbb-2222-2222-2222-000000000003', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222205', 'bbbbbbbb-2222-2222-2222-000000000003', 1),
    -- Post 4 reactions
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201', 'bbbbbbbb-2222-2222-2222-000000000004', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222204', 'bbbbbbbb-2222-2222-2222-000000000004', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222207', 'bbbbbbbb-2222-2222-2222-000000000004', 2),
    -- Post 5 reactions
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201', 'bbbbbbbb-2222-2222-2222-000000000005', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222203', 'bbbbbbbb-2222-2222-2222-000000000005', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222209', 'bbbbbbbb-2222-2222-2222-000000000005', 2),
    -- Post 6 reactions
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201', 'bbbbbbbb-2222-2222-2222-000000000006', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222202', 'bbbbbbbb-2222-2222-2222-000000000006', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222204', 'bbbbbbbb-2222-2222-2222-000000000006', 3),
    -- Post 7 reactions
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201', 'bbbbbbbb-2222-2222-2222-000000000007', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222206', 'bbbbbbbb-2222-2222-2222-000000000007', 2),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222210', 'bbbbbbbb-2222-2222-2222-000000000007', 1),
    -- Post 8 reactions
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201', 'bbbbbbbb-2222-2222-2222-000000000008', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222202', 'bbbbbbbb-2222-2222-2222-000000000008', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222203', 'bbbbbbbb-2222-2222-2222-000000000008', 2),
    -- Post 9 reactions
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201', 'bbbbbbbb-2222-2222-2222-000000000009', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222206', 'bbbbbbbb-2222-2222-2222-000000000009', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222207', 'bbbbbbbb-2222-2222-2222-000000000009', 1),
    -- Post 10 reactions
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222201', 'bbbbbbbb-2222-2222-2222-000000000010', 1),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222204', 'bbbbbbbb-2222-2222-2222-000000000010', 2),
    ('b057c22e-5332-4326-9216-f3af7a77828f', '22222222-2222-2222-2222-222222222210', 'bbbbbbbb-2222-2222-2222-000000000010', 1);

-- =====================================================
-- Summary:
-- - 20 Users (10 per company)
-- - 6 Teams (3 per company) with managers and members
-- - Users assigned to teams via UPDATE after team creation
-- - 20 Hashtags (10 per company)
-- - 20 Posts (10 per company) with hashtags and mentions
-- - 30 Comments (15 per company)
-- - 60 Reactions (30 per company, 3 per post average)
-- =====================================================
