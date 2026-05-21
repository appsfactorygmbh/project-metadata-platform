using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;

/// <summary>
/// Exception thrown when a business unit ist still connected to a team when trying to delete it.
/// </summary>
/// <param name="bu">The bu that cant be deleted.</param>
/// <param name="teamIds">List of connected teams.</param>
public class BusinessUnitStillLinkedToTeamsException(BusinessUnit bu, List<int> teamIds)
    : BusinessUnitException(
        $"The bu {bu.Id} ({bu.BusinessUnitName}) cant be deleted because it is still linked to these teams (ids): [{string.Join(", ", teamIds)}]"
    );
