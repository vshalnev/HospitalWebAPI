using System;
using Microsoft.EntityFrameworkCore;

namespace HospitalWebAPI.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{
		}

		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<Patient> Patients { get; set; }
		public DbSet<Cabinet> Cabinets { get; set; }
		public DbSet<Specialization> Specializations { get; set; }
		public DbSet<Area> Areas { get; set; }
	}
}

