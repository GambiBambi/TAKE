using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace MvcNews.Models
{
    public class NewsItem
    {
        private int Id;

        private DateTime TimeStamp;

        private string Text = string.Empty;

        public int id
        {
            get => Id;
            set => Id = value;
        }

        [DataType(DataType.Date)]
        public DateTime timeStamp
        {
            get => TimeStamp;
            set => TimeStamp = value;
        }

        [Required]
        [StringLength(140, MinimumLength = 5, ErrorMessage = "too short")]
        public string text
        {
            get => Text;
            set => Text = value ?? throw new ArgumentNullException(nameof(value));
        }

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
