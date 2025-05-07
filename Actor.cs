using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsoleApp
{
	public class Actor
	{
		[Key]
		[JsonPropertyName("Id")]
		public int Id { get; set; } = default!;

		[JsonPropertyName("Name")]
		public string Name { get; set; } = default!;

		[JsonPropertyName("BirthYear")]
		public int BirthYear { get; set; } = default!;
	}
}