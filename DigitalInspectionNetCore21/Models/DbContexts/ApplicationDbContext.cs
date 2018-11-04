using System;
using System.Collections.Generic;
using System.Linq;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using Microsoft.EntityFrameworkCore;

namespace DigitalInspectionNetCore21.Models.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
		// TODO DJC EF Best Practice - USE THE NEW APPROACH SHOWN HERE. COMBINE WITH MOSH'S TUTORIAL ON REPOSITORY + UNIT OF WORK PATTERN
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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			ConfigureOneToManyRelationships(modelBuilder);
			ConfigureManyToManyRelationships(modelBuilder);
			ConfigureTypeConversions(modelBuilder);
		}

		#region One to Many

		// https://www.learnentityframeworkcore.com/configuration/one-to-many-relationship-configuration
		private void ConfigureOneToManyRelationships(ModelBuilder modelBuilder)
		{
			InspectionItemChecklistItemRelationship(modelBuilder);
			InspectionItemInspectionRelationship(modelBuilder);
			InspectionItemInspectionImageRelationship(modelBuilder);
			InspectionItemInspectionMeasurementRelationship(modelBuilder);
			MeasurementInspectionMeasurementRelationship(modelBuilder);
		}

		private void InspectionItemChecklistItemRelationship(ModelBuilder modelBuilder)
		{
		    modelBuilder.Entity<InspectionItem>()
			    .HasOne(ii => ii.ChecklistItem)
			    .WithMany(ci => ci.InspectionItems);

		    // Rename the shadow property EF Core will generate to match original EF 6 convention
		    modelBuilder.Entity<InspectionItem>()
			    .Property<Guid>("ChecklistItemId")
			    .HasColumnName("ChecklistItem_Id");
		}

		private void InspectionItemInspectionRelationship(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<InspectionItem>()
			    .HasOne(ii => ii.Inspection)
			    .WithMany(i => i.InspectionItems);

		    // Rename the shadow property EF Core will generate to match original EF 6 convention
			modelBuilder.Entity<InspectionItem>()
			    .Property<Guid>("InspectionId")
			    .HasColumnName("Inspection_Id");
		}

		private void InspectionItemInspectionImageRelationship(ModelBuilder modelBuilder)
		{
		    modelBuilder.Entity<InspectionImage>()
			    .HasOne(ii => ii.InspectionItem)
			    .WithMany(ii => ii.InspectionImages);

		    // Rename the shadow property EF Core will generate to match original EF 6 convention
		    modelBuilder.Entity<InspectionImage>()
			    .Property<Guid>("InspectionItemId")
			    .HasColumnName("InspectionItem_Id");
		}

	    private void InspectionItemInspectionMeasurementRelationship(ModelBuilder modelBuilder)
	    {
		    modelBuilder.Entity<InspectionMeasurement>()
			    .HasOne(ii => ii.InspectionItem)
			    .WithMany(ii => ii.InspectionMeasurements);

		    // Rename the shadow property EF Core will generate to match original EF 6 convention
		    modelBuilder.Entity<InspectionMeasurement>()
			    .Property<Guid>("InspectionItemId")
			    .HasColumnName("InspectionItem_Id");
	    }

	    private void MeasurementInspectionMeasurementRelationship(ModelBuilder modelBuilder)
	    {
		    modelBuilder.Entity<InspectionMeasurement>()
			    .HasOne(im => im.Measurement)
			    .WithMany(m => m.InspectionMeasurements);

		    // Rename the shadow property EF Core will generate to match original EF 6 convention
		    modelBuilder.Entity<InspectionMeasurement>()
			    .Property<Guid>("MeasurementId")
			    .HasColumnName("Measurement_Id");
	    }

		#endregion

		#region Many-To-Many

		/**
		 * References
		 * https://www.learnentityframeworkcore.com/configuration/many-to-many-relationship-configuration
		 * 4 part blog for possible refactor
		 * https://blog.oneunicorn.com/2017/09/25/many-to-many-relationships-in-ef-core-2-0-part-2-hiding-as-ienumerable/
		 */
		private void ConfigureManyToManyRelationships(ModelBuilder modelBuilder)
	    {
			ChecklistChecklistItemRelationship(modelBuilder);
		    ChecklistInspectionRelationship(modelBuilder);
		    ChecklistItemInspectionRelationship(modelBuilder);
		    ChecklistItemTagRelationship(modelBuilder);
		    InspectionItemCannedResponseRelationship(modelBuilder);
	    }

	    private void ChecklistChecklistItemRelationship(ModelBuilder modelBuilder)
	    {
		    modelBuilder.Entity<ChecklistChecklistItem>()
			    .HasKey(joinItem => new { joinItem.ChecklistId, joinItem.ChecklistItemId });

			modelBuilder.Entity<ChecklistChecklistItem>()
			    .HasOne(joinItem => joinItem.Checklist)
			    .WithMany(c => c.ChecklistChecklistItems)
			    .HasForeignKey(joinItem => joinItem.ChecklistId);

		    modelBuilder.Entity<ChecklistChecklistItem>()
			    .HasOne(joinItem => joinItem.ChecklistItem)
			    .WithMany(ci => ci.ChecklistChecklistItems)
			    .HasForeignKey(joinItem => joinItem.ChecklistItemId);
	    }

	    private void ChecklistInspectionRelationship(ModelBuilder modelBuilder)
	    {
		    modelBuilder.Entity<ChecklistInspection>()
			    .HasKey(joinItem => new { joinItem.ChecklistId, joinItem.InspectionId });

			modelBuilder.Entity<ChecklistInspection>()
			    .HasOne(joinItem => joinItem.Checklist)
			    .WithMany(c => c.ChecklistInspections)
			    .HasForeignKey(joinItem => joinItem.ChecklistId);

		    modelBuilder.Entity<ChecklistInspection>()
			    .HasOne(joinItem => joinItem.Inspection)
			    .WithMany(i => i.ChecklistInspections)
			    .HasForeignKey(joinItem => joinItem.InspectionId);
	    }

	    private void ChecklistItemInspectionRelationship(ModelBuilder modelBuilder)
	    {
		    modelBuilder.Entity<ChecklistItemInspection>()
			    .HasKey(joinItem => new { joinItem.ChecklistItemId, joinItem.InspectionId });

			modelBuilder.Entity<ChecklistItemInspection>()
			    .HasOne(joinItem => joinItem.ChecklistItem)
			    .WithMany(ci => ci.ChecklistItemInspections)
			    .HasForeignKey(joinItem => joinItem.ChecklistItemId);

		    modelBuilder.Entity<ChecklistItemInspection>()
			    .HasOne(joinItem => joinItem.Inspection)
			    .WithMany(i => i.ChecklistItemInspections)
			    .HasForeignKey(joinItem => joinItem.InspectionId);
	    }

	    private void ChecklistItemTagRelationship(ModelBuilder modelBuilder)
	    {
		    modelBuilder.Entity<ChecklistItemTag>()
			    .HasKey(joinItem => new { joinItem.ChecklistItemId, joinItem.TagId });

			modelBuilder.Entity<ChecklistItemTag>()
			    .HasOne(joinItem => joinItem.ChecklistItem)
			    .WithMany(ci => ci.ChecklistItemTags)
			    .HasForeignKey(joinItem => joinItem.ChecklistItemId);

		    modelBuilder.Entity<ChecklistItemTag>()
			    .HasOne(joinItem => joinItem.Tag)
			    .WithMany(t => t.ChecklistItemTags)
			    .HasForeignKey(joinItem => joinItem.TagId);
	    }

	    private void InspectionItemCannedResponseRelationship(ModelBuilder modelBuilder)
	    {
		    modelBuilder.Entity<InspectionItemCannedResponse>()
			    .HasKey(joinItem => new { joinItem.InspectionItemId, joinItem.CannedResponseId });

		    modelBuilder.Entity<InspectionItemCannedResponse>()
			    .HasOne(joinItem => joinItem.InspectionItem)
			    .WithMany(ii => ii.InspectionItemCannedResponses)
			    .HasForeignKey(joinItem => joinItem.InspectionItemId);

		    modelBuilder.Entity<InspectionItemCannedResponse>()
			    .HasOne(joinItem => joinItem.CannedResponse)
			    .WithMany(cr => cr.InspectionItemCannedResponses)
			    .HasForeignKey(joinItem => joinItem.CannedResponseId);
	    }

		#endregion

		#region Type Conversions

	    private void ConfigureTypeConversions(ModelBuilder modelBuilder)
	    {
		    modelBuilder
			    .Entity<InspectionItem>()
			    .Property(ii => ii.Condition)
			    .HasConversion<int>();

		    modelBuilder
			    .Entity<CannedResponse>()
			    .Property(ii => ii.LevelsOfConcern)
			    .HasConversion(
				    // Going into the DB
				    v => ConvertToString(v),
				    // Coming out of the DB
				    v => ConvertToRecommendServiceSeverities(v));
	    }

		private string ConvertToString(IList<InspectionItemCondition> levelsOfConcern)
	    {
		    return string.Join(",", levelsOfConcern);
		}

	    private IList<InspectionItemCondition> ConvertToRecommendServiceSeverities(string levelsOfConcernInDb)
	    {
			if (string.IsNullOrEmpty(levelsOfConcernInDb))
			{
				return new List<InspectionItemCondition>();
			}
			else
			{
				IList<string> stringList = levelsOfConcernInDb.Split(',').ToList();
				return stringList
					.Select(s => (InspectionItemCondition)Enum.Parse(typeof(InspectionItemCondition), s))
					.ToList();
			}
		}

		#endregion
	}
}
