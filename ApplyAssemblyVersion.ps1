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

function ReplaceVersionXml($content2, $name, $version) {
    $startTag = '<' + $name + '>'
    $endTag = '</' + $name + '>'
    return $content2 -replace ($startTag + '[\d\.]+' + $endTag), ($startTag + $version + $endTag)
}

function ReplaceVersionCs($content, $name, $version) {
    return $content -replace ($name + '\("[\d\.]+"\)'), ($name + '("' + $version + '")')
}

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

$sharedVersionFile = Join-Path -Path $SourcesPath -ChildPath 'SharedVersionInfo.cs'
Set-ItemProperty $sharedVersionFile -Name IsReadOnly -Value $false

$content = Get-Content $sharedVersionFile
$content = ReplaceVersionCs $content 'AssemblyVersion' $AssemblyVersion
$content = ReplaceVersionCs $content 'AssemblyFileVersion' $AssemblyFileVersion
Set-Content -Encoding Default -Path $sharedVersionFile -Value $content

$commonTargetsFile = Join-Path -Path $SourcesPath -ChildPath 'NetStandard.Common.targets'
Set-ItemProperty $commonTargetsFile -Name IsReadOnly -Value $false

$content = Get-Content $commonTargetsFile
$content = ReplaceVersionXml $content 'AssemblyVersion' $AssemblyVersion
$content = ReplaceVersionXml $content 'FileVersion' $AssemblyFileVersion
Set-Content -Encoding Default -Path $commonTargetsFile -Value $content