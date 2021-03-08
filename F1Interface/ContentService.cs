using System;
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

            
            QueryStringBuilder builder = new QueryStringBuilder(Endpoints.F1TV.SearchEndpoint)
                .AddParameter(Constants.QueryParameters.SortOrder, "asc")
                .AddParameter(Constants.QueryParameters.FilterBySeason, seasonId)
                .AddParameter(Constants.QueryParameters.FilterOrderDate, "Y");
            if (seasonId < DateTime.UtcNow.Year)
            {
                builder.AddParameter(Constants.QueryParameters.FilterByEvent, "Replay")
                    .AddParameter(Constants.QueryParameters.OrderBy, "meeting_Number");
            }
            else
            {
                // Generate request string
                builder.AddParameter(Constants.QueryParameters.FilterByType, "Meeting")
                    .AddParameter(Constants.QueryParameters.OrderBy, "meeting_End_Date");
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
                .AddParameter(Constants.QueryParameters.FilterByType, "Meeting")
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
                .AddParameter(Constants.QueryParameters.FilterByType, "Meeting")
                .AddParameter(Constants.QueryParameters.OrderBy, "meeting_End_Date")
                .AddParameter(Constants.QueryParameters.SortOrder, "asc")
                .AddParameter(Constants.QueryParameters.FilterPastEvent, "Y")
                .AddParameter(Constants.QueryParameters.FilterOrderDate, "Y")
                .AddParameter(Constants.QueryParameters.FilterBySeason, (uint)DateTime.Now.Year);

            return GetEvents(builder, cancellationToken);
        }

        public Task<FIAEvent> GetEventAsync(uint eventId, CancellationToken cancellationToken = default)
            => GetEventAsync(eventId, null, cancellationToken);

        public Task<FIAEvent> GetEventAsync(uint eventId, string series, CancellationToken cancellationToken = default)
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
                series = series.Trim();
                if (!Constants.Categories.KnownCategories.Contains(series))
                {
                    throw new ArgumentException($"The specified series isn't supported, use one of {string.Join(", ", Constants.Categories.KnownCategories)}");
                }

                queryBuilder.AddParameter(Constants.QueryParameters.FilterBySeries, series);
            }


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

            string url = Endpoints.F1TV.ContentEndpoint.Replace("{{CONTENT_ID}}", contentId.ToString());
            SeasonResponse result = await RESTRequestObject<SeasonResponse>(url, cancellationToken);            
            if (result != null)
            {

                if (result.ResultCode == "OK" && result.Result != null && result.Result.Total > 0)
                {   
                    var container = result.Result.Containers[0];
                    Session session = new Session
                    {
                        Id = ulong.Parse(container.Id),
                        OfficialName = container.Metadata.Title,
                        Name = container.Metadata.TitleBrief,
                        Series = container.Properties[0]?.Series,
                        IsLive = container.Metadata.Attributes.IsOnAir,
                        Testing = container.Metadata.Attributes.IsTest,
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
                else
                {
                    return null;
                }
            }

            throw new F1InterfaceException("Couldn't parse session, this shouldn't happen!");
        }

        public async Task<Playback> GenerateStreamUrlAsync(ulong contentId, string subscriberToken, CancellationToken cancellationToken = default)
        {
            if (contentId > 0)
            {
                throw new ArgumentException("The contentId can't be zero");
            }
            else if (string.IsNullOrWhiteSpace(subscriberToken))
            {
                throw new ArgumentException("The subscriber token must be valid!");
            }

            PlaybackResponse result = null;
            try
            {
                string url = $"{Endpoints.F1TV.PlaybackEndpoint}{contentId}";
                logger.LogDebug("Requesting playback stream url for id {ContentId} at {Url}", contentId);
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
                    var events = result.Result.Containers.Select(x => new FIAEvent
                    {
                        Id = uint.Parse(x.Metadata.Attributes.MeetingKey),
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
                    var metadata = result.Result.Containers.Where(x => x.Metadata != null && x.Metadata.Attributes != null && x.Metadata.Attributes.MeetingKey != string.Empty)
                        .OrderByDescending(x => x.Metadata.Attributes.MeetingStarts)
                        .Select(x => x.Metadata)
                        .FirstOrDefault();

                    FIAEvent fiaEvent = new FIAEvent
                    {
                        Sessions = result.Result.Containers.Select(x => new Session
                        {
                            Id = ulong.Parse(x.Id),
                            OfficialName = x.Metadata.Title,
                            Name = x.Metadata.TitleBrief,
                            EventId = uint.Parse(x.Metadata.Attributes.MeetingKey),
                            Testing = x.Metadata.Attributes.IsTest,
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
                else
                {
                    return null;
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