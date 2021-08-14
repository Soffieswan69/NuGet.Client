// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.ProjectSystem.References;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.ProjectManagement;
using NuGet.ProjectModel;
using NuGet.VisualStudio;

namespace NuGet.PackageManagement.VisualStudio
{
    /// <summary>
    /// Reference reader implementation for the core project system in the integrated development environment (IDE).
    /// </summary>
    internal class CpsProjectSystemReferenceReader
        : IProjectSystemReferencesReader
    {
        private readonly IVsProjectAdapter _vsProjectAdapter;
        private readonly IVsProjectThreadingService _threadingService;

        public CpsProjectSystemReferenceReader(
            IVsProjectAdapter vsProjectAdapter,
            INuGetProjectServices projectServices)
        {
            Assumes.Present(vsProjectAdapter);
            Assumes.Present(projectServices);

            _vsProjectAdapter = vsProjectAdapter;
            _threadingService = projectServices.GetGlobalService<IVsProjectThreadingService>();
            Assumes.Present(_threadingService);
        }

        private async Task<ConfiguredProject> GetConfiguredProjectAsync(EnvDTE.Project project)
        {
            await _threadingService.JoinableTaskFactory.SwitchToMainThreadAsync();

            var context = project as IVsBrowseObjectContext;
            if (context == null)
            {
                // VC implements this on their DTE.Project.Object
                context = project.Object as IVsBrowseObjectContext;
            }
            return context?.ConfiguredProject;
        }

        public async Task<IEnumerable<ProjectRestoreReference>> GetProjectReferencesAsync(
            Common.ILogger logger, CancellationToken _)
        {
            var project = await GetConfiguredProjectAsync(_vsProjectAdapter.Project);
            IBuildDependencyProjectReferencesService service = project.Services.ProjectReferences;

            // TODO NK - There's something *weird* about this
            // The resolved references returns the dll path.
            // The unresolved references returns the project file path, but again stands as unresolved.

            var resolvedProjectReferences = await service.GetResolvedReferencesAsync();
            var unresolvedProjectReferences = await service.GetUnresolvedReferencesAsync();

            var results = new List<ProjectRestoreReference>();
            var hasMissingReferences = unresolvedProjectReferences.Any();

            foreach (IBuildDependencyProjectReference projectReference in resolvedProjectReferences)
            {
                try
                {
                    var childProjectPath = await projectReference.Metadata.GetEvaluatedPropertyValueAsync("MSBuildSourceProjectFile");
                    // TODO NK - Skip shared projects!

                    var projectRestoreReference = new ProjectRestoreReference()
                    {
                        ProjectPath = childProjectPath,
                        ProjectUniqueName = childProjectPath
                    };

                    results.Add(projectRestoreReference);
                }
                catch (Exception ex)
                {
                    hasMissingReferences = true;
                    // Are exceptions expected here
                    logger.LogDebug(ex.ToString());
                }
            }

            if (hasMissingReferences)
            {
                // Log a generic message once per project if any items could not be resolved.
                // In most cases this can be ignored, but in the rare case where the unresolved
                // item is actually a project the restore result will be incomplete.
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Strings.UnresolvedItemDuringProjectClosureWalk,
                    _vsProjectAdapter.UniqueName);

                logger.LogVerbose(message);
            }

            return results;
        }

        public Task<IEnumerable<LibraryDependency>> GetPackageReferencesAsync(
            NuGetFramework targetFramework, CancellationToken _)
        {
            throw new NotSupportedException();
        }
    }
}
