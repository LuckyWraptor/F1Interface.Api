using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using F1Interface.Contracts;
using F1Interface.Domain;
using F1Interface.Domain.Extensions;
using F1Interface.Domain.Models;
using F1Interface.Domain.Models.Internal;
using F1Interface.Domain.Responses;
using Microsoft.Extensions.Logging;

namespace F1Interface
{
    /// <summary>
    /// Simple API interface to retrieve FIA content data using a plain HttpClient
    /// </summary>
    public class ContentService : IContentService
    {
        private readonly ILogger<ContentService> logger;
        private readonly HttpClient httpClient;

        public ContentService(ILogger<ContentService> logger, HttpClient httpClient)
        {
            this.logger = logger;
            this.httpClient = httpClient;
        }
#region Seasons
        public Task<FIASeason> GetSeasonAsync(uint year, CancellationToken cancellationToken = default)
        {
            // Generate request string
            QueryStringBuilder builder = new QueryStringBuilder(Endpoints.F1TV.SearchEndpoint)
                .AddParameter(Constants.QueryParameters.FilterByType, "Meeting")
                .AddParameter(Constants.QueryParameters.OrderBy, "meeting_End_Date")
                .AddParameter(Constants.QueryParameters.SortOrder, "asc")
                .AddParameter(Constants.QueryParameters.FilterBySeason, year);

            return GetSeason(builder, cancellationToken);
        }
        public Task<FIASeason> GetCurrentSeasonAsync(CancellationToken cancellationToken = default)
            => GetSeasonAsync((uint)DateTime.Now.Year, cancellationToken); 
#endregion
#region Events
        public Task<FIAEvent[]> GetUpcomingEventsAsync(CancellationToken cancellationToken = default)
        {
            // Generate request string
            QueryStringBuilder builder = new QueryStringBuilder(Endpoints.F1TV.SearchEndpoint)
                .AddParameter(Constants.QueryParameters.FilterByType, "Meeting")
                .AddParameter(Constants.QueryParameters.OrderBy, "meeting_End_Date")
                .AddParameter(Constants.QueryParameters.SortOrder, "asc")
                .AddParameter(Constants.QueryParameters.FilterUpcomingEvent, "Y")
                .AddParameter(Constants.QueryParameters.FilterBySeason, (uint)DateTime.Now.Year);

            return GetEvents(builder, cancellationToken);
        }

        public Task<FIAEvent[]> GetPastEventsAsync(CancellationToken cancellationToken = default)
        {
            // Generate request string
            QueryStringBuilder builder = new QueryStringBuilder(Endpoints.F1TV.SearchEndpoint)
                .AddParameter(Constants.QueryParameters.FilterByType, "Meeting")
                .AddParameter(Constants.QueryParameters.OrderBy, "meeting_End_Date")
                .AddParameter(Constants.QueryParameters.SortOrder, "asc")
                .AddParameter(Constants.QueryParameters.FilterPastEvent, "Y")
                .AddParameter(Constants.QueryParameters.FilterBySeason, (uint)DateTime.Now.Year);

            return GetEvents(builder, cancellationToken);
        }

        public Task<FIAEvent> GetEventAsync(uint eventId, CancellationToken cancellationToken = default)
        {
            // Generate request string
            QueryStringBuilder queryBuilder = new QueryStringBuilder(Endpoints.F1TV.SearchEndpoint)
                .AddParameter(Constants.QueryParameters.FilterByEvent, eventId)
                .AddParameter(Constants.QueryParameters.OrderBy, "session_index")
                .AddParameter(Constants.QueryParameters.SortOrder, "asc");

            return GetEvent(queryBuilder, cancellationToken);
        }
        public Task<FIAEvent> GetEventAsync(uint eventId, string series, CancellationToken cancellationToken = default)
        {
            if (!Constants.Categories.KnownCategories.Contains(series))
            {
                throw new ArgumentException($"The specified series isn't supported, use one of {string.Join(", ", Constants.Categories.KnownCategories)}");
            }

            // Generate request string
            QueryStringBuilder queryBuilder = new QueryStringBuilder(Endpoints.F1TV.SearchEndpoint)
                .AddParameter(Constants.QueryParameters.FilterByEvent, eventId)
                .AddParameter(Constants.QueryParameters.FilterBySeries, series)
                .AddParameter(Constants.QueryParameters.OrderBy, "session_index")
                .AddParameter(Constants.QueryParameters.SortOrder, "asc");

            return GetEvent(queryBuilder, cancellationToken);
        }
#endregion
#region Content
        public async Task<Session> GetContentAsync(ulong contentId, CancellationToken cancellationToken = default)
        {
            if (contentId == 0)
            {
                throw new ArgumentException("The contentId can't be zero");
            }

            SeasonResponse result = null;
            try {
                string url = Endpoints.F1TV.ContentEndpoint.Replace("{{CONTENT_ID}}", contentId.ToString());
                logger.LogDebug("Requesting session data for id {ContentId} at {Url}", contentId);
                HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<SeasonResponse>()
                        .ConfigureAwait(false);
                }

            } catch (HttpRequestException ex) {
                throw new HttpException("Couldn't fetch seasonal data", ex.StatusCode.GetValueOrDefault());
            }

            
            if (result.ResultCode == "OK" && result.Result != null)
            {

                if (result.Result.Total > 0)
                {   
                    var container = result.Result.Containers[0];
                    Session session = new Session
                    {
                        Id = ulong.Parse(container.Id),
                        Series = container.Properties[0]?.Series,
                        IsLive = container.Metadata.Attributes.IsOnAir,
                        Testing = container.Metadata.Attributes.IsTestEvent,
                        EventId = uint.Parse(container.Metadata.Attributes.MeetingKey),
                        Starts = DateTimeUtils.UnixToDateTime(container.Metadata.ContractStartDate),
                        Ends = DateTimeUtils.UnixToDateTime(container.Metadata.ContractEndDate),
                        ImageId = container.Metadata.PictureId,
                    };

                    var attributes = container.Metadata.Attributes;
                    session.Event = new FIAEvent
                    {
                        Id = uint.Parse(attributes.MeetingKey),
                        Name = attributes.MeetingName,
                        OfficialName = attributes.MeetingOfficialName,
                        SeasonId = container.Metadata.Season,
                        Sponsor = attributes.MeetingSponsor,
                        Starts = attributes.MeetingStarts,
                        Ends = attributes.MeetingEnds,

                        Circuit = new Circuit
                        {
                            Id = attributes.CircuitKey,
                            Location = attributes.MeetingLocation,
                            Name = attributes.CircuitShortName,
                            OfficialName = attributes.CircuitShortName
                        }
                    };

                    return session;
                }
            }

            throw new F1InterfaceException("Couldn't parse session, this shouldn't happen!");
        }
#endregion

#region Private logic
        private async Task<FIAEvent[]> GetEvents(QueryStringBuilder queryBuilder, CancellationToken cancellationToken)
        {
            SeasonResponse result = null;
            try {
                string url = queryBuilder.ToString();
                logger.LogDebug("Requesting season events data using url {Url}", url);
                HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<SeasonResponse>()
                        .ConfigureAwait(false);
                }

            } catch (HttpRequestException ex) {
                throw new HttpException("Couldn't fetch seasonal data", ex.StatusCode.GetValueOrDefault());
            }

            
            if (result.ResultCode == "OK" && result.Result != null)
            {

                if (result.Result.Total > 0)
                {   
                    var events = result.Result.Containers.Select(x => new FIAEvent
                    {
                        Id = uint.Parse(x.Id),
                        Name = x.Metadata.TitleBrief,
                        OfficialName = x.Metadata.Title,
                        SeasonId = x.Metadata.Season,
                        Sponsor = x.Metadata.Attributes.MeetingSponsor,
                        Circuit = new Circuit
                        {
                            Id = x.Metadata.Attributes.CircuitKey,
                            Location = x.Metadata.Attributes.CircuitLocation,
                            Name = x.Metadata.Attributes.CircuitShortName,
                            OfficialName = x.Metadata.Attributes.CircuitOfficialName
                        },
                        Starts = x.Metadata.Attributes.MeetingStarts,
                        Ends = x.Metadata.Attributes.MeetingEnds
                    }).ToArray();

                    return events;
                }
            }
            
            
            throw new F1InterfaceException("The response was invalid, this shouldn't happen!");
        }
        
        private async Task<FIAEvent> GetEvent(QueryStringBuilder queryBuilder, CancellationToken cancellationToken)
        {
            EventResponse result = null;
            try {
                string url = queryBuilder.ToString();
                logger.LogDebug("Requesting season events data using url {Url}", url);
                HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<EventResponse>()
                        .ConfigureAwait(false);
                }

            } catch (HttpRequestException ex) {
                throw new HttpException("Couldn't fetch seasonal data", ex.StatusCode.GetValueOrDefault());
            }

            
            if (result.ResultCode == "OK" && result.Result != null)
            {

                if (result.Result.Total > 0)
                {
                    var metadata = result.Result.Containers.Where(x => x.Metadata != null)
                        .Select(x => x.Metadata)
                        .FirstOrDefault();

                    FIAEvent fiaEvent = new FIAEvent
                    {
                        Sessions = result.Result.Containers.Select(x => new Session
                        {
                            Id = ulong.Parse(x.Id),
                            EventId = uint.Parse(x.Metadata.Attributes.MeetingKey),
                            Testing = x.Metadata.Attributes.IsTestEvent,
                            Starts = DateTimeUtils.UnixToDateTime(x.Metadata.ContractStartDate),
                            Ends = DateTimeUtils.UnixToDateTime(x.Metadata.ContractEndDate),
                            ImageId = x.Metadata.PictureId
                        }).ToArray()
                    };

                    
                    fiaEvent.Id = uint.Parse(metadata.Attributes.MeetingKey);
                    fiaEvent.Name = metadata.Attributes.MeetingName;
                    fiaEvent.OfficialName = metadata.Attributes.MeetingOfficialName;
                    fiaEvent.Sponsor = metadata.Attributes.MeetingSponsor;
                    fiaEvent.SeasonId = metadata.Season;
                    fiaEvent.Starts = metadata.Attributes.MeetingStarts;
                    fiaEvent.Ends = metadata.Attributes.MeetingEnds;

                    return fiaEvent;
                }
            }
            
            
            throw new F1InterfaceException("The response was invalid, this shouldn't happen!");
        }

        private async Task<FIASeason> GetSeason(QueryStringBuilder queryBuilder, CancellationToken cancellationToken)
        {
            FIAEvent[] events = await GetEvents(queryBuilder, cancellationToken);

            FIASeason season = new FIASeason
            {
                Events = events,
                Season = events.Where(x => x.SeasonId > 0)
                    .Select(x => x.SeasonId)
                    .FirstOrDefault()
            };

            return season;
        }
#endregion
    }
}