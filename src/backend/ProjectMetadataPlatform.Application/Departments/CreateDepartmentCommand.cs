using System.Collections;
using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

public record CreateDepartmentCommand(string DepartmentName) : IRequest<int>;
