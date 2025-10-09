# Cherish Database Schema Management

This folder contains all database schema management for the Cherish backend system.

## 📁 Structure

```
Database/
├── DatabaseSchemaManager.cs    # Main schema management class
├── DatabaseManager.cs          # Executable database manager
├── Database.csproj            # Project file
├── README.md                  # This file
└── Scripts/                   # Individual SQL scripts
    ├── 01_companies.sql       # Companies table
    ├── 02_users.sql          # Users table
    ├── 03_teams.sql          # Teams table
    ├── 04_hashtags.sql       # Hashtags table
    ├── 05_transactions.sql   # Transactions table
    └── 06_posts.sql          # Posts table
```

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK installed
- PostgreSQL database running
- Connection string configured in `RestApi/appsettings.json`

### Step-by-Step Setup

1. **Navigate to project root:**
   ```bash
   cd D:\cherish\backendv2
   ```

2. **Initialize database schema:**
   ```bash
   dotnet run --project Database -- init
   ```

3. **Verify schema status:**
   ```bash
   dotnet run --project Database -- status
   ```

4. **Start the API:**
   ```bash
   dotnet run --project RestApi --urls "http://localhost:5000"
   ```

### Alternative: Using PowerShell Script
```powershell
# Initialize database schema
.\setup-database.ps1 -Action init

# Check schema status
.\setup-database.ps1 -Action status
```

## 🛠️ Available Commands

### Initialize Database Schema
```bash
dotnet run --project Database -- init
```
**What it does:**
- Creates all database tables in proper dependency order
- Handles existing tables and migrations
- Sets up indexes and constraints
- Outputs progress with emojis and status messages

**Expected Output:**
```
🔧 Cherish Database Schema Manager
==================================
🚀 Initializing Cherish Database Schema...
==========================================
📋 Creating companies table...
✅ Companies table created/verified
👥 Creating users table...
✅ Users table created/verified
👥 Creating teams table...
✅ Teams table created/verified
🏷️ Creating hashtags table...
✅ Hashtags table created/verified
💰 Creating transactions table...
✅ Transactions table created/verified
📝 Creating posts table...
✅ Posts table created/verified
✅ Database schema initialization completed successfully!
```

### Check Schema Status
```bash
dotnet run --project Database -- status
```
**What it does:**
- Lists all database tables
- Shows row count for each table
- Indicates if tables exist or are missing

**Expected Output:**
```
📊 Database Schema Status:
=========================
✅ companies - 2 rows
✅ users - 2 rows
✅ teams - 0 rows
✅ hashtags - 1 rows
✅ transactions - 2 rows
✅ posts - 0 rows
```

## 📋 Features

- **Centralized Schema Management**: All table creation scripts in one place
- **Dependency Order**: Tables are created in proper dependency order
- **Migration Support**: Handles schema changes and migrations
- **Status Checking**: View current database schema status
- **Individual Scripts**: Each table has its own SQL script for easy maintenance

## 🔧 Database Tables

1. **Companies** - Company information
2. **Users** - User accounts and profiles
3. **Teams** - Team management with employee lists
4. **Hashtags** - Hashtag definitions (company-scoped)
5. **Transactions** - Point transactions between users
6. **Posts** - User posts with mentions, hashtags, and points

## ⚙️ Configuration

The database manager reads configuration from:
- `RestApi/appsettings.json`
- `RestApi/appsettings.Development.json`
- Environment variables

Make sure your PostgreSQL connection string is properly configured in these files.

## 🔄 Schema Changes

When making schema changes:

1. Update the relevant SQL script in `Scripts/`
2. Update the `DatabaseSchemaManager.cs` if needed
3. Test the changes locally
4. Run `dotnet run --project Database -- init` to apply changes

## 🚨 Troubleshooting

### Common Issues

**1. Build Errors:**
```bash
# If you get file lock errors, kill any running processes
taskkill /F /IM dotnet.exe
# Then rebuild
dotnet build Cherish.sln
```

**2. Connection Issues:**
- Verify PostgreSQL is running
- Check connection string in `RestApi/appsettings.json`
- Ensure database exists and is accessible

**3. Permission Issues:**
- Make sure the database user has CREATE TABLE permissions
- Check if the database exists: `cheris` (not `defaultdb`)

**4. PowerShell Script Issues:**
```powershell
# If PowerShell script fails, use direct .NET CLI instead
dotnet run --project Database -- init
```

### Debug Mode
```bash
# Run with verbose output
dotnet run --project Database -- init --verbosity detailed
```

## 📝 Database Schema Notes

- **Primary Keys**: All tables use UUID except hashtags (SERIAL)
- **JSONB Columns**: Used for arrays (employee_ids, user_mentioned, hashtags, metadata)
- **Foreign Keys**: Ensure data integrity across tables
- **Indexes**: Created for optimal query performance
- **Comments**: Added to all tables and columns for documentation

## 🏗️ Table Dependencies

Tables are created in this order to respect foreign key constraints:
1. `companies` (no dependencies)
2. `users` (depends on companies)
3. `teams` (depends on users, companies)
4. `hashtags` (depends on companies, users)
5. `transactions` (depends on companies, users)
6. `posts` (depends on users, companies)

## 🔧 Development Workflow

1. **First Time Setup:**
   ```bash
   dotnet run --project Database -- init
   dotnet run --project Database -- status
   ```

2. **Making Schema Changes:**
   - Edit SQL scripts in `Scripts/` folder
   - Update `DatabaseSchemaManager.cs` if needed
   - Test locally with `dotnet run --project Database -- init`
   - Commit changes to version control

3. **Production Deployment:**
   - Run the same `init` command on production
   - The system handles existing tables gracefully
   - No data loss during schema updates
