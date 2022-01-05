using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class ChangeAdStatusModel // ChangeAdStatusModelValidator
    {
       public int AdId { get; set; }       
       public string Status { get; set; }
    }
}
