// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System.Threading.Tasks;

namespace NuGet.ProjectManagement
{
    /// <summary>
    /// Represents an API providing different capabilities
    /// exposed by a project system
    /// </summary>
    public interface IProjectSystemCapabilities
    {
#pragma warning disable RS0016 // Add public types and members to the declared API
        Task<bool> SupportsPackageReferences();
#pragma warning restore RS0016 // Add public types and members to the declared API

        bool NominatesOnSolutionLoad { get; }
    }
}
