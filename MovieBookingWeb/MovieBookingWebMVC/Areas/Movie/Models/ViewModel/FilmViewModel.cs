using MovieBookingWebMVC.Areas.User.Models.DTOs;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingWebMVC.Areas.Movie.Models.ViewModel
{
    public class FilmViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên phim")]
        public string? Title { get; set; }

        public string? Tagline { get; set; }
        public string? Image { get; set; }
        public string? CarouselImage { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public IFormFile? CarouselImageFile { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập link trailer")]
        [Url(ErrorMessage = "URL trailer không hợp lệ")]
        public string? TrailerLink { get; set; }

        public string? BookingLink { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngôn ngữ phụ đề")]
        public string? Subtitle { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhãn kiểm duyệt")]
        public string? RatingCode { get; set; }

        public string? Rating { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thời lượng")]
        public int? Duration { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        public List<string>? Genres { get; set; }

        [NotMapped]
        public string? CastText { get; set; }  // ⬅️ textarea binding tại View

        [NotMapped]
        // KHÔNG CẦN Required ở đây vì nó do CastText xử lý
        public List<string>? Actors { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đạo diễn")]
        public string? Director { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả phim")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nhà sản xuất")]
        public string? ProductionCompany { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Vui lòng nhập định dạng")]
        public List<string>? Format { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Vui lòng nhập ngày khởi chiếu")]
        public DateTime? ReleaseDate { get; set; }

        public string? Status { get; set; }

        public string? CreatedByUsername { get; set; }
        public string? EditedByUsername { get; set; }

        public int? CreatedByUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public UserDto? CreatedByUser { get; set; }
    }
}
