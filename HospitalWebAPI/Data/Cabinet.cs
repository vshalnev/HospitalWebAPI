using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWebAPI.Data
{
	[Table("Cabinet", Schema = "dbo")]
	public class Cabinet
	{
		[Key]
		public int Id { get; set; }
		public string Number { get; set; } = string.Empty;
	}
}

