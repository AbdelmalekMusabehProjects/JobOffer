using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace JobOffer.Models
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Addressh> Addresshes { get; set; }
        public virtual DbSet<Applyjob> Applyjobs { get; set; }
        public virtual DbSet<Attchmenth> Attchmenths { get; set; }
        public virtual DbSet<Jobcategoryh> Jobcategoryhs { get; set; }
        public virtual DbSet<Jobh> Jobhs { get; set; }
        public virtual DbSet<Roleh> Rolehs { get; set; }
        public virtual DbSet<Testmonialh> Testmonialhs { get; set; }
        public virtual DbSet<Useraccounth> Useraccounths { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseOracle("USER ID=SALEH;PASSWORD=Saleh;DATA SOURCE=localhost:1521/orcl");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("SALEH")
                .HasAnnotation("Relational:Collation", "USING_NLS_COMP");

            modelBuilder.Entity<Addressh>(entity =>
            {
                entity.HasKey(e => e.Addressid)
                    .HasName("SYS_C00370675");

                entity.ToTable("ADDRESSH");

                entity.Property(e => e.Addressid)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ADDRESSID");

                entity.Property(e => e.Addresscity)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESSCITY");

                entity.Property(e => e.Addressname)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESSNAME");
            });


            modelBuilder.Entity<Applyjob>(entity =>
            {
                entity.HasKey(e => e.Applyid)
                    .HasName("SYS_C00371217");

                entity.ToTable("APPLYJOB");

                entity.Property(e => e.Applyid)
                    .HasColumnType("NUMBER(38)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("APPLYID");

                entity.Property(e => e.Attachid)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("ATTACHID");

                entity.Property(e => e.Jobid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("JOBID");

                entity.Property(e => e.Userid)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("USERID");

                entity.HasOne(d => d.Attach)
                    .WithMany(p => p.Applyjobs)
                    .HasForeignKey(d => d.Attachid)
                    .HasConstraintName("SYS_C00371219");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Applyjobs)
                    .HasForeignKey(d => d.Jobid)
                    .HasConstraintName("FK_PERSONORDER");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Applyjobs)
                    .HasForeignKey(d => d.Userid)
                    .HasConstraintName("SYS_C00371218");
            });

            modelBuilder.Entity<Attchmenth>(entity =>
            {
                entity.HasKey(e => e.Attachid)
                    .HasName("SYS_C00371214");

                entity.ToTable("ATTCHMENTH");

                entity.Property(e => e.Attachid)
                    .HasColumnType("NUMBER(38)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ATTACHID");

                entity.Property(e => e.Attchpath)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ATTACHPATH");

                entity.Property(e => e.Userid)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("USERID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Attchmenths)
                    .HasForeignKey(d => d.Userid)
                    .HasConstraintName("SYS_C00371215");
            });


            modelBuilder.Entity<Jobcategoryh>(entity =>
            {
                entity.HasKey(e => e.Jobcategoryid)
                    .HasName("SYS_C00370671");

                entity.ToTable("JOBCATEGORYH");

                entity.Property(e => e.Jobcategoryid)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("JOBCATEGORYID");

                entity.Property(e => e.Jobcategoryname)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("JOBCATEGORYNAME");
            });

            modelBuilder.Entity<Jobh>(entity =>
            {
                entity.HasKey(e => e.Jobid)
                    .HasName("SYS_C00370692");

                entity.ToTable("JOBH");

                entity.Property(e => e.Jobid)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("JOBID");

                entity.Property(e => e.Addressid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("ADDRESSID");

                entity.Property(e => e.Jobcategoryid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("JOBCATEGORYID");

                entity.Property(e => e.Jobdescription)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("JOBDESCRIPTION");

                entity.Property(e => e.Jobimage)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("JOBIMAGE");

                entity.Property(e => e.Jobname)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("JOBNAME");

                entity.Property(e => e.Jobsalary)
                    .HasColumnType("NUMBER")
                    .HasColumnName("JOBSALARY");

                entity.Property(e => e.Jobtype)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("JOBTYPE");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");

                entity.Property(e => e.Userid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("USERID");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Jobhs)
                    .HasForeignKey(d => d.Addressid)
                    .HasConstraintName("SYS_C00370694");

                entity.HasOne(d => d.Jobcategory)
                    .WithMany(p => p.Jobhs)
                    .HasForeignKey(d => d.Jobcategoryid)
                    .HasConstraintName("SYS_C00370695");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Jobhs)
                    .HasForeignKey(d => d.Userid)
                    .HasConstraintName("SYS_C00370693");
            });

            modelBuilder.Entity<Roleh>(entity =>
            {
                entity.HasKey(e => e.Roleid)
                    .HasName("SYS_C00370660");

                entity.ToTable("ROLEH");

                entity.Property(e => e.Roleid)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ROLEID");

                entity.Property(e => e.Rolename)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("ROLENAME");
            });

            modelBuilder.Entity<Testmonialh>(entity =>
            {
                entity.HasKey(e => e.Testmonialid)
                    .HasName("SYS_C00370698");

                entity.ToTable("TESTMONIALH");

                entity.Property(e => e.Testmonialid)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("TESTMONIALID");

                entity.Property(e => e.Message)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("MESSAGE");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");

                entity.Property(e => e.Userid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("USERID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Testmonialhs)
                    .HasForeignKey(d => d.Userid)
                    .HasConstraintName("SYS_C00370699");
            });

            modelBuilder.Entity<Useraccounth>(entity =>
            {
                entity.HasKey(e => e.Userid)
                    .HasName("SYS_C00370667");

                entity.ToTable("USERACCOUNTH");

                entity.Property(e => e.Userid)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("USERID");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("FULLNAME");

                entity.Property(e => e.Imagepath)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IMAGEPATH");

                entity.Property(e => e.Industialname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("INDUSTIALNAME");

                entity.Property(e => e.Password)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORD");

                entity.Property(e => e.Phonenumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PHONENUMBER");

                entity.Property(e => e.Roleid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("ROLEID");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("USERNAME");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Useraccounths)
                    .HasForeignKey(d => d.Roleid)
                    .HasConstraintName("SYS_C00370668");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
