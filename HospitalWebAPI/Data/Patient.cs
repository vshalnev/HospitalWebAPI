using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWebAPI.Data
{
	[Table("Patient", Schema = "dbo")]
	public class Patient
	{
		[Key]
		public int Id { get; set; }
		public string LastName { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string SecondName { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string Birthday { get; set; } = string.Empty;
		public Gender Gender { get; set; }
		public int AreaId { get; set; }
	}

	public enum Gender
    {
		Male = 0,
		Female = 1
    }

	public class PatientP
	{
		public int Id { get; set; }
		public string LastName { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string SecondName { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string Birthday { get; set; } = string.Empty;
		public string Gender { get; set; } = string.Empty;
		public string Area { get; set; } = string.Empty;
	}
}

