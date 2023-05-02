using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TMS.Models
{
    public class BooksDB
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        [ValidateNever]
        public string Image { get; set; }

        public string Genre1 { get; set; }

        public string Genre2 { get; set; }

        public int Quantity { get; set; }

        [Range(1, 1000)]
        public double Price { get; set; }
    }
}
