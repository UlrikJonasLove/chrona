using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrona.Auth.Application.Features.Users.DTOs;

public record CurrentUserDTO(
	long Id,
	long OrganisationId,
	string Email,
	string FirstName,
	string LastName,
	IReadOnlyList<string> Roles);
