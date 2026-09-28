using ProjectMetadataPlatform.Application.Helper.Models;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Application.Billing;

public record BillingCursor(string BillingKind) : Cursor;
