using System.Collections;
using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

public record GetAllDepartmentsQuery : IRequest<IEnumerable<Department>>;
