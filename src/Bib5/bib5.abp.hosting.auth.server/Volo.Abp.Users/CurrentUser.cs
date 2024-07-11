using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace Volo.Abp.Users;

[Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
public class CurrentUser : ICurrentUser, ITransientDependency
{
	private static readonly Claim[] EmptyClaimsArray = Array.Empty<Claim>();

	private readonly ICurrentPrincipalAccessor _principalAccessor;

	public virtual bool IsAuthenticated => Id.HasValue;

	public virtual Guid? Id => CurrentUserExtensions.FindClaimValue<Guid>((ICurrentUser)(object)this, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

	public virtual string? UserName => CurrentUserExtensions.FindClaimValue((ICurrentUser)(object)this, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

	public virtual string? Name => CurrentUserExtensions.FindClaimValue((ICurrentUser)(object)this, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");

	public virtual string? SurName => CurrentUserExtensions.FindClaimValue((ICurrentUser)(object)this, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");

	public virtual string? PhoneNumber => CurrentUserExtensions.FindClaimValue((ICurrentUser)(object)this, "phone_number");

	public virtual bool PhoneNumberVerified => string.Equals(CurrentUserExtensions.FindClaimValue((ICurrentUser)(object)this, "phone_number_verified"), "true", StringComparison.InvariantCultureIgnoreCase);

	public virtual string? Email => CurrentUserExtensions.FindClaimValue((ICurrentUser)(object)this, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

	public virtual bool EmailVerified => string.Equals(CurrentUserExtensions.FindClaimValue((ICurrentUser)(object)this, "email_verified"), "true", StringComparison.InvariantCultureIgnoreCase);

	public virtual Guid? TenantId
	{
		get
		{
			ClaimsPrincipal principal = _principalAccessor.Principal;
			if (principal == null)
			{
				return null;
			}
			return AbpClaimsIdentityExtensions.FindTenantId(principal);
		}
	}

	public virtual string[] Roles => (from c in FindClaims("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
		select c.Value).Distinct().ToArray();

	public CurrentUser(ICurrentPrincipalAccessor principalAccessor)
	{
		_principalAccessor = principalAccessor;
	}

	public virtual Claim? FindClaim(string claimType)
	{
		string claimType2 = claimType;
		return _principalAccessor.Principal?.Claims.FirstOrDefault((Claim c) => c.Type == claimType2);
	}

	public virtual Claim[] FindClaims(string claimType)
	{
		string claimType2 = claimType;
		return _principalAccessor.Principal?.Claims.Where((Claim c) => c.Type == claimType2).ToArray() ?? EmptyClaimsArray;
	}

	public virtual Claim[] GetAllClaims()
	{
		return _principalAccessor.Principal?.Claims.ToArray() ?? EmptyClaimsArray;
	}

	public virtual bool IsInRole(string roleName)
	{
		string roleName2 = roleName;
		return FindClaims("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Any((Claim c) => c.Value == roleName2);
	}
}
