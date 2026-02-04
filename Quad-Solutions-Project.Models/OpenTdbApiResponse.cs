using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quad_Solutions_Project.Models
{
    public class OpenTdbApiResponse
    {
        /// <summary>
        ///     Gets or sets numeric status code returned by the Open Trivia Database.
        /// </summary>
        public int ResponseCode { get; set; } = 0;

        /// <summary>
        ///     The list of trivia questions returned by the API.
        /// </summary>
        public List<OpenTdbQuestion> Results { get; set; } = new();
    }
}
