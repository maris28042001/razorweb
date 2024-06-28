using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace razor.models
{
    public class Article{
        [Key]
        public int Id { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage ="{0} Phải dài từ {2} tới {1} ký tự")]
        [Required(ErrorMessage ="{0} phải nhập")]
        [Column(TypeName ="nvarchar")]
        [DisplayName("Tiêu đề")]
        public string Title { get; set;}

        [Required]
        [DisplayName("Ngày tạo")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set;}

        [DisplayName("Nội dung")]
        [Column(TypeName ="ntext")]
        public string Content { get; set;}
    }
}