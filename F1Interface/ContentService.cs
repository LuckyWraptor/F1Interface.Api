using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
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
        public Task<FIASeason> GetSeasonAsync(uint seasonId, CancellationToken cancellationToken = default)
        {
            if (seasonId == 0)
            {
                throw new ArgumentException("The season id (year) cannot be zero!");
            }

            // Generate request string
            QueryStringBuilder builder = new QueryStringBuilder(Endpoints.F1TV.SearchEndpoint)
                .AddParameter(Constants.QueryParameters.SortOrder, "asc")
                .AddParameter(Constants.QueryParameters.FilterOrderDate, "Y")
                .AddParameter(Constants.QueryParameters.FilterFetchAll, "Y");
            if (seasonId > 2017)
            {
                builder.AddParameter(Constants.QueryParameters.FilterByType, ContentParser.TypeToString(ContentType.Meeting))
                    .AddParameter(Constants.QueryParameters.OrderBy, "meeting_End_Date")
                    .AddParameter(Constants.QueryParameters.FilterBySeason, seasonId);
            }
            else
            {
                builder.AddParameter(Constants.QueryParameters.OrderBy, "meeting_Number")
                    .AddParameter(Constants.QueryParameters.FilterByYear, seasonId);
            }

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
                .AddParameter(Constants.QueryParameters.FilterByType, ContentParser.TypeToString(ContentType.Meeting))
                .AddParameter(Constants.QueryParameters.OrderBy, "meeting_End_Date")
                .AddParameter(Constants.QueryParameters.SortOrder, "asc")
                .AddParameter(Constants.QueryParameters.FilterUpcomingEvent, "Y")
                .AddParameter(Constants.QueryParameters.FilterOrderDate, "Y")
                .AddParameter(Constants.QueryParameters.FilterBySeason, (uint)DateTime.Now.Year);

            return GetEvents(builder, cancellationToken);
        }

        public Task<FIAEvent[]> GetPastEventsAsync(CancellationToken cancellationToken = default)
        {
            // Generate request string
            QueryStringBuilder builder = new QueryStringBuilder(Endpoints.F1TV.SearchEndpoint)
                .AddParameter(Constants.QueryParameters.FilterByType, ContentParser.TypeToString(ContentType.Meeting))
                .AddParameter(Constants.QueryParameters.OrderBy, "meeting_End_Date")
                .AddParameter(Constants.QueryParameters.SortOrder, "asc")
                .AddParameter(Constants.QueryParameters.FilterPastEvent, "Y")
                .AddParameter(Constants.QueryParameters.FilterOrderDate, "Y")
                .AddParameter(Constants.QueryParameters.FilterBySeason, (uint)DateTime.Now.Year);

            return GetEvents(builder, cancellationToken);
        }

        public Task<FIAEvent> GetEventAsync(uint eventId, CancellationToken cancellationToken = default)
            => GetEventAsync(eventId, null, ContentType.Unspecified, cancellationToken);

        public Task<FIAEvent> GetEventAsync(uint eventId, string series, CancellationToken cancellationToken = default)
            => GetEventAsync(eventId, series, ContentType.Unspecified, cancellationToken);

        public Task<FIAEvent> GetEventAsync(uint eventId, string series, ContentType contentType, CancellationToken cancellationToken = default)
        {
            if (eventId == 0)
            {
                throw new ArgumentException("The event id cannot be zero!");
            }

            // Generate request string
            QueryStringBuilder queryBuilder = new QueryStringBuilder(Endpoints.F1TV.SearchEndpoint)
                .AddParameter(Constants.QueryParameters.FilterByEvent, eventId)
                .AddParameter(Constants.QueryParameters.OrderBy, "session_index")
                .AddParameter(Constants.QueryParameters.SortOrder, "asc")
                .AddParameter(Constants.QueryParameters.FilterOrderDate, "Y");

            if (!string.IsNullOrWhiteSpace(series))
            {
                series = Uri.UnescapeDataString(series).Trim().ToUpper();
                if (!Constants.Categories.KnownCategories.Contains(series))
                {
                    throw new ArgumentException($"The specified series isn't supported, use one of {string.Join(", ", Constants.Categories.KnownCategories)}");
                }

                queryBuilder.AddParameter(Constants.QueryParameters.FilterBySeries, series);
            }

            if (contentType > ContentType.Unspecified)
            {
                string contentTypeString = ContentParser.TypeToString(contentType);
                queryBuilder.AddParameter(Constants.QueryParameters.FilterByType, contentTypeString); 
            }

            return GetEvent(queryBuilder, cancellationToken);
        }

        public Task<FIAEvent> GetEventWithScheduleAsync(uint eventId, CancellationToken cancellationToken = default)
        {
            return GetEventWithScheduleAsync(eventId, null, cancellationToken);
        }
        public Task<FIAEvent> GetEventWithScheduleAsync(uint eventId, string series, CancellationToken cancellationToken = default)
        {
            // Generate request string
            QueryStringBuilder queryBuilder = new QueryStringBuilder(Endpoints.F1TV.SearchEndpoint)
                .AddParameter(Constants.QueryParameters.FilterByEvent, eventId)
                .AddParameter(Constants.QueryParameters.OrderBy, "session_index")
                .AddParameter(Constants.QueryParameters.SortOrder, "asc")
                .AddParameter(Constants.QueryParameters.FilterOrderDate, "Y");

            return GetEventWithSchedule(queryBuilder, series, cancellationToken);
        }
#endregion
#region Content
        public async Task<FIASession> GetContentAsync(ulong contentId, CancellationToken cancellationToken = default)
        {
            if (contentId == 0)
            {
                throw new ArgumentException("The contentId can't be zero");
            }

            string url = Endpoints.F1TV.ContentEndpoint.Replace("{{CONTENT_ID}}", contentId.ToString());
            SessionResponse result = await RESTRequestObject<SessionResponse>(url, cancellationToken);            
            if (result != null)
            {

                if (result.ResultCode == "OK" && result.Result != null && result.Result.Total > 0)
                {
                    Session rawSession = result.Result.Containers[0];
                    SessionMetadata metadata = rawSession.Metadata;
                    FIASession session = new FIASession
                    {
                        Id = rawSession.Id,
                        OfficialName = metadata.Title,
                        Name = metadata.TitleBrief,
                        EventId = metadata.Attributes.MeetingId,
                        Testing = metadata.Attributes.IsTest,
                        Available = metadata.Attributes.IsOnAir,
                        Starts = DateTimeUtils.UnixToDateTime(metadata.ContractStartDate),
                        Ends = DateTimeUtils.UnixToDateTime(metadata.ContractEndDate),
                        ImageId = metadata.PictureId,
                        Series = rawSession.Properties[0]?.Series,
                        Type = ContentParser.DetermineType(metadata.ContentSubtype),
                        Duration = metadata.Duration,
                        Temporary = metadata.LeavingSoon,
                        Drivers = metadata.Actors,
                        Teams = metadata.Directors,
                        SideChannels = metadata.AdditionalStreams
                    };
                    session.Event = ParseEvent(metadata);
                    return session;
                }
                else
                {
                    return null;
                }
            }

            throw new F1InterfaceException("Couldn't parse session, this shouldn't happen!");
        }
        public Task<Playback> GenerateStreamUrlAsync(ulong contentId, string subscriberToken, CancellationToken cancellationToken = default)
            => GenerateStreamUrlAsync(contentId, 0, subscriberToken, cancellationToken);
        public async Task<Playback> GenerateStreamUrlAsync(ulong contentId, uint channelId, string subscriberToken, CancellationToken cancellationToken = default)
        {
            if (contentId == 0)
            {
                throw new ArgumentException("The contentId can't be zero");
            }
            else if (string.IsNullOrWhiteSpace(subscriberToken))
            {
                throw new ArgumentException("The subscriber token must be valid!");
            }

            QueryStringBuilder queryBuilder = new QueryStringBuilder(Endpoints.F1TV.PlaybackEndpoint);
                queryBuilder.AddParameter(Constants.QueryParameters.ContentId, contentId);

            if (channelId > 0)
            {
                queryBuilder.AddParameter(Constants.QueryParameters.ChannelId, channelId);
            }

            PlaybackResponse result = null;
            try
            {
                string url = queryBuilder.ToString();
                logger.LogDebug("Requesting playback stream url for id {ContentId} at {Url}", contentId, url);
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url),
                };
                request.Headers.Add("ascendontoken", subscriberToken);

                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<PlaybackResponse>()
                        .ConfigureAwait(false);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException("Couldn't generate the stream url", ex.StatusCode.GetValueOrDefault());
            }

            if (result != null)
            {
                if (result.ResultCode == "OK" && result.Result != null)
                {
                    result.Result.ContentId = contentId;

                    return result.Result;
                }
                else
                {
                    return null;
                }
            }

            throw new F1InterfaceException("Couldn't parse playback token, this shouldn't happen.");
        }
#endregion

#region Private logic
        private async Task<FIAEvent[]> GetEvents(QueryStringBuilder queryBuilder, CancellationToken cancellationToken)
        {
            SeasonResponse result = await RESTRequestObject<SeasonResponse>(queryBuilder, cancellationToken);

            if (result != null)
            {
                if (result.ResultCode == "OK" && result.Result != null && result.Result.Total > 0)
                {   
                    var events = result.Result.Containers
                        .Where(x => x.Metadata.Attributes.MeetingId > 0)
                        .OrderBy(x => x.Metadata.Attributes.MeetingStarts)
                        .GroupBy(x => x.Metadata.Attributes.MeetingId)
                        .Select(x => ParseEvent(x.First().Metadata))
                        .ToArray();

                    return events;
                }
                else
                {
                    return null;
                }
            }
            
            
            throw new F1InterfaceException("The response was invalid, this shouldn't happen!");
        }
        
        private async Task<FIAEvent> GetEvent(QueryStringBuilder queryBuilder, CancellationToken cancellationToken)
        {
            EventResponse result = await RESTRequestObject<EventResponse>(queryBuilder, cancellationToken);
                        
            if (result != null)
            {
                if (result.ResultCode == "OK" && result.Result != null && result.Result.Total > 0)
                {
                    var metadata = result.Result.Containers.Where(x => x.Metadata != null
                            && x.Metadata.Attributes != null
                            && x.Metadata.Attributes.MeetingId != 0)
                        .OrderByDescending(x => x.Metadata.Attributes.MeetingStarts)
                        .ThenByDescending(x => x.Metadata.Attributes.MeetingEnds)
                        .Select(x => x.Metadata)
                        .FirstOrDefault();

                    FIAEvent fiaEvent = ParseEvent(metadata);
                    fiaEvent.Sessions = result.Result.Containers.Select(x => ParseSession(x))
                        .ToArray();

                    return fiaEvent;
                }
                else
                {
                    return null;
                }
            }
            
            
            throw new F1InterfaceException("The response was invalid, this shouldn't happen!");
        }

        private async Task<FIAEvent> GetEventWithSchedule(QueryStringBuilder queryBuilder, string series, CancellationToken cancellationToken)
        {
            EventResponse result = await RESTRequestObject<EventResponse>(queryBuilder, cancellationToken);
            if (result != null && result.ResultCode != null && result.ResultCode == "OK"
                && result.Result != null && result.Result.Total > 0 && result.Result.Containers.Length > 0)
            {
                Event container = result.Result.Containers.FirstOrDefault(x => x.Metadata.ContentSubtype == "MEETING");
                if (container.Actions.Length > 0)
                {
                    var action = container.Actions.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Uri) && x.Key == "onClick");
                    if (action != null)
                    {
                        QueryStringBuilder scheduleQueryBuilder = new QueryStringBuilder(Endpoints.F1TV.ApiBase)
                            .AppendUri(action.Uri);

                        EventFullResponse schedule = await RESTRequestObject<EventFullResponse>(scheduleQueryBuilder, cancellationToken);
                        if (schedule != null && schedule.ResultCode != null && schedule.ResultCode == "OK"
                            && schedule.Result != null && schedule.Result.Total > 0)
                        {
                            EventFullContainer fullContainer = schedule.Result.Containers.FirstOrDefault(x => x.Layout == "schedule");
                            if (fullContainer != null && fullContainer.Items != null && fullContainer.Items.Categories != null
                                && fullContainer.Items.Categories.Total > 0)
                            {
                                EventFullCategory eventCategory = fullContainer.Items.Categories.Containers.FirstOrDefault(x => x.Name == "ALL");
                                if (eventCategory != null)
                                {
                                    Event[] sessions;
                                    if (!string.IsNullOrWhiteSpace(series))
                                    {
                                        sessions = eventCategory.Events.Where(x => x.Metadata.Type == "VIDEO" && x.Properties[0].Series == Uri.UnescapeDataString(series).Trim().ToUpper())
                                            .ToArray();
                                    }
                                    else
                                    {
                                        sessions = eventCategory.Events.Where(x => x.Metadata.Type == "VIDEO")
                                            .ToArray();
                                    }
                    
                                    if (sessions != null && sessions.Length > 0)
                                    {   
                                        var metadata = result.Result.Containers.Where(x => x.Metadata != null
                                                && x.Metadata.Attributes != null
                                                && x.Metadata.Attributes.MeetingId != 0)
                                            .OrderByDescending(x => x.Metadata.Attributes.MeetingStarts)
                                            .ThenByDescending(x => x.Metadata.Attributes.MeetingEnds)
                                            .Select(x => x.Metadata)
                                            .FirstOrDefault();

                                        FIAEvent fiaEvent = ParseEvent(metadata);
                                        fiaEvent.Sessions = sessions.Select(x => ParseSession(x))
                                            .ToArray();

                                        return fiaEvent;
                                    }
                                }
                            }

                            return null;
                        }
                    }
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
                Season = events?.Where(x => x.SeasonId > 0)
                    .Select(x => x.SeasonId)
                    .FirstOrDefault() ?? 0
            };

            return season;
        }
        private FIAEvent ParseEvent(EventMetadata metadata)
            => new FIAEvent
            {
                Id = metadata.Attributes.MeetingId,
                Name = metadata.Attributes.MeetingName,
                OfficialName = metadata.Attributes.MeetingOfficialName,
                SeasonId = metadata.Season,
                Sponsor = metadata.Attributes.MeetingSponsor,
                Starts = metadata.Attributes.MeetingStarts,
                Ends = metadata.Attributes.MeetingEnds,

                Circuit = new Circuit
                {
                    Id = metadata.Attributes.CircuitId,
                    Location = metadata.Attributes.MeetingLocation,
                    Name = metadata.Attributes.CircuitShortName,
                    OfficialName = metadata.Attributes.CircuitShortName
                }
            };

        private FIASession ParseSession(Event rawEvent)
        {
            EventMetadata metadata = rawEvent.Metadata;
            FIASession session = new FIASession
            {
                Id = rawEvent.Id,
                OfficialName = metadata.Title,
                Name = metadata.TitleBrief,
                EventId = metadata.Attributes.MeetingId,
                Testing = metadata.Attributes.IsTest,
                Available = metadata.Attributes.IsOnAir,
                ImageId = metadata.PictureId,
                Series = rawEvent.Properties[0]?.Series,
                Type = ContentParser.DetermineType(metadata.ContentSubtype),
                Duration = metadata.Duration
            };

            session.Starts = DateTimeUtils.UnixToDateTime(metadata.Attributes?.SessionStartDate > 0 ? metadata.Attributes.SessionStartDate : metadata.ContractStartDate);
            session.Ends = DateTimeUtils.UnixToDateTime(metadata.Attributes?.SessionEndDate > 0 ? metadata.Attributes.SessionEndDate : metadata.ContractEndDate);
            return session;
        }

        private Task<T> RESTRequestObject<T>(QueryStringBuilder queryBuilder, CancellationToken cancellationToken) where T : class
            => RESTRequestObject<T>(queryBuilder.ToString(), cancellationToken);

        private async Task<T> RESTRequestObject<T>(string url, CancellationToken cancellationToken) where T : class
        {
            T result = null;
            try
            {
                logger.LogDebug("Requesting content data using url {Url}", url);
                HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound)
                {
                    result = await response.Content.ReadFromJsonAsync<T>(null, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException("Couldn't fetch content data", ex.StatusCode.GetValueOrDefault());
            }

            return result;
        }
#endregion
    }
}
