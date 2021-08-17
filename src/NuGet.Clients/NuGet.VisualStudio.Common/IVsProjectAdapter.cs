// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.Frameworks;
using NuGet.ProjectManagement;
using NuGet.RuntimeModel;

namespace NuGet.VisualStudio
{
    /// <summary>
    /// Represents an abstraction over Visual Studio project object. Provides access to project's properties and capabilities.
    /// </summary>
    public interface IVsProjectAdapter
    {
        /// <summary>
        /// AssetTargetFallback project property
        /// </summary>
        Task<string> AssetTargetFallback();

        /// <summary>
        /// MSBuildProjectExtensionsPath project property (e.g. c:\projFoo\obj)
        /// </summary>
        Task<string> GetMSBuildProjectExtensionsPathAsync();

        IProjectBuildProperties BuildProperties { get; }

        string CustomUniqueName { get; }

        string FullName { get; }

        string FullProjectPath { get; }

        Task<bool> IsSupportedAsync();

        /// <summary>
        /// Comma or Semicolon separated list of NU* diagnostic codes e.g. NU1000,NU1001
        /// </summary>
        Task<string> NoWarn();

        /// <summary>
        /// PackageTargetFallback project property
        /// </summary>
        Task<string> PackageTargetFallback();

        /// <summary>
        /// In unavoidable circumstances where we need to DTE object, it's exposed here
        /// </summary>
        EnvDTE.Project Project { get; }

        string ProjectId { get; }

        /// <summary>
        /// Full path to a parent directory containing project file.
        /// </summary>
        Task<string> GetProjectDirectoryAsync();

        string ProjectName { get; }

        ProjectNames ProjectNames { get; }

        /// <summary>
        /// Additional fallback folders DTE property
        /// </summary>
        Task<string> RestoreAdditionalProjectFallbackFolders();

        /// <summary>
        /// Additional Sources DTE property
        /// </summary>
        Task<string> RestoreAdditionalProjectSources();

        /// <summary>
        /// RestoreFallbackFolders DTE property
        /// </summary>
        Task<string> RestoreFallbackFolders();

        /// <summary>
        /// Restore Packages Path DTE property
        /// </summary>
        Task<string> RestorePackagesPath();

        /// <summary>
        /// Restore Sources DTE property
        /// </summary>
        Task<string> RestoreSources();

        /// <summary>
        /// TreatWarningsAsErrors true/false
        /// </summary>
        Task<string> TreatWarningsAsErrors();

        string UniqueName { get; }

        /// <summary>
        /// Version
        /// </summary>
        Task<string> Version();

        IVsHierarchy VsHierarchy { get; }

        /// <summary>
        /// Comma or Semicolon separated list of NU* diagnostic codes e.g. NU1000,NU1001
        /// </summary>
        Task<string> WarningsAsErrors();

        Task<string[]> GetProjectTypeGuidsAsync();

        Task<FrameworkName> GetDotNetFrameworkNameAsync();

        Task<IEnumerable<string>> GetReferencedProjectsAsync();

        /// <summary>
        /// Project's runtime identifiers. Should never be null but can be an empty sequence.
        /// </summary>
        Task<IEnumerable<RuntimeDescription>> GetRuntimeIdentifiersAsync();

        /// <summary>
        /// Project's supports (a.k.a guardrails). Should never be null but can be an empty sequence.
        /// </summary>
        Task<IEnumerable<CompatibilityProfile>> GetRuntimeSupportsAsync();

        /// <summary>
        /// Project's target framework
        /// </summary>
        Task<NuGetFramework> GetTargetFrameworkAsync();

        /// <summary>
        /// RestorePackagesWithLockFile project property.
        /// </summary>
        /// <returns></returns>
        Task<string> GetRestorePackagesWithLockFileAsync();

        /// <summary>
        /// NuGetLockFilePath project property.
        /// </summary>
        /// <returns></returns>
        Task<string> GetNuGetLockFilePathAsync();

        /// <summary>
        /// RestoreLockedMode project property.
        /// </summary>
        /// <returns></returns>
        Task<bool> IsRestoreLockedAsync();

        /// <summary>
        /// Reads a project property and return its value.
        /// </summary>
        Task<string> GetPropertyValueAsync(string propertyName);

        /// <summary>
        /// Reads a project build items and the requested metadata.
        /// </summary>
        /// <param name="itemName">The item name.</param>
        /// <param name="metadataNames">The metadata names to read.</param>
        /// <returns>An <see cref="IEnumerable{(string ItemId, string[] ItemMetadata)}"/> containing the itemId and the metadata values.</returns>
        Task<IEnumerable<(string ItemId, string[] ItemMetadata)>> GetBuildItemInformationAsync(string itemName, params string[] metadataNames);

        /// <summary>
        /// See <see cref="Microsoft.VisualStudio.Shell.PackageUtilities.IsCapabilityMatch(IVsHierarchy, string)"/>
        /// </summary>
        Task<bool> IsCapabilityMatchAsync(string capabilityExpression);
    }
}
