param(
    [string] $Environment
)

$arguments = @('env', 'get-values')
if ($Environment) {
    $arguments += @('--environment', $Environment)
}

$values = & azd @arguments
if ($LASTEXITCODE -ne 0) {
    throw 'Unable to read the selected azd environment.'
}

foreach ($line in $values) {
    if ($line -match '^([A-Za-z_][A-Za-z0-9_]*)="(.*)"$') {
        $name = $Matches[1]
        $value = $Matches[2].Replace('\"', '"')
        Set-Item -Path "Env:$name" -Value $value
    }
}
