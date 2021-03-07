using System.Threading;
using System.Threading.Tasks;
using F1Interface.Domain.Models;

namespace F1Interface.Contracts
{
    public interface IContentService
    {
        Task<FIASeason> GetSeasonAsync(uint year, CancellationToken cancellationToken = default);
        Task<FIASeason> GetCurrentSeasonAsync(CancellationToken cancellationToken = default);
        Task<FIAEvent[]> GetUpcomingEventsAsync(CancellationToken cancellationToken = default);
        Task<FIAEvent[]> GetPastEventsAsync(CancellationToken cancellationToken = default);

        Task<FIAEvent> GetEventAsync(uint eventId, CancellationToken cancellationToken = default);
        Task<FIAEvent> GetEventAsync(uint eventId, string series, CancellationToken cancellationToken = default);

        Task<Session> GetContentAsync(ulong contentId, CancellationToken cancellationToken = default);

        Task<Playback> GenerateStreamUrlAsync(ulong contentId, string subscriberToken, CancellationToken cancellationToken = default);
    }
}