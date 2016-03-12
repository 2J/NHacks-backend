using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace nhacks.ViewModels.Picture
{
    public class ScanViewModel
    {
        [Required]
        public string ImgData { get; set; }

        [Required]
        public string GroupId { get; set; }
    }
}
