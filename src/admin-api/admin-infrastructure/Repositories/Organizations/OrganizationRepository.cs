using admin_application.Interfaces;

using admin_domain.Entities;

using admin_infrastructure.Db;

using FluentResults;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace admin_infrastructure.Repositories.Organizations;

public sealed class OrganizationRepository(FeatureToggleDbContext dbContext) : IOrganizationRepository
{
	public async Task<Result<Organization>> CreateAsync(Organization organization, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<OrganizationRepository>()
			.ForContext("Name", organization.Name);

		log.Information("Organization Create started");

		try
		{
			var entity = new Db.Entities.Organization { Id = organization.Id, Name = organization.Name };
			dbContext.Organizations.Add(entity);
			await dbContext.SaveChangesAsync(cancellationToken);
			log.Information("Organization Create completed");
			return Result.Ok(organization);
		}
		catch (DbUpdateException ex)
		{
			log.Error(ex, "Organization Create failed");
			return Result.Fail("Failed to create organization");
		}
	}

	public async Task<Result<Organization>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<OrganizationRepository>()
			.ForContext("Id", id);

		log.Information("Organization GetById started");

		var entity = await dbContext.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
		if (entity == null)
		{
			log.Information("Organization not found");
			return Result.Fail("NotFound");
		}

		var model = new Organization { Id = entity.Id, Name = entity.Name };
		log.Information("Organization GetById completed");
		return Result.Ok(model);
	}

	public async Task<Result<List<Organization>>> ListAsync(string? name, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<OrganizationRepository>()
			.ForContext("Name", name);

		log.Information("Organization List started");

		var query = dbContext.Organizations.AsNoTracking().AsQueryable();
		if (!string.IsNullOrWhiteSpace(name))
		{
			query = query.Where(o => o.Name.Contains(name));
		}

		var result = await query
			.OrderBy(o => o.Name)
			.Select(o => new Organization { Id = o.Id, Name = o.Name })
			.ToListAsync(cancellationToken);

		log.Information("Organization List completed: Count={Count}", result.Count);
		return Result.Ok(result);
	}

	public async Task<Result<Organization>> UpdateAsync(Organization organization, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<OrganizationRepository>()
			.ForContext("Id", organization.Id)
			.ForContext("Name", organization.Name);

		log.Information("Organization Update started");

		try
		{
			var affected = await dbContext.Organizations
				.Where(o => o.Id == organization.Id)
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(o => o.Name, organization.Name), cancellationToken);

			if (affected == 0)
			{
				log.Information("Organization to update not found");
				return Result.Fail("NotFound");
			}

			log.Information("Organization Update completed");
			return Result.Ok(organization);
		}
		catch (DbUpdateException ex)
		{
			log.Error(ex, "Organization Update failed");
			return Result.Fail("Failed to update organization");
		}
	}

	public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var log = Log.ForContext<OrganizationRepository>()
			.ForContext("Id", id);

		log.Information("Organization Delete started");

		var entity = await dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
		if (entity == null)
		{
			log.Information("Organization to delete not found");
			return Result.Fail("NotFound");
		}

		dbContext.Organizations.Remove(entity);

		try
		{
			await dbContext.SaveChangesAsync(cancellationToken);
			log.Information("Organization Delete completed");
			return Result.Ok();
		}
		catch (DbUpdateException ex)
		{
			log.Error(ex, "Organization Delete failed");
			return Result.Fail("Failed to delete organization");
		}
	}
}