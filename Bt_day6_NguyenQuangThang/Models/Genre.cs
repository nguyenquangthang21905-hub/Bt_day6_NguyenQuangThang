using System.ComponentModel.DataAnnotations;

namespace Bt_day6_NguyenQuangThang.Models;

public class Genre
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên thể loại.")]
    [StringLength(100, ErrorMessage = "Tên thể loại không được vượt quá 100 ký tự.")]
    [Display(Name = "Tên thể loại")]
    public string Name { get; set; } = string.Empty;

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}