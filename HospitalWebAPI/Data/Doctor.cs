using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWebAPI.Data
{
	[Table("Doctor", Schema = "dbo")]
	public class Doctor
	{
		[Key]
		public int Id { get; set; }
		public string FIO { get; set; } = string.Empty;
		public int CabinetId { get; set; }
		public int SpecializationId { get; set; }
		public int AreaId { get; set; }
	}

	public class DoctorP
	{
		public int Id { get; set; }
		public string FIO { get; set; } = string.Empty;
		public string Cabinet { get; set; } = string.Empty;
		public string Specialization { get; set; } = string.Empty;
		public string Area { get; set; } = string.Empty;
	}
}

