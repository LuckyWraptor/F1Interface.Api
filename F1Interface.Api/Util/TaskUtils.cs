using System;
using System.Threading;
using System.Threading.Tasks;

namespace F1Interface.Api.Util
{
    public class TaskUtils
    {
        /// <summary>
        /// Randomly delay between page interactions.
        /// </summary>
        /// <param name="random">Random generator</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public static Task RandomDelay(Random random, CancellationToken cancellationToken = default)
            => RandomDelay(random, 500, 3000, cancellationToken);
        /// <summary>
        /// Randomly delay between page interactions.
        /// </summary>
        /// <param name="random">Random generator</param>
        /// <param name="minimumDelay"></param>
        /// <param name="maximumDelay"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static Task RandomDelay(Random random, int minimumDelay, int maximumDelay, CancellationToken cancellationToken = default)
        {
#if DEBUG
            // When we're unit-testing, we shouldn't really wait.
            if (XUnitUtils.IsUnitTesting)
                return Task.Delay(0, cancellationToken);
#endif

            return Task.Delay(GetRandomDelay(random, minimumDelay, maximumDelay), cancellationToken);
        }

        internal static int GetRandomDelay(Random random, int minimumDelay, int maximumDelay)
            => (random ?? new Random()).Next(minimumDelay, maximumDelay);
    }
}
