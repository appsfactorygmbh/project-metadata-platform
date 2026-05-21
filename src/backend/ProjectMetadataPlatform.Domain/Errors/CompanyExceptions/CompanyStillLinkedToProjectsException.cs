using System;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;

/// <summary>
/// Exception thrown when a company ist still connected to a project when trying to delete it.
/// </summary>
/// <param name="company">The company that cant be deleted.</param>
/// <param name="projectIds">List of connected projects.</param>
public class CompanyStillLinkedToProjectsException(Company company, List<int> projectIds)
    : CompanyException(
        $"The Company {company.Id} ({company.CompanyName}) cant be deleted because it is still linked to these projects (ids): [{string.Join(", ", projectIds)}]"
    );
