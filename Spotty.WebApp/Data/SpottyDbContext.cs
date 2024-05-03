using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Spotty.WebApp.Data;

public class SpottyDbContext(DbContextOptions<SpottyDbContext> options) : IdentityDbContext(options)
{
}
