using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWebAPI.Data
{
	[Table("Area", Schema = "dbo")]
	public class Area
	{
		[Key]
		public int Id { get; set; }
		public string Number { get; set; } = string.Empty;
	}
}

