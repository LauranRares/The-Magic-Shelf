using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Models
{
    public class TheShoppingCart
    {
        public int Id { get; set; }

        public string TheUserId { get; set; }

        [ForeignKey("TheUserId")]
        public UserDB TheUser { get; set; }

        public int BookId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }
    }
}
