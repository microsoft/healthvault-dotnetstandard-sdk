<#
  <copyright file="ApplyAssemblyVersion.ps1" company="Microsoft Corporation">
   Copyright (C) Microsoft Corporation.  All rights reserved.
  </copyright>
  <summary>
    This script copies the OneSDK dlls to an isolated destination so they can be processed for signing without including unnecessary dlls.
  </summary>
 #>

param
(
   [Parameter(Mandatory=$true)]
   [string]$Root,

   [Parameter(Mandatory=$true)]
   [string]$Destination,

   [Parameter(Mandatory=$true)]
   [string]$Configuration
)

if (!(Test-Path $Destination)) {
    mkdir $Destination
}

$destPath = (Get-Item $Destination).FullName
$rootPath = (Get-Item $Root).FullName

"Destination: $destPath"
"Root: $rootPath"

# Get projects that will be included with the nuget package
$projects = Get-ChildItem -Name -Directory -Path $rootPath -Include Microsoft.HealthVault* -Exclude *Test*

# Get binaries from each project directory that match their project name, plus the Client dlls, which don't match but aren't duplicated in the different directories.
$dlls = $projects | ForEach-Object{Get-ChildItem -Recurse -Path $rootPath\${_}\bin\$Configuration\ -Include @("${_}.dll","Microsoft.HealthVault.Client.dll")} | ForEach-Object{@{"RelativeName" = $_.FullName.Substring($rootPath.Length); "RelativePath" = $_.Directory.FullName.Substring($rootPath.Length)}}

# Recreate the relative directory structure in the destination if it doesn't exist yet, copy files to their proper places.
foreach ($dll in $dlls) {
    if (!(Test-Path "$destPath$($dll.RelativePath)")) {
        mkdir "$destPath$($dll.RelativePath)"
    }

    Copy-Item "$rootPath$($dll.RelativeName)" "$destPath$($dll.RelativeName)"
}
