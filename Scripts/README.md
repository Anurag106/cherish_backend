# Cherish Seed Data Generator

This script populates your Cherish database with realistic test data using the APIs. It creates a complete ecosystem of companies, users, teams, hashtags, posts, reactions, and comments.

## 🎯 What Gets Created

### Companies (2)
- **TechCorp Solutions** (CompanyA)
- **InnovateLabs Inc** (CompanyB)

### Users (20 total, 10 per company)
Each company gets 10 users with realistic profiles:
- **3 Managers** per company
- **7 Employees** per company
- Real names, emails, and usernames

### Teams (6 total, 3 per company)
Each company gets 3 teams:
1. **Development Team** (4 members: 1 manager + 3 employees)
2. **Marketing Team** (3 members: 1 manager + 2 employees)  
3. **Leadership Team** (3 managers)

### Hashtags (20 total, 10 per company)
Realistic hashtags like:
- #innovation, #teamwork, #productivity
- #tech, #leadership, #project
- #meeting, #achievement, #feedback, #celebration

### Posts (60 total, 30 per company)
Realistic posts with:
- **Smart mentions** (@username)
- **Relevant hashtags** (#hashtagname)
- **Point allocations** (+10, +5, etc.)
- **Different visibility levels** (public, team, private)
- **Realistic content** about work, meetings, achievements

### Reactions (100+ total)
- **5 different emoji types** (Like, Love, Laugh, Angry, Sad)
- **70% of posts get reactions** (realistic engagement)
- **1-5 users react per post** (varied engagement levels)
- **Random reaction types** for realistic social interaction

### Comments (50+ total)
- **Realistic comment content** with mentions and points
- **60% of posts get comments** (natural engagement)
- **1-3 users comment per post** (varied discussion)
- **Smart mentions** in comments (@username +5 #feedback)
- **Point allocations** in comments (+5, +10)
- **Hashtag usage** in comments (#achievement, #feedback, etc.)

## 🚀 How to Run

### Prerequisites
1. **Cherish API running**: `dotnet run --project RestApi`
2. **Database initialized**: `dotnet run --project Database init`

### Option 1: PowerShell Script (Recommended)
```powershell
# Run with default API URL (https://localhost:7000)
.\Scripts\run-seed-data.ps1

# Run with custom API URL
.\Scripts\run-seed-data.ps1 -ApiUrl "https://localhost:5000"
```

### Option 2: Direct .NET Command
```bash
# Build the script
dotnet build Scripts/Scripts.csproj

# Run the script
dotnet run --project Scripts/Scripts.csproj

# Run with custom API URL
dotnet run --project Scripts/Scripts.csproj https://localhost:5000
```

## 🔑 Test Credentials

After running the script, you can login with any of these users:

### TechCorp Solutions Users
- **alice.johnson** (Manager) - Password: `Password123!`
- **bob.smith** (Employee) - Password: `Password123!`
- **carol.davis** (Employee) - Password: `Password123!`
- **david.wilson** (Employee) - Password: `Password123!`
- **emma.brown** (Manager) - Password: `Password123!`
- **frank.garcia** (Employee) - Password: `Password123!`
- **grace.martinez** (Employee) - Password: `Password123!`
- **henry.anderson** (Manager) - Password: `Password123!`
- **ivy.taylor** (Employee) - Password: `Password123!`
- **jack.thomas** (Employee) - Password: `Password123!`

### InnovateLabs Inc Users
- **kate.hernandez** (Manager) - Password: `Password123!`
- **liam.moore** (Employee) - Password: `Password123!`
- **maya.jackson** (Employee) - Password: `Password123!`
- **noah.martin** (Employee) - Password: `Password123!`
- **olivia.lee** (Manager) - Password: `Password123!`
- **parker.perez** (Employee) - Password: `Password123!`
- **quinn.thompson** (Employee) - Password: `Password123!`
- **riley.white** (Manager) - Password: `Password123!`
- **sophia.harris** (Employee) - Password: `Password123!`
- **tyler.sanchez** (Employee) - Password: `Password123!`

## 🧪 Testing Your APIs

### 1. Test Authentication
```bash
POST /api/auth/login
{
  "username": "alice.johnson",
  "password": "Password123!"
}
```

### 2. Test Post Filtering
```bash
POST /api/post/filter
{
  "pageSize": 15,
  "filterByTeam": true,
  "sortOrder": 1
}
```

### 3. Test User Filtering
```bash
POST /api/post/filter
{
  "pageSize": 15,
  "filterByUserId": "user-guid-here",
  "sortOrder": 1
}
```

### 4. Test Hashtag Filtering
```bash
POST /api/post/filter
{
  "pageSize": 15,
  "hashtagNames": ["innovation", "teamwork"],
  "sortOrder": 1
}
```

### 5. Test Reactions
```bash
POST /api/reaction/react
{
  "postId": "post-guid-here",
  "emojiType": 1
}
```

### 6. Test Comments
```bash
POST /api/comment
{
  "content": "Great post! @alice.johnson +5 #feedback",
  "postId": "post-guid-here",
  "postedByAdded": true
}
```

### 7. Test Reaction Counts
```bash
GET /api/reaction/post/{postId}
```

### 8. Test User Reactions
```bash
GET /api/reaction/user/{postId}
```

### 9. Test Post with Details (includes reactions and comments)
```bash
GET /api/post/{postId}/details
```

## 📊 Data Relationships

The script creates realistic relationships:
- **Users belong to teams** based on their roles
- **Posts mention users** from the same company
- **Posts use relevant hashtags** based on content
- **Point allocations** are realistic (+5, +10, +15)
- **Visibility settings** vary (public, team, private)

## 🔧 Troubleshooting

### API Not Accessible
```
❌ API is not accessible at https://localhost:7000
```
**Solution**: Make sure the API is running:
```bash
dotnet run --project RestApi
```

### Database Not Initialized
```
❌ Database schema initialization failed
```
**Solution**: Initialize the database first:
```bash
dotnet run --project Database init
```

### SSL Certificate Issues
If you get SSL certificate errors, you can:
1. Use HTTP instead: `http://localhost:5000`
2. Or trust the development certificate

## 🎉 What You Get

After running this script, you'll have:
- **Complete test ecosystem** with realistic data
- **All API endpoints** working with real data
- **Complex filtering scenarios** to test
- **Real user interactions** (mentions, hashtags, points)
- **Team-based content** for testing team filters
- **Cross-company isolation** for testing security

Perfect for testing your comprehensive filtering system! 🚀
