// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharedVersionInfo.cs" company="Microsoft Corporation">
//   Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyCopyright("Copyright (c) Microsoft Corporation. All rights reserved.")]
[assembly: AssemblyProduct("Microsoft HealthVault (R)")]
[assembly: CLSCompliant(false)]
[assembly: ComVisible(false)]

// NOTE: Always leave these values at 1.0.0.0 and 1.0.1.1 as the build process does a string replace with the actual build value.
// Assembly version is set from build process in form major.minor.0.0.
// Assembly file version will represent the assembly version with the same major and minor values.
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.1.1")]