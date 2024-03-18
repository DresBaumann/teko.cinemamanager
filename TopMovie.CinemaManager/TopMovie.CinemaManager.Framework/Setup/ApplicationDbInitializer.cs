using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TopMovie.CinemaManager.Core.Features.Identity;
using TopMovie.CinemaManager.Core.Features.Memberships.Models;

namespace TopMovie.CinemaManager.Framework.Setup;

public static class ApplicationDbInitializer
{
	public static async Task Initialize(IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = serviceProvider.GetRequiredService<UserManager<Member>>();
		
		foreach (var roleEnum in Enum.GetValues(typeof(ApplicationRoles)))
		{
			var roleName = roleEnum.ToString();
			if (!await roleManager.RoleExistsAsync(roleName))
			{
				await roleManager.CreateAsync(new IdentityRole(roleName));
			}
		}
		
		var user = new Member { UserName = "dres.baumann@edu.teko.ch", Email = "dres.baumann@edu.teko.ch" };
		var userResult = await userManager.CreateAsync(user, "Passw0rd!");

		if (userResult.Succeeded)
		{
			await userManager.AddToRoleAsync(user, ApplicationRoles.Admin.ToString());
		}
	}
}