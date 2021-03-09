using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models
{
    public enum ContentType
    {
        Unknown,
        Analysis,
        Documentary,
        Feature,
        Highlights,
        Meeting,
        PressConference,
        Replay,
        Show,
        //Live?
    }
}
