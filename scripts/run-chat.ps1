param(
    [string] $Environment
)

. "$PSScriptRoot\Import-AzdEnvironment.ps1" -Environment $Environment

dotnet run --project "$PSScriptRoot\..\src\IThelper.Agent" -- --chat
exit $LASTEXITCODE
