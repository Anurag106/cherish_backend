# Cherish Backend - Low Level Design

## Table of Contents
1. [System Overview](#system-overview)
2. [Database Schema](#database-schema)
3. [Table Definitions](#table-definitions)
4. [Relationships](#relationships)
5. [Indexes and Performance](#indexes-and-performance)
6. [Data Types and Constraints](#data-types-and-constraints)
7. [API Endpoints](#api-endpoints)
8. [Service Layer Architecture](#service-layer-architecture)

## System Overview

Cherish is a social collaboration platform that enables:
- **User Management**: Multi-tenant with company-based isolation
- **Team Collaboration**: Team-based content sharing and management
- **Social Features**: Posts, comments, reactions, and following
- **Gamification**: Point system for user engagement
- **Content Management**: Posts with hashtags, mentions, and visibility controls

## Database Schema

### Core Entities
- **Companies**: Multi-tenant isolation
- **Users**: Company-scoped user management
- **Teams**: Company-scoped team management
- **Posts**: User-generated content
- **Comments**: Post interactions
- **Reactions**: User engagement tracking
- **Transactions**: Point transfer system
- **Hashtags**: Content categorization
- **Follow Relationships**: User-to-user and user-to-team following

## Table Definitions

### 1. Companies Table
```sql
companies (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
)
```

**Purpose**: Multi-tenant isolation, each company is a separate tenant
**Key Features**:
- Unique company names
- Auto-generated UUIDs
- Timestamp tracking

### 2. Users Table
```sql
users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    role INTEGER DEFAULT 0, -- 0=Employee, 1=Manager, 2=Admin
    status INTEGER DEFAULT 0, -- 0=Active, 1=Inactive, 2=Suspended, 3=Terminated
    team_id UUID,
    department VARCHAR(100),
    job_title VARCHAR(100),
    date_hired DATE,
    date_of_birth DATE,
    total_points INTEGER DEFAULT 0,
    available_points INTEGER DEFAULT 0,
    company_id UUID NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (company_id) REFERENCES companies(id)
)
```

**Purpose**: User management with company isolation
**Key Features**:
- Company-scoped users
- Role-based access control (Employee, Manager, Admin)
- Point system integration
- Optional team membership
- Comprehensive user profile data

### 3. Teams Table
```sql
teams (
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
)
```

**Purpose**: Team management within companies
**Key Features**:
- Company-scoped teams
- Manager assignment
- JSONB array for employee IDs (flexible membership)
- Unique team names per company

### 4. Hashtags Table
```sql
hashtags (
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
)
```

**Purpose**: Content categorization within companies
**Key Features**:
- Company-scoped hashtags
- Audit trail (created/modified by)
- Auto-incrementing integer IDs
- Unique hashtag names per company

### 5. Transactions Table
```sql
transactions (
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
)
```

**Purpose**: Point transfer system between users
**Key Features**:
- Company-scoped transactions
- User-to-user point transfers
- Immutable transaction history
- Optional descriptions

### 6. Posts Table
```sql
posts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    company_id UUID NOT NULL,
    context TEXT NOT NULL,
    user_mentioned JSONB DEFAULT '[]'::jsonb,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    hashtags JSONB DEFAULT '[]'::jsonb,
    metadata JSONB DEFAULT '{}'::jsonb,
    total_points INTEGER DEFAULT 0,
    visibility INTEGER DEFAULT 0, -- 0=Public, 1=Team, 2=Private
    deleted BOOLEAN DEFAULT false,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (company_id) REFERENCES companies(id)
)
```

**Purpose**: User-generated content with social features
**Key Features**:
- Company-scoped posts
- User mentions (JSONB array)
- Hashtag references (JSONB array)
- Visibility controls (Public, Team, Private)
- Soft delete capability
- Point allocation tracking
- Flexible metadata storage

### 7. Comments Table
```sql
comments (
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
)
```

**Purpose**: Post interactions and discussions
**Key Features**:
- Post-specific comments
- Point allocation in comments
- Hashtag support in comments
- Soft delete capability
- Company isolation

### 8. Reactions Table
```sql
reactions (
    id BIGSERIAL PRIMARY KEY,
    company_id UUID NOT NULL,
    user_id UUID NOT NULL,
    post_id UUID NOT NULL,
    emoji_type INTEGER NOT NULL CHECK (emoji_type >= 1 AND emoji_type <= 5),
    last_modified_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (company_id) REFERENCES companies(id),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (post_id) REFERENCES posts(id),
    UNIQUE(user_id, post_id)
)
```

**Purpose**: User engagement tracking on posts
**Key Features**:
- One reaction per user per post
- 5 emoji types (Like, Love, Laugh, Angry, Sad)
- Auto-incrementing big serial IDs
- Update tracking with timestamps

### 9. User Follow User Table
```sql
user_follow_user (
    company_id UUID NOT NULL,
    follower_user_id UUID NOT NULL,
    followee_user_id UUID NOT NULL,
    last_modified TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_followed BOOLEAN DEFAULT true,
    
    PRIMARY KEY (follower_user_id, followee_user_id, company_id),
    FOREIGN KEY (company_id) REFERENCES companies(id),
    FOREIGN KEY (follower_user_id) REFERENCES users(id),
    FOREIGN KEY (followee_user_id) REFERENCES users(id),
    
    CHECK (follower_user_id <> followee_user_id)
)
```

**Purpose**: User-to-user following relationships
**Key Features**:
- Composite primary key
- Soft delete with `is_followed` flag
- Self-follow prevention
- Company-scoped relationships

### 10. User Follow Team Table
```sql
user_follow_team (
    company_id UUID NOT NULL,
    follower_user_id UUID NOT NULL,
    followee_team_id UUID NOT NULL,
    last_modified TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_followed BOOLEAN DEFAULT true,
    
    PRIMARY KEY (follower_user_id, followee_team_id, company_id),
    FOREIGN KEY (company_id) REFERENCES companies(id),
    FOREIGN KEY (follower_user_id) REFERENCES users(id),
    FOREIGN KEY (followee_team_id) REFERENCES teams(id)
)
```

**Purpose**: User-to-team following relationships
**Key Features**:
- Composite primary key
- Soft delete capability
- Company-scoped relationships
- Team membership prevention check

## Relationships

### Primary Relationships
```
Companies (1) ←→ (N) Users
Companies (1) ←→ (N) Teams
Companies (1) ←→ (N) Posts
Companies (1) ←→ (N) Comments
Companies (1) ←→ (N) Reactions
Companies (1) ←→ (N) Hashtags
Companies (1) ←→ (N) Transactions

Users (1) ←→ (N) Posts
Users (1) ←→ (N) Comments
Users (1) ←→ (N) Reactions
Users (1) ←→ (N) Transactions (as from_user)
Users (1) ←→ (N) Transactions (as to_user)
Users (1) ←→ (N) Hashtags (as created_by)
Users (1) ←→ (N) Hashtags (as modified_by)

Teams (1) ←→ (N) Users (as manager)
Teams (1) ←→ (N) User_Follow_Team

Posts (1) ←→ (N) Comments
Posts (1) ←→ (N) Reactions
```

### Many-to-Many Relationships
```
Users ←→ Users (via user_follow_user)
Users ←→ Teams (via user_follow_team)
Posts ←→ Users (via user_mentioned JSONB)
Posts ←→ Hashtags (via hashtags JSONB)
Comments ←→ Hashtags (via hashtags JSONB)
Teams ←→ Users (via employee_ids JSONB)
```

## Indexes and Performance

### Critical Indexes
```sql
-- User lookups
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_company_id ON users(company_id);
CREATE INDEX idx_users_team_id ON users(team_id);

-- Post queries
CREATE INDEX idx_posts_user_id ON posts(user_id);
CREATE INDEX idx_posts_company_id ON posts(company_id);
CREATE INDEX idx_posts_created_at ON posts(created_at);
CREATE INDEX idx_posts_visibility ON posts(visibility);

-- JSONB indexes for complex queries
CREATE INDEX idx_posts_user_mentioned ON posts USING GIN(user_mentioned);
CREATE INDEX idx_posts_hashtags ON posts USING GIN(hashtags);
CREATE INDEX idx_teams_employee_ids ON teams USING GIN(employee_ids);

-- Reaction queries
CREATE INDEX idx_reactions_user_id ON reactions(user_id);
CREATE INDEX idx_reactions_post_id ON reactions(post_id);

-- Follow relationships
CREATE INDEX idx_user_follow_user_follower ON user_follow_user(follower_user_id, is_followed);
CREATE INDEX idx_user_follow_user_followee ON user_follow_user(followee_user_id, is_followed);
```

## Data Types and Constraints

### Key Data Types
- **UUID**: Primary keys for most entities (globally unique)
- **SERIAL/BIGSERIAL**: Auto-incrementing IDs for hashtags and reactions
- **JSONB**: Flexible storage for arrays and metadata
- **TIMESTAMP**: Audit trails and temporal queries
- **INTEGER**: Enums (roles, status, emoji types, visibility)

### Important Constraints
```sql
-- Unique constraints
UNIQUE(username) ON users
UNIQUE(name, company_id) ON teams
UNIQUE(name, company_id) ON hashtags
UNIQUE(user_id, post_id) ON reactions

-- Check constraints
CHECK (emoji_type >= 1 AND emoji_type <= 5) ON reactions
CHECK (follower_user_id <> followee_user_id) ON user_follow_user

-- Foreign key constraints
All tables reference companies(id) for multi-tenancy
```

## API Endpoints

### Authentication & User Management
```
POST /api/v1/auth/login
POST /api/v1/auth/register
GET  /api/v1/user/{id}
PUT  /api/v1/user/{id}
GET  /api/v1/user/company/{companyId}
```

### Team Management
```
POST /api/v1/team/create
GET  /api/v1/team/{id}
GET  /api/v1/team/company/{companyId}
GET  /api/v1/team/manager/{managerId}
GET  /api/v1/team/all
PUT  /api/v1/team/{id}
DELETE /api/v1/team/{id}
POST /api/v1/team/{id}/employees
DELETE /api/v1/team/{id}/employees/{employeeId}
```

### Content Management
```
POST /api/v1/post
GET  /api/v1/post/{id}
GET  /api/v1/post/company
GET  /api/v1/post/user/{userId}
GET  /api/v1/post/mentioned/{userId}
GET  /api/v1/post/hashtag/{hashtagId}
GET  /api/v1/post/all
GET  /api/v1/post (filtered with pagination)
PUT  /api/v1/post/{id}
DELETE /api/v1/post/{id}
GET  /api/v1/post/{id}/details
```

### Comments & Reactions
```
POST /api/v1/comment
GET  /api/v1/comment/{id}
PUT  /api/v1/comment/{id}
DELETE /api/v1/comment/{id}
GET  /api/v1/comment/post/{postId}

POST /api/v1/reaction
DELETE /api/v1/reaction/{postId}
GET  /api/v1/reaction/post/{postId}
GET  /api/v1/reaction/user/{postId}
```

### Following & Social Features
```
POST /api/v1/follow/user
DELETE /api/v1/follow/user/{userId}
POST /api/v1/follow/team
DELETE /api/v1/follow/team/{teamId}
GET  /api/v1/follow/user/followers/{userId}
GET  /api/v1/follow/user/following/{userId}
```

### Hashtags & Transactions
```
POST /api/v1/hashtag
GET  /api/v1/hashtag/{id}
GET  /api/v1/hashtag/company/{companyId}
PUT  /api/v1/hashtag/{id}
DELETE /api/v1/hashtag/{id}

GET  /api/v1/transaction/{id}
GET  /api/v1/transaction/user/{userId}
GET  /api/v1/transaction/company/{companyId}
```

## Service Layer Architecture

### Core Services
```
IAuthService - Authentication and authorization
IUserService - User management operations
ITeamService - Team management with employee details
IPostService - Post creation, filtering, and pagination
ICommentService - Comment management
IReactionService - Reaction handling and counting
IFollowService - Follow relationship management
IHashtagService - Hashtag operations
ITransactionService - Point transfer operations
ITokenService - JWT token management
IContentParsingService - Content parsing for mentions/hashtags
```

### Provider Layer
```
PostgreSQLUserProvider
PostgreSQLTeamProvider
PostgreSQLPostProvider
PostgreSQLCommentProvider
PostgreSQLReactionProvider
PostgreSQLFollowProvider
PostgreSQLHashtagProvider
PostgreSQLTransactionProvider
PostgreSQLCompanyProvider
```

### Key Design Patterns
- **Repository Pattern**: Providers handle data access
- **Service Layer**: Business logic encapsulation
- **Dependency Injection**: Loose coupling
- **Multi-tenancy**: Company-based data isolation
- **JSONB Flexibility**: Dynamic content structure
- **Soft Deletes**: Data retention and audit trails
- **Pagination**: Cursor-based for performance
- **Concurrency Control**: SemaphoreSlim for database operations

## 9. API Specifications

### 9.1 Authentication APIs

#### POST /api/auth/login
**Purpose**: User authentication
```json
Request:
{
  "username": "string",
  "password": "string"
}

Response:
{
  "success": true,
  "data": {
    "token": "string",
    "user": {
      "id": "uuid",
      "username": "string",
      "email": "string",
      "firstName": "string",
      "lastName": "string",
      "role": 1,
      "teamId": "uuid",
      "companyId": "uuid"
    }
  }
}
```

#### POST /api/auth/register
**Purpose**: User registration
```json
Request:
{
  "username": "string",
  "password": "string",
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "teamId": "uuid"
}

Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "username": "string",
    "email": "string",
    "firstName": "string",
    "lastName": "string"
  }
}
```

### 9.2 User Management APIs

#### GET /api/users/{id}
**Purpose**: Get user by ID
```json
Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "username": "string",
    "email": "string",
    "firstName": "string",
    "lastName": "string",
    "role": 1,
    "status": 1,
    "teamId": "uuid",
    "department": "string",
    "jobTitle": "string",
    "dateHired": "2024-01-01",
    "dateOfBirth": "1990-01-01",
    "totalPoints": 100,
    "availablePoints": 50,
    "companyId": "uuid",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

#### GET /api/users/team/{teamId}
**Purpose**: Get users by team
```json
Response:
{
  "success": true,
  "data": [
    {
      "id": "uuid",
      "username": "string",
      "firstName": "string",
      "lastName": "string",
      "role": 1,
      "department": "string",
      "jobTitle": "string",
      "totalPoints": 100,
      "availablePoints": 50
    }
  ]
}
```

#### PUT /api/users/{id}
**Purpose**: Update user profile
```json
Request:
{
  "firstName": "string",
  "lastName": "string",
  "department": "string",
  "jobTitle": "string",
  "dateOfBirth": "1990-01-01"
}

Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "username": "string",
    "firstName": "string",
    "lastName": "string",
    "department": "string",
    "jobTitle": "string",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

### 9.3 Team Management APIs

#### POST /api/teams
**Purpose**: Create new team
```json
Request:
{
  "name": "string",
  "managerId": "uuid",
  "companyId": "uuid"
}

Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "name": "string",
    "managerId": "uuid",
    "companyId": "uuid",
    "employeeIds": ["uuid"],
    "employees": [
      {
        "id": "uuid",
        "fullName": "string"
      }
    ],
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

#### GET /api/teams/{id}
**Purpose**: Get team by ID with employees
```json
Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "name": "string",
    "managerId": "uuid",
    "companyId": "uuid",
    "employeeIds": ["uuid"],
    "employees": [
      {
        "id": "uuid",
        "fullName": "string"
      }
    ],
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

#### GET /api/teams/company/{companyId}
**Purpose**: Get teams by company
```json
Response:
{
  "success": true,
  "data": [
    {
      "id": "uuid",
      "name": "string",
      "managerId": "uuid",
      "companyId": "uuid",
      "employees": [
        {
          "id": "uuid",
          "fullName": "string"
        }
      ],
      "createdAt": "2024-01-01T00:00:00Z"
    }
  ]
}
```

#### PUT /api/teams/{id}
**Purpose**: Update team
```json
Request:
{
  "name": "string",
  "managerId": "uuid"
}

Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "name": "string",
    "managerId": "uuid",
    "companyId": "uuid",
    "employees": [
      {
        "id": "uuid",
        "fullName": "string"
      }
    ],
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

### 9.4 Post Management APIs

#### POST /api/posts
**Purpose**: Create new post
```json
Request:
{
  "context": "string",
  "userMentioned": ["uuid"],
  "hashtags": ["uuid"],
  "totalPoints": 10,
  "visibility": 1
}

Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "userId": "uuid",
    "companyId": "uuid",
    "context": "string",
    "userMentioned": ["uuid"],
    "hashtags": ["uuid"],
    "totalPoints": 10,
    "visibility": 1,
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

#### GET /api/posts/filtered
**Purpose**: Get filtered posts with pagination
```json
Query Parameters:
- pageSize: int (default: 10)
- cursor: string (optional)
- filterByTeam: uuid (optional)
- filterByUserId: uuid (optional)
- hashtagIds: string (comma-separated UUIDs, optional)
- sortOrder: int (0=Newest, 1=Oldest, 2=MostPoints)

Response:
{
  "success": true,
  "data": {
    "posts": [
      {
        "id": "uuid",
        "userId": "uuid",
        "userFullName": "string",
        "companyId": "uuid",
        "context": "string",
        "userMentioned": ["uuid"],
        "hashtags": ["uuid"],
        "totalPoints": 10,
        "visibility": 1,
        "createdAt": "2024-01-01T00:00:00Z",
        "latestComments": [
          {
            "comment": {
              "id": "uuid",
              "userId": "uuid",
              "content": "string",
              "points": 5,
              "createdAt": "2024-01-01T00:00:00Z"
            },
            "userFullName": "string"
          }
        ]
      }
    ],
    "pagination": {
      "hasMore": true,
      "nextCursor": "string",
      "totalCount": 100
    }
  }
}
```

#### GET /api/posts/{id}
**Purpose**: Get post by ID with details
```json
Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "userId": "uuid",
    "userFullName": "string",
    "companyId": "uuid",
    "context": "string",
    "userMentioned": ["uuid"],
    "hashtags": ["uuid"],
    "totalPoints": 10,
    "visibility": 1,
    "createdAt": "2024-01-01T00:00:00Z",
    "latestComments": [
      {
        "comment": {
          "id": "uuid",
          "userId": "uuid",
          "content": "string",
          "points": 5,
          "createdAt": "2024-01-01T00:00:00Z"
        },
        "userFullName": "string"
      }
    ]
  }
}
```

### 9.5 Comment Management APIs

#### POST /api/comments
**Purpose**: Create new comment
```json
Request:
{
  "content": "string",
  "postId": "uuid",
  "postedByAdded": true
}

Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "userId": "uuid",
    "userFullName": "string",
    "postedByAdded": true,
    "companyId": "uuid",
    "content": "string",
    "points": 0,
    "postId": "uuid",
    "hashtags": [],
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

#### GET /api/comments/post/{postId}
**Purpose**: Get comments by post ID with pagination
```json
Query Parameters:
- page: int (default: 1)
- pageSize: int (default: 10)

Response:
{
  "success": true,
  "data": {
    "comments": [
      {
        "id": "uuid",
        "userId": "uuid",
        "userFullName": "string",
        "content": "string",
        "points": 5,
        "createdAt": "2024-01-01T00:00:00Z"
      }
    ],
    "pagination": {
      "page": 1,
      "pageSize": 10,
      "totalCount": 50,
      "totalPages": 5
    }
  }
}
```

#### PUT /api/comments/{id}
**Purpose**: Update comment
```json
Request:
{
  "content": "string"
}

Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "content": "string",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

#### DELETE /api/comments/{id}
**Purpose**: Soft delete comment
```json
Response:
{
  "success": true,
  "message": "Comment deleted successfully"
}
```

### 9.6 Reaction Management APIs

#### POST /api/reactions
**Purpose**: Create or update reaction
```json
Request:
{
  "postId": "uuid",
  "emojiType": "like" // "like", "love", "laugh", "angry", "sad"
}

Response:
{
  "success": true,
  "data": {
    "id": "long",
    "companyId": "uuid",
    "userId": "uuid",
    "postId": "uuid",
    "emojiType": "like",
    "lastModifiedAt": "2024-01-01T00:00:00Z"
  }
}
```

#### GET /api/reactions/post/{postId}/counts
**Purpose**: Get reaction counts for a post
```json
Response:
{
  "success": true,
  "data": {
    "like": 10,
    "love": 5,
    "laugh": 3,
    "angry": 1,
    "sad": 0
  }
}
```

#### DELETE /api/reactions/post/{postId}
**Purpose**: Remove user's reaction from post
```json
Response:
{
  "success": true,
  "message": "Reaction removed successfully"
}
```

### 9.7 Hashtag Management APIs

#### GET /api/hashtags
**Purpose**: Get all hashtags for company
```json
Response:
{
  "success": true,
  "data": [
    {
      "id": "int",
      "name": "string",
      "description": "string",
      "companyId": "uuid",
      "createdBy": "uuid",
      "createdDate": "2024-01-01T00:00:00Z",
      "modifiedBy": "uuid",
      "modifiedDate": "2024-01-01T00:00:00Z"
    }
  ]
}
```

#### POST /api/hashtags
**Purpose**: Create new hashtag
```json
Request:
{
  "name": "string",
  "description": "string"
}

Response:
{
  "success": true,
  "data": {
    "id": "int",
    "name": "string",
    "description": "string",
    "companyId": "uuid",
    "createdBy": "uuid",
    "createdDate": "2024-01-01T00:00:00Z"
  }
}
```

### 9.8 Transaction Management APIs

#### GET /api/transactions/user/{userId}
**Purpose**: Get user's transaction history
```json
Response:
{
  "success": true,
  "data": [
    {
      "id": "uuid",
      "companyId": "uuid",
      "fromUserId": "uuid",
      "toUserId": "uuid",
      "points": 10,
      "description": "string",
      "createdAt": "2024-01-01T00:00:00Z"
    }
  ]
}
```

#### POST /api/transactions
**Purpose**: Create point transaction
```json
Request:
{
  "toUserId": "uuid",
  "points": 10,
  "description": "string"
}

Response:
{
  "success": true,
  "data": {
    "id": "uuid",
    "fromUserId": "uuid",
    "toUserId": "uuid",
    "points": 10,
    "description": "string",
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

### 9.9 Follow Management APIs

#### POST /api/follow/user/{userId}
**Purpose**: Follow/unfollow a user
```json
Response:
{
  "success": true,
  "data": {
    "companyId": "uuid",
    "followerUserId": "uuid",
    "followeeUserId": "uuid",
    "isFollowed": true,
    "lastModified": "2024-01-01T00:00:00Z"
  }
}
```

#### POST /api/follow/team/{teamId}
**Purpose**: Follow/unfollow a team
```json
Response:
{
  "success": true,
  "data": {
    "companyId": "uuid",
    "followerUserId": "uuid",
    "followeeTeamId": "uuid",
    "isFollowed": true,
    "lastModified": "2024-01-01T00:00:00Z"
  }
}
```

#### GET /api/follow/followers
**Purpose**: Get user's followers
```json
Response:
{
  "success": true,
  "data": [
    {
      "id": "uuid",
      "username": "string",
      "firstName": "string",
      "lastName": "string"
    }
  ]
}
```

#### GET /api/follow/following
**Purpose**: Get users that current user follows
```json
Response:
{
  "success": true,
  "data": [
    {
      "id": "uuid",
      "username": "string",
      "firstName": "string",
      "lastName": "string"
    }
  ]
}
```

## 10. API Response Standards

### 10.1 Standard Response Format
All APIs follow a consistent response format:

```json
{
  "success": true|false,
  "data": object|array|null,
  "message": "string",
  "errors": [
    {
      "field": "string",
      "message": "string"
    }
  ]
}
```

### 10.2 Error Handling
- **400 Bad Request**: Invalid input data
- **401 Unauthorized**: Missing or invalid token
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **409 Conflict**: Business rule violation
- **500 Internal Server Error**: Server error

### 10.3 Authentication
- **JWT Token**: Required in Authorization header
- **Format**: `Bearer <token>`
- **Token Expiry**: Configurable (default 24 hours)
- **Company Scoping**: All operations are company-bound

### 10.4 Pagination
- **Cursor-based**: For posts and large datasets
- **Page-based**: For comments and smaller datasets
- **Consistent Format**: Standard pagination metadata

## 11. Performance Considerations

### Database Optimization
- **JSONB Indexes**: GIN indexes for array/meta queries
- **Composite Indexes**: Multi-column queries
- **Connection Pooling**: NpgsqlDataSource with controlled concurrency
- **Query Optimization**: Provider-level query optimization

### Caching Strategy
- **Connection Pooling**: Reused database connections
- **Controlled Concurrency**: SemaphoreSlim limiting concurrent operations
- **Efficient Pagination**: Cursor-based pagination for large datasets

### Scalability Features
- **Multi-tenancy**: Company-based data isolation
- **Flexible Schema**: JSONB for evolving requirements
- **Horizontal Scaling**: Stateless service design
- **Async Operations**: Non-blocking I/O throughout

This design provides a robust foundation for a scalable social collaboration platform with comprehensive user management, content sharing, and engagement features.
