using DigitalInspectionNetCore21.Models.Inspections;
using Microsoft.EntityFrameworkCore;

namespace DigitalInspectionNetCore21.Models.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
		// FIXME DJC USE THE NEW APPROACH SHOWN HERE
		// https://docs.microsoft.com/en-us/ef/core/#the-model
		public DbSet<InspectionImage> InspectionImages { get; set; }
	    public DbSet<Inspection> Inspections { get; set; }
	    public DbSet<InspectionItem> InspectionItems { get; set; }
	    public DbSet<InspectionMeasurement> InspectionMeasurements { get; set; }
	    public DbSet<CannedResponse> CannedResponses { get; set; }
	    public DbSet<Measurement> Measurements { get; set; }
	    public DbSet<Checklist> Checklists { get; set; }
	    public DbSet<ChecklistItem> ChecklistItems { get; set; }
	    public DbSet<Tag> Tags { get; set; }

	    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	    {
	    }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
		}
	}
}
