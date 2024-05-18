using System.ComponentModel.DataAnnotations;

namespace PatientApi.Models;

public class Name
{
    [Key]
    public Guid Id { get; set; }
    public string Use { get; set; }
    public string Family { get; set; }
    public List<string> Given { get; set; }
}
