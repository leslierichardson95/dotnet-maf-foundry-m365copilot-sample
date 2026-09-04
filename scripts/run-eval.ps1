param(
    [string] $Environment,
    [switch] $Foundry
)

. "$PSScriptRoot\Import-AzdEnvironment.ps1" -Environment $Environment

$arguments = @()
if ($Foundry) {
    $arguments += '--foundry'
}

dotnet run --project "$PSScriptRoot\..\src\IThelper.Eval" -- @arguments
exit $LASTEXITCODE
