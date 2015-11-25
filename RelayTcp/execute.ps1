###########################################################
# Start - Initialization - Invocation, Logging etc
###########################################################
$VerbosePreference = "SilentlyContinue"
$ErrorActionPreference = "Stop"

$scriptPath = $MyInvocation.MyCommand.Path
$scriptDir = Split-Path $scriptPath

& "$scriptDir\..\scripts\init.ps1"
if(-not $?)
{
    throw "Initialization failure."
}
###########################################################
# End - Initialization - Invocation, Logging etc
###########################################################

$configFile = Join-Path $scriptDir "run\configurations.properties"
$config = & "$scriptDir\..\scripts\config\ReadConfig.ps1" $configFile
Select-AzureSubscription -SubscriptionName $config["AZURE_SUBSCRIPTION_NAME"]

