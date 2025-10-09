# Cherish Seed Data Generator
# This script populates the database with realistic test data using the APIs

param(
    [string]$ApiUrl = "https://localhost:7000"
)

Write-Host "🌱 Cherish Seed Data Generator" -ForegroundColor Green
Write-Host "==============================" -ForegroundColor Green
Write-Host ""

# Check if the API is running
Write-Host "🔍 Checking if API is running at $ApiUrl..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$ApiUrl/swagger" -Method Get -TimeoutSec 5 -UseBasicParsing
    Write-Host "✅ API is running and accessible" -ForegroundColor Green
} catch {
    Write-Host "❌ API is not accessible at $ApiUrl" -ForegroundColor Red
    Write-Host "💡 Please make sure:" -ForegroundColor Yellow
    Write-Host "   1. The Cherish API is running (dotnet run --project RestApi)" -ForegroundColor Yellow
    Write-Host "   2. The database is initialized (dotnet run --project Database init)" -ForegroundColor Yellow
    Write-Host "   3. The API URL is correct" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "🚀 Starting seed data generation..." -ForegroundColor Green

# Build and run the seed data script
try {
    dotnet build Scripts/Scripts.csproj
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Build failed" -ForegroundColor Red
        exit 1
    }
    
    dotnet run --project Scripts/Scripts.csproj $ApiUrl
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "🎉 Seed data generation completed successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📊 You can now test your APIs with realistic data:" -ForegroundColor Cyan
        Write-Host "   - Login with any user (e.g., alice.johnson, bob.smith)" -ForegroundColor Cyan
        Write-Host "   - Password: Password123!" -ForegroundColor Cyan
        Write-Host "   - Test post filtering by team, user, and hashtags" -ForegroundColor Cyan
        Write-Host "   - Try reactions and comments" -ForegroundColor Cyan
    } else {
        Write-Host "❌ Seed data generation failed" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ Error running seed data script: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
