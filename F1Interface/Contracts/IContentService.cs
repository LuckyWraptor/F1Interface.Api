using System.Threading;
using System.Threading.Tasks;
using F1Interface.Domain.Models;

namespace F1Interface.Contracts
{
    public interface IContentService
    {
        /// <summary>
        /// Fetch a season based on the year
        /// </summary>
        /// <param name="year">Season year (seasonId)</param>
        /// <exception cref="System.ArgumentException">If the year is zero</exception>
        /// <exception cref="F1Interface.Domain.HttpException">When a http response error ocurred</exception>
        /// <returns>AN FIA Season containing all events for the specified season</returns>
        Task<FIASeason> GetSeasonAsync(uint year, CancellationToken cancellationToken = default);
        /// <summary>
        /// FetcReh the current season.
        /// </summary>
        /// <remarks>
        /// The 'current' season is determined by the current UTC year to ensure the next season gets 'loaded in time'
        /// </remarks>
        /// <exception cref="F1Interface.Domain.HttpException">When a http response error ocurred</exception>
        /// <returns>AN FIA Season containing all events for the current season</returns>
        Task<FIASeason> GetCurrentSeasonAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve the upcoming events for the current season
        /// </summary>
        /// <exception cref="F1Interface.Domain.HttpException">When a http response error ocurred</exception>
        /// <returns>A list of upcoming events in the current season</returns>
        Task<FIAEvent[]> GetUpcomingEventsAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Retrieve the past events for the current season
        /// </summary>
        /// <exception cref="F1Interface.Domain.HttpException">When a http response error ocurred</exception>
        /// <returns>A list of past events in the current season</returns>
        Task<FIAEvent[]> GetPastEventsAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Retrieve specific event details and the containing planned sessions
        /// </summary>
        /// <param name="eventId">Event identifier</param>
        /// <exception cref="System.ArgumentException">If the eventId is zero</exception>
        /// <exception cref="F1Interface.Domain.HttpException">When a http response error ocurred</exception>
        /// <returns>The requested FIAEvent containing all planned sessions</returns>
        Task<FIAEvent> GetEventAsync(uint eventId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Retrieve specific event details and the containing planned sessions, filtered by a series
        /// </summary>
        /// <param name="eventId">Event identifier</param>
        /// <param name="series">Series identifier</param>
        /// <exception cref="System.ArgumentException">If the eventId is zero</exception>
        /// <exception cref="System.ArgumentException">If the series isn't recognised</exception>
        /// <exception cref="F1Interface.Domain.HttpException">When a http response error ocurred</exception>
        /// <returns>The requested FIAEvent containing all planned sessions for a specific series</returns>
        Task<FIAEvent> GetEventAsync(uint eventId, string series, CancellationToken cancellationToken = default);
        /// <summary>
        /// Retrieve specific event details and the containing planned sessions, filtered by a series
        /// </summary>
        /// <param name="eventId">Event identifier</param>
        /// <param name="series">Series identifier</param>
        /// <param name="contentType">Type of content to pull</param>
        /// <exception cref="System.ArgumentException">If the eventId is zero</exception>
        /// <exception cref="System.ArgumentException">If the series isn't recognised</exception>
        /// <exception cref="F1Interface.Domain.HttpException">When a http response error ocurred</exception>
        /// <returns>The requested FIAEvent containing all planned sessions for a specific series</returns>
        Task<FIAEvent> GetEventAsync(uint eventId, string series, ContentType contentType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve content information
        /// </summary>
        /// <remarks>
        /// A session also falls under content
        /// </remarks>
        /// <param name="contentId">Content identifier</param>
        /// <exception cref="System.ArgumentException">If the contentId is zero</exception>
        /// <exception cref="F1Interface.Domain.HttpException">When a http response error ocurred</exception>
        /// <returns>A Session instance containging the content details</returns>
        Task<FIASession> GetContentAsync(ulong contentId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Generate a stream url based on a content id and a subscriber token
        /// </summary>
        /// <param name="contentId">Content identifier</param>
        /// <param name="subscriberToken">Subscriber token string (must be valid)</param>
        /// <exception cref="System.ArgumentException">If the contentId is zero</exception>
        /// <exception cref="System.ArgumentException">If the subsriberToken is invalid</exception>
        /// <exception cref="F1Interface.Domain.HttpException">When a http response error ocurred</exception>
        /// <returns>A playback object containing the stream url</returns>
        Task<Playback> GenerateStreamUrlAsync(ulong contentId, string subscriberToken, CancellationToken cancellationToken = default);
        /// <summary>
        /// Generate a stream url based on a content id and a subscriber token
        /// </summary>
        /// <param name="contentId">Content identifier</param>
        /// <param name="channelId">Channel identifier, 0 means main content</param>
        /// <param name="subscriberToken">Subscriber token string (must be valid)</param>
        /// <exception cref="System.ArgumentException">If the contentId is zero</exception>
        /// <exception cref="System.ArgumentException">If the subsriberToken is invalid</exception>
        /// <exception cref="F1Interface.Domain.HttpException">When a http response error ocurred</exception>
        /// <returns>A playback object containing the stream url</returns>
        Task<Playback> GenerateStreamUrlAsync(ulong contentId, uint channelId, string subscriberToken, CancellationToken cancellationToken = default);
    }
}