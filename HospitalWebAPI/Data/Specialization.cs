using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWebAPI.Data
{
	[Table("Specialization", Schema = "dbo")]
	public class Specialization
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
	}
}

