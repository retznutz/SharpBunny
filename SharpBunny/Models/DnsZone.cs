using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class DnsZone
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("domain")]
    public string Domain { get; set; } = string.Empty;

    [JsonPropertyName("recordsCount")]
    public int RecordsCount { get; set; }

    [JsonPropertyName("dateModified")]
    public DateTime DateModified { get; set; }

    [JsonPropertyName("dateCreated")]
    public DateTime DateCreated { get; set; }

    [JsonPropertyName("nameservers")]
    public List<string> Nameservers { get; set; } = new();

    [JsonPropertyName("customNameservers")]
    public List<string> CustomNameservers { get; set; } = new();

    [JsonPropertyName("nameserversNextCheck")]
    public DateTime? NameserversNextCheck { get; set; }

    [JsonPropertyName("soaEmail")]
    public string SoaEmail { get; set; } = string.Empty;

    [JsonPropertyName("logAnonymizationType")]
    public int LogAnonymizationType { get; set; }

    [JsonPropertyName("logAnonymizationEnabled")]
    public bool LogAnonymizationEnabled { get; set; }

    [JsonPropertyName("logFormat")]
    public int LogFormat { get; set; }

    [JsonPropertyName("logForwardingEnabled")]
    public bool LogForwardingEnabled { get; set; }

    [JsonPropertyName("logForwardingHostname")]
    public string LogForwardingHostname { get; set; } = string.Empty;

    [JsonPropertyName("logForwardingPort")]
    public int LogForwardingPort { get; set; }

    [JsonPropertyName("logForwardingToken")]
    public string LogForwardingToken { get; set; } = string.Empty;

    [JsonPropertyName("logForwardingProtocol")]
    public int LogForwardingProtocol { get; set; }

    [JsonPropertyName("logForwardingTemplate")]
    public string LogForwardingTemplate { get; set; } = string.Empty;

    [JsonPropertyName("loggingEnabled")]
    public bool LoggingEnabled { get; set; }

    [JsonPropertyName("loggingIPAnonymizationEnabled")]
    public bool LoggingIPAnonymizationEnabled { get; set; }
}