using System;
using Billing.Application.DTOs;
using MediatR;

namespace Billing.Application.Queries;

public record GetBillByIdQuery(Guid Id) : IRequest<BillDto?>;
