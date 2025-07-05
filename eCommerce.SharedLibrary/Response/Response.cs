using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.SharedLibrary.Responses
{
    public record class Response (bool Flag = false, string? Message = null);
}
