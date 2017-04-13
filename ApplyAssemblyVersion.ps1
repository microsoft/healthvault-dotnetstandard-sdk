<#
  <copyright file="ApplyAssemblyVersion.ps1" company="Microsoft Corporation">
   Copyright (C) Microsoft Corporation.  All rights reserved.
  </copyright>
  <summary>
    This script prepares the SharedVersionInfo.cs so that it contains the right build info to stamp into the built files.
  </summary>
 #>

param
(
   [Parameter(Mandatory=$true)]
   [string]$BuildNumber,

   [Parameter(Mandatory=$true)]
   [string]$SourcesPath
)

$parts = $BuildNumber.Split('.');

$sprint = 0;

if ($parts[1] -match '^s[0-9]+$')
{
    $sprint = $parts[1].TrimStart('s');
}

$build = "" + ($parts[2].Substring(0,4) % 5) + $parts[2].Substring(4);
$revision = $parts[3];

$AssemblyVersion = "1.$sprint.0.0"
$AssemblyFileVersion = "1.$sprint.$build.$revision"
Write-Host "##vso[task.setvariable variable=AssemblyFileVersion;]$AssemblyFileVersion";
Write-Host "##vso[task.setvariable variable=PreviewFileVersion;]${AssemblyFileVersion}-preview";

$file = Join-Path -Path $SourcesPath -ChildPath 'SharedVersionInfo.cs'
Set-ItemProperty $file -Name IsReadOnly -Value $false

$content = Get-Content $file
$content = $content -replace "1.0.0.0", $AssemblyVersion
$content = $content -replace "1.0.1.1", $AssemblyFileVersion
Set-Content -Encoding Default -Path $file -Value $content