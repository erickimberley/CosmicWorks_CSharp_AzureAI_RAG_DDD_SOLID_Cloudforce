[CmdletBinding()]
param(
  [string]$AssemblyFilter = 'CosmicWorks.*',
  [string]$ReportDir = 'CoverageReport',
  [switch]$Open
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

function Remove-PathSafe([string]$p) {
  if (Test-Path $p) { Remove-Item $p -Recurse -Force -ErrorAction SilentlyContinue }
}

Write-Host "==> Cleaning previous coverage artifacts..." -ForegroundColor Cyan
# remove any prior coverage files
Get-ChildItem -Path $repoRoot -Recurse -Include coverage.cobertura.xml,coverage.json,coverage.info `
  -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue
# remove all TestResults dirs
Get-ChildItem -Path $repoRoot -Recurse -Directory -Filter TestResults `
  -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
# remove last HTML report
Remove-PathSafe $ReportDir

Write-Host "==> Restoring solution..." -ForegroundColor Cyan
dotnet restore | Out-Host

# build a temporary runsettings (self-contained; no repo file required)
$runsettings = Join-Path $env:TEMP "coverlet.$([guid]::NewGuid()).runsettings"
$includePattern  = "[$AssemblyFilter]*"
$excludeAssemblies = "[CosmicWorks.Tests]*"
# keep both back/forward-slash variants for Windows/PDB paths
$excludeByFile = "**\CosmicWorks.Domain\Services\IDiscountPolicy.cs;**/CosmicWorks.Domain/Services/IDiscountPolicy.cs"
$excludeType   = "[CosmicWorks.Domain]CosmicWorks.Domain.Services.IDiscountPolicy"

@"
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <Format>cobertura</Format>
          <Include>$includePattern</Include>
          <Exclude>$excludeAssemblies;$excludeType</Exclude>
          <ExcludeByFile>$excludeByFile</ExcludeByFile>
          <ExcludeByAttribute>System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute</ExcludeByAttribute>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
"@ | Out-File -FilePath $runsettings -Encoding utf8

Write-Host "==> Running tests with coverage..." -ForegroundColor Cyan
dotnet test --settings $runsettings | Out-Host

# clean again just before report generation (extra safety if other runs created files)
Write-Host "==> Pre-report cleanup..." -ForegroundColor Cyan
Remove-PathSafe $ReportDir
Get-ChildItem -Path $repoRoot -Recurse -Include coverage.cobertura.xml `
  -ErrorAction SilentlyContinue | ForEach-Object { $_.FullName } | Out-Null

# ensure reportgenerator is available
if (-not (Get-Command reportgenerator -ErrorAction SilentlyContinue)) {
  Write-Host "==> Installing reportgenerator tool..." -ForegroundColor Cyan
  dotnet tool install --global dotnet-reportgenerator-globaltool | Out-Host
  $env:PATH += ";" + (Join-Path $env:USERPROFILE ".dotnet\tools")
}

Write-Host "==> Generating HTML report..." -ForegroundColor Cyan
reportgenerator `
  -reports:"**/coverage.cobertura.xml" `
  -targetdir:$ReportDir `
  -reporttypes:"HtmlInline_AzurePipelines;TextSummary" `
  -assemblyfilters:"+$AssemblyFilter" `
  -classfilters:"-CosmicWorks.Domain.Services.IDiscountPolicy" | Out-Host

Write-Host "`n==> Coverage Summary" -ForegroundColor Green
Get-Content (Join-Path $ReportDir "Summary.txt") | Out-Host

if ($Open) { Start-Process (Join-Path $ReportDir "index.html") }

Write-Host "==> Done." -ForegroundColor Green