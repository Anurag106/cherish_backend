#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Cherish Database Schema Setup Script

.DESCRIPTION
    PowerShell script to set up database schema for the Cherish backend system.
    This script provides an easy way to initialize database tables.

.PARAMETER Action
    The action to perform: init, status

.PARAMETER Project
    The project path (default: current directory)

.EXAMPLE
    .\setup-database.ps1 -Action init
    Initialize the database schema with all tables

.EXAMPLE
    .\setup-database.ps1 -Action status
    Show current database schema status
#>

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("init", "status")]
    [string]$Action,
    
    [Parameter(Mandatory=$false)]
    [string]$Project = "."
)

# Colors for output
$SuccessColor = "Green"
$ErrorColor = "Red"
$InfoColor = "Cyan"
$WarningColor = "Yellow"

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor $SuccessColor
}

function Write-Error {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor $ErrorColor
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ️  $Message" -ForegroundColor $InfoColor
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠️  $Message" -ForegroundColor $WarningColor
}

# Main execution
try {
    Write-Host "🚀 Cherish Database Schema Setup" -ForegroundColor $InfoColor
    Write-Host "=================================" -ForegroundColor $InfoColor
    
    # Change to project directory
    if ($Project -ne ".") {
        Write-Info "Changing to project directory: $Project"
        Set-Location $Project
    }
    
    # Check if we're in the right directory
    if (-not (Test-Path "Cherish.sln")) {
        Write-Error "Cherish.sln not found. Please run this script from the project root directory."
        exit 1
    }
    
    # Check if appsettings.json exists
    if (-not (Test-Path "RestApi\appsettings.json")) {
        Write-Error "appsettings.json not found in RestApi directory. Please ensure configuration is set up."
        exit 1
    }
    
    Write-Info "Building the project..."
    $buildResult = dotnet build Cherish.sln --configuration Release --verbosity quiet
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed. Please fix compilation errors before running database setup."
        exit 1
    }
    
    Write-Success "Build completed successfully"
    
    # Run the appropriate database command
    switch ($Action) {
        "init" {
            Write-Info "Initializing database schema..."
            dotnet run --project Database -- init
        }
        "status" {
            Write-Info "Checking database schema status..."
            dotnet run --project Database -- status
        }
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Database operation completed successfully"
    } else {
        Write-Error "Database operation failed"
        exit $LASTEXITCODE
    }
    
} catch {
    Write-Error "An unexpected error occurred: $($_.Exception.Message)"
    exit 1
} finally {
    # Return to original directory
    if ($Project -ne ".") {
        Set-Location $OLDPWD
    }
}

Write-Host ""
Write-Info "For more information, run: .\setup-database.ps1 -Action status"
