#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Execute all SQL scripts under Database/Scripts against the configured PostgreSQL database.

.DESCRIPTION
    Reads the PostgreSQL connection string from RestApi/appsettings.json, ensures all
    SQL files in Database/Scripts are executed (sorted by name), and stops on failure.

.PARAMETER Project
    Optional path to the backendv4 project root (default: current directory).

.EXAMPLE
    ./run-db-scripts.ps1

.EXAMPLE
    ./run-db-scripts.ps1 -Project "D:/cherish/backendv4"
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$Project = "."
)

function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
}

function Write-ErrorMessage {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

try {
    Push-Location $Project

    if (-not (Test-Path "RestApi/appsettings.json")) {
        Write-ErrorMessage "RestApi/appsettings.json not found. Run this from the backendv4 root (or specify -Project)."
        exit 1
    }

    $configJson = Get-Content "RestApi/appsettings.json" -Raw | ConvertFrom-Json
    $connString = $configJson.ConnectionStrings.PostgreSQL
    if ([string]::IsNullOrWhiteSpace($connString)) {
        Write-ErrorMessage "Connection string 'PostgreSQL' missing in RestApi/appsettings.json."
        exit 1
    }

    $parts = $connString -split ';' | Where-Object { $_ -and $_.Contains('=') }
    $dict = @{}
    foreach ($part in $parts) {
        $kv = $part -split '=', 2
        if ($kv.Length -eq 2) {
            $key = $kv[0].Trim().ToLower()
            $value = $kv[1].Trim()
            $dict[$key] = $value
        }
    }

    $dbHost = $dict["host"]
    $database = $dict["database"]

    if ($dict.ContainsKey("port")) {
        $dbPort = $dict["port"]
    } else {
        $dbPort = "5432"
    }

    if ($dict.ContainsKey("username")) {
        $dbUser = $dict["username"]
    } elseif ($dict.ContainsKey("user id")) {
        $dbUser = $dict["user id"]
    } else {
        $dbUser = $null
    }

    if ($dict.ContainsKey("password")) {
        $dbPassword = $dict["password"]
    } else {
        $dbPassword = $null
    }

    if (-not $dbHost -or -not $database) {
        Write-ErrorMessage "Host or Database missing from connection string."
        exit 1
    }

    if (-not $dbUser) {
        Write-ErrorMessage "Username missing from connection string."
        exit 1
    }

    Write-Info ("Using PostgreSQL server {0}:{1}, database '{2}'" -f $dbHost, $dbPort, $database)

    if ($dbPassword) {
        $env:PGPASSWORD = $dbPassword
    }

    $scriptsPath = Join-Path (Get-Location) "Database/Scripts"
    if (-not (Test-Path $scriptsPath)) {
        Write-ErrorMessage "Scripts directory '$scriptsPath' not found."
        exit 1
    }

    $scriptFiles = Get-ChildItem $scriptsPath -Filter "*.sql" | Sort-Object Name
    if ($scriptFiles.Count -eq 0) {
        Write-Warning "No SQL scripts found in $scriptsPath"
        exit 0
    }

    Write-Info ("Executing {0} script(s)..." -f $scriptFiles.Count)

    foreach ($file in $scriptFiles) {
        Write-Info ("Running {0}" -f $file.Name)
        & psql -h $dbHost -p $dbPort -U $dbUser -d $database -f $file.FullName
        if ($LASTEXITCODE -ne 0) {
            Write-ErrorMessage ("Script {0} failed with exit code {1}" -f $file.Name, $LASTEXITCODE)
            exit $LASTEXITCODE
        }
    }

    Write-Success "All scripts executed successfully."
}
catch {
    $errorMessage = $_.Exception.Message
    Write-ErrorMessage ("Unexpected error: {0}" -f $errorMessage)
}
finally {
    Pop-Location
    Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue | Out-Null
}
