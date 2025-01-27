# This script installs a dotnet template from a subfolder relative to the script location.

# Resolve the path of the current script file (Works in PowerShell 3.0+)
# Note: $PSCommandPath is only available in PowerShell 3.0 and later.
#       If you're in older PowerShell versions, you can use $MyInvocation.MyCommand.Definition
$scriptDirectory = Split-Path -Path $PSCommandPath -Parent

# Join the script directory with the template folder name
$templatePath = Join-Path -Path $scriptDirectory -ChildPath "DayTemplate"

Write-Host "Installing .NET template from: $templatePath"

# Run the dotnet new --install command
dotnet new install $templatePath --force

Write-Host "Template installation complete."