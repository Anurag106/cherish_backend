# Cherish Database Schema Diagram

## Entity Relationship Diagram

```mermaid
erDiagram
    COMPANIES {
        UUID id PK
        VARCHAR name UK
        TIMESTAMP created_at
        TIMESTAMP updated_at
    }

    USERS {
        UUID id PK
        VARCHAR username UK
        VARCHAR password
        VARCHAR email
        VARCHAR first_name
        VARCHAR last_name
        INTEGER role
        INTEGER status
        UUID team_id FK
        VARCHAR department
        VARCHAR job_title
        DATE date_hired
        DATE date_of_birth
        INTEGER total_points
        INTEGER available_points
        UUID company_id FK
        TIMESTAMP created_at
        TIMESTAMP updated_at
    }

    TEAMS {
        UUID id PK
        VARCHAR name
        UUID manager_id FK
        UUID company_id FK
        JSONB employee_ids
        TIMESTAMP created_at
        TIMESTAMP updated_at
    }

    HASHTAGS {
        SERIAL id PK
        VARCHAR name
        TEXT description
        UUID company_id FK
        UUID created_by FK
        TIMESTAMP created_date
        UUID modified_by FK
        TIMESTAMP modified_date
    }

    POSTS {
        UUID id PK
        UUID user_id FK
        UUID company_id FK
        TEXT context
        JSONB user_mentioned
        TIMESTAMP created_at
        JSONB hashtags
        JSONB metadata
        INTEGER total_points
        INTEGER visibility
        BOOLEAN deleted
    }

    COMMENTS {
        UUID id PK
        UUID user_id FK
        BOOLEAN posted_by_added
        UUID company_id FK
        TEXT content
        INTEGER points
        UUID post_id FK
        JSONB hashtags
        TIMESTAMP created_at
        JSONB metadata
        BOOLEAN deleted
    }

    REACTIONS {
        BIGSERIAL id PK
        UUID company_id FK
        UUID user_id FK
        UUID post_id FK
        INTEGER emoji_type
        TIMESTAMP last_modified_at
    }

    TRANSACTIONS {
        UUID id PK
        UUID company_id FK
        UUID from_user_id FK
        UUID to_user_id FK
        INTEGER points
        TEXT description
        TIMESTAMP created_at
    }

    USER_FOLLOW_USER {
        UUID company_id FK
        UUID follower_user_id FK
        UUID followee_user_id FK
        TIMESTAMP last_modified
        BOOLEAN is_followed
    }

    USER_FOLLOW_TEAM {
        UUID company_id FK
        UUID follower_user_id FK
        UUID followee_team_id FK
        TIMESTAMP last_modified
        BOOLEAN is_followed
    }

    %% Primary Relationships
    COMPANIES ||--o{ USERS : "has"
    COMPANIES ||--o{ TEAMS : "has"
    COMPANIES ||--o{ HASHTAGS : "has"
    COMPANIES ||--o{ POSTS : "contains"
    COMPANIES ||--o{ COMMENTS : "contains"
    COMPANIES ||--o{ REACTIONS : "contains"
    COMPANIES ||--o{ TRANSACTIONS : "contains"
    COMPANIES ||--o{ USER_FOLLOW_USER : "scopes"
    COMPANIES ||--o{ USER_FOLLOW_TEAM : "scopes"

    USERS ||--o{ TEAMS : "manages"
    USERS ||--o{ POSTS : "creates"
    USERS ||--o{ COMMENTS : "creates"
    USERS ||--o{ REACTIONS : "creates"
    USERS ||--o{ TRANSACTIONS : "from_user"
    USERS ||--o{ TRANSACTIONS : "to_user"
    USERS ||--o{ HASHTAGS : "created_by"
    USERS ||--o{ HASHTAGS : "modified_by"

    TEAMS ||--o{ USER_FOLLOW_TEAM : "followed_by"

    POSTS ||--o{ COMMENTS : "has"
    POSTS ||--o{ REACTIONS : "has"

    %% Many-to-Many Relationships
    USERS ||--o{ USER_FOLLOW_USER : "follower"
    USERS ||--o{ USER_FOLLOW_USER : "followee"
    USERS ||--o{ USER_FOLLOW_TEAM : "follower"
```

## Data Flow Diagram

```mermaid
graph TD
    A[Client Request] --> B[API Controller]
    B --> C[Service Layer]
    C --> D[Provider Layer]
    D --> E[PostgreSQL Database]
    
    C --> F[Business Logic]
    C --> G[Validation]
    C --> H[Authentication]
    
    D --> I[SQL Queries]
    D --> J[Data Mapping]
    D --> K[Connection Management]
    
    E --> L[Companies Table]
    E --> M[Users Table]
    E --> N[Teams Table]
    E --> O[Posts Table]
    E --> P[Comments Table]
    E --> Q[Reactions Table]
    E --> R[Other Tables]
    
    L --> S[Multi-tenant Isolation]
    M --> S
    N --> S
    O --> S
    P --> S
    Q --> S
    R --> S
```

## System Architecture Diagram

```mermaid
graph TB
    subgraph "Client Layer"
        A[Web Client]
        B[Mobile Client]
        C[API Client]
    end
    
    subgraph "API Gateway"
        D[Load Balancer]
        E[Authentication]
        F[Rate Limiting]
    end
    
    subgraph "Application Layer"
        G[Controllers]
        H[Services]
        I[Providers]
    end
    
    subgraph "Data Layer"
        J[(PostgreSQL)]
        K[Connection Pool]
        L[Redis Cache]
    end
    
    subgraph "External Services"
        M[Email Service]
        N[File Storage]
        O[Analytics]
    end
    
    A --> D
    B --> D
    C --> D
    
    D --> E
    E --> F
    F --> G
    
    G --> H
    H --> I
    I --> K
    K --> J
    
    H --> L
    H --> M
    H --> N
    H --> O
```

## API Request Flow

```mermaid
sequenceDiagram
    participant C as Client
    participant API as API Controller
    participant S as Service
    participant P as Provider
    participant DB as Database
    
    C->>API: HTTP Request
    API->>API: Validate Input
    API->>API: Extract Token
    API->>S: Call Service Method
    S->>S: Business Logic
    S->>P: Call Provider
    P->>DB: Execute SQL
    DB-->>P: Return Data
    P-->>S: Return Domain Model
    S-->>API: Return Result
    API->>API: Map to Response
    API-->>C: HTTP Response
```

## Key Design Patterns

### 1. Multi-Tenancy Pattern
```mermaid
graph LR
    A[Company A] --> B[Users A]
    A --> C[Teams A]
    A --> D[Posts A]
    
    E[Company B] --> F[Users B]
    E --> G[Teams B]
    E --> H[Posts B]
    
    B -.-> F
    D -.-> H
```

### 2. Service Layer Pattern
```mermaid
graph TD
    A[Controller] --> B[Service Interface]
    B --> C[Service Implementation]
    C --> D[Provider Interface]
    D --> E[Provider Implementation]
    E --> F[Database]
    
    B --> G[Business Logic]
    C --> H[Validation]
    C --> I[Authentication]
    C --> J[Error Handling]
```

### 3. Repository Pattern
```mermaid
graph LR
    A[Service] --> B[IUserProvider]
    A --> C[IPostProvider]
    A --> D[ITeamProvider]
    
    B --> E[PostgreSQLUserProvider]
    C --> F[PostgreSQLPostProvider]
    D --> G[PostgreSQLTeamProvider]
    
    E --> H[(Database)]
    F --> H
    G --> H
```

## Performance Optimization Strategies

### 1. Index Strategy
```mermaid
graph TD
    A[Query Performance] --> B[Primary Indexes]
    A --> C[JSONB GIN Indexes]
    A --> D[Composite Indexes]
    
    B --> E[Primary Keys]
    B --> F[Unique Constraints]
    
    C --> G[User Mentions]
    C --> H[Hashtags]
    C --> I[Employee IDs]
    
    D --> J[Multi-column Queries]
    D --> K[Filter Combinations]
```

### 2. Connection Management
```mermaid
graph LR
    A[NpgsqlDataSource] --> B[Connection Pool]
    B --> C[Connection 1]
    B --> D[Connection 2]
    B --> E[Connection N]
    
    C --> F[Database]
    D --> F
    E --> F
    
    G[SemaphoreSlim] --> H[Concurrency Control]
    H --> B
```

This comprehensive design provides a complete low-level view of the Cherish backend system, including database schema, relationships, API structure, and architectural patterns.
