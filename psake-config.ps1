$config.buildFileName="default.ps1"
$config.framework = "4.6.1"
$config.taskNameFormat="Executing {0}"
$config.verboseError= $true
$config.coloredOutput = $true
$config.moduleScope="global"
$config.taskNameFormat= { param($taskName) "Executing $taskName at $(get-date)" }