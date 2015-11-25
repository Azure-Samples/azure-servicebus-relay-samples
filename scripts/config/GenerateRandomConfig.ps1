[CmdletBinding()]
param(
    [parameter(Mandatory=$true)]
    [string]$ConfigFile,
    [String]$ResourcePrefix
)

###########################################################
# Start - Initialization - Invocation, Logging etc
###########################################################
$VerbosePreference = "SilentlyContinue"
$ErrorActionPreference = "Stop"

$scriptPath = $MyInvocation.MyCommand.Path
$scriptDir = Split-Path $scriptPath

& "$scriptDir\..\init.ps1"
if(-not $?)
{
    throw "Initialization failure."
    exit -9999
}
###########################################################
# End - Initialization - Invocation, Logging etc
###########################################################


$scriptPath = $MyInvocation.MyCommand.Path
$scriptDir = Split-Path $scriptPath

if([String]::IsNullOrWhiteSpace($ResourcePrefix))
{
    $ResourcePrefix = "sbsamples"
}

$resourceName = $ResourcePrefix + [System.DateTime]::Now.ToString("yyyyMMddHHmm")

if($resourceName.Length -gt 24)
{
    $resourceName = $resourceName.Substring(0, 24)
}

$config = @{
    RANDOM_RESOURCE_NAME = $resourceName
}

$runConfigDir = Split-Path $ConfigFile
mkdir $runConfigDir -ErrorAction SilentlyContinue

Write-InfoLog "Generating run configurations at $configFile" (Get-ScriptName) (Get-ScriptLineNumber)

if(-not (Test-Path $ConfigFile))
{
    Write-InfoLog "Creating a new run configuration at $configFile" (Get-ScriptName) (Get-ScriptLineNumber)
    &$scriptDir\ReplaceStringInFile.ps1 "$scriptDir\configurations.properties.template" $ConfigFile $config
}
else
{
    Write-InfoLog "An existing run configuration was found at $configFile, just updating newer entries." (Get-ScriptName) (Get-ScriptLineNumber)
    &$scriptDir\ReplaceStringInFile.ps1 $ConfigFile $ConfigFile $config
}