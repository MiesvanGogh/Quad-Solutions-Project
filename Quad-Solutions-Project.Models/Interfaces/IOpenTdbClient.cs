using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quad_Solutions_Project.Models.Interfaces
{
    public interface IOpenTdbClient
    {
        /// <summary>
        ///     Retrieves a list of trivia questions from the Open Trivia Database.
        /// </summary>
        /// <param name="parameters">
        ///     The query parameters.
        /// <param name="cancellationToken">
        ///     The cancellation token to cancel the operation.  Optional.
        /// </param>
        /// <returns>
        ///     A list of <see cref="OpenTdbQuestion"/> objects.
        /// </returns>
        Task<List<OpenTdbQuestion>> GetQuestionsAsync(
            GetQuestionsParameters parameters,
            CancellationToken cancellationToken = default);

    }
}
