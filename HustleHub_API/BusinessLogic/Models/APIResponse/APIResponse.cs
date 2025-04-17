using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HustleHub.BusinessArea.Models.APIResponse
{
    public class APIResponse
    {
        public int Code { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}
