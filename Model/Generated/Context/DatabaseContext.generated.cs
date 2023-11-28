//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
//
//     Produced by Entity Framework Visual Editor v4.2.5.1
//     Source:                    https://github.com/msawczyn/EFDesigner
//     Visual Studio Marketplace: https://marketplace.visualstudio.com/items?itemName=michaelsawczyn.EFDesigner
//     Documentation:             https://msawczyn.github.io/EFDesigner/
//     License (MIT):             https://github.com/msawczyn/EFDesigner/blob/master/LICENSE
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MiRest_V2
{
   /// <summary>
   /// Test
   /// </summary>
   /// <remarks>
   /// Test
   /// </remarks>
   public partial class DatabaseContext : DbContext
   {
      #region DbSets

      /// <summary>
      /// Repository for global::MiRest_V2.Tracking - TrackingModel
      /// </summary>
      public virtual Microsoft.EntityFrameworkCore.DbSet<global::MiRest_V2.Tracking> Tracking { get; set; }

      /// <summary>
      /// Repository for global::MiRest_V2.User - UserModel
      /// </summary>
      public virtual Microsoft.EntityFrameworkCore.DbSet<global::MiRest_V2.User> User { get; set; }

      #endregion DbSets

      /// <summary>
      /// Default connection string
      /// </summary>
      public static string ConnectionString { get; set; } = @"Name=dev";

      /// <summary>
      ///     <para>
      ///         Initializes a new instance of the <see cref="T:Microsoft.EntityFrameworkCore.DbContext" /> class using the specified options.
      ///         The <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" /> method will still be called to allow further
      ///         configuration of the options.
      ///     </para>
      /// </summary>
      /// <param name="options">The options for this context.</param>
      public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
      {
      }

      partial void CustomInit(DbContextOptionsBuilder optionsBuilder);

      /// <inheritdoc />
      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
         optionsBuilder.UseLazyLoadingProxies();

         CustomInit(optionsBuilder);
      }

      partial void OnModelCreatingImpl(ModelBuilder modelBuilder);
      partial void OnModelCreatedImpl(ModelBuilder modelBuilder);

      /// <summary>
      ///     Override this method to further configure the model that was discovered by convention from the entity types
      ///     exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
      ///     and re-used for subsequent instances of your derived context.
      /// </summary>
      /// <remarks>
      ///     If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
      ///     then this method will not be run.
      /// </remarks>
      /// <param name="modelBuilder">
      ///     The builder being used to construct the model for this context. Databases (and other extensions) typically
      ///     define extension methods on this object that allow you to configure aspects of the model that are specific
      ///     to a given database.
      /// </param>
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);
         OnModelCreatingImpl(modelBuilder);

         modelBuilder.HasDefaultSchema("dbo");

         modelBuilder.Entity<global::MiRest_V2.Tracking>()
                     .ToTable("Tracking")
                     .HasKey(t => t.TrackingId);
         modelBuilder.Entity<global::MiRest_V2.Tracking>()
                     .Property(t => t.TrackingId)
                     .HasMaxLength(50)
                     .ValueGeneratedOnAdd()
                     .IsRequired();
         modelBuilder.Entity<global::MiRest_V2.Tracking>()
                     .Property(t => t.Status)
                     .HasMaxLength(50);

         modelBuilder.Entity<global::MiRest_V2.User>()
                     .ToTable("User")
                     .HasKey(t => t.Id);
         modelBuilder.Entity<global::MiRest_V2.User>()
                     .Property(t => t.Id)
                     .ValueGeneratedOnAdd()
                     .IsRequired();
         modelBuilder.Entity<global::MiRest_V2.User>()
                     .Property(t => t.Username)
                     .HasMaxLength(100);
         modelBuilder.Entity<global::MiRest_V2.User>()
                     .Property(t => t.Password)
                     .HasMaxLength(100);

         OnModelCreatedImpl(modelBuilder);
      }
   }
}
