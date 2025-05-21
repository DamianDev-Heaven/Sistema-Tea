using System.ComponentModel.DataAnnotations;

public class CARS2_Item
{
    [Key]
    public int ItemID { get; set; }

    [Required]
    [StringLength(5)]
    [RegularExpression("ST|HF")]
    public string Version { get; set; }

    [Required]
    public int Numero { get; set; }

    [StringLength(100)]
    public string? Nombre { get; set; }

    public string? Criterios { get; set; }
}