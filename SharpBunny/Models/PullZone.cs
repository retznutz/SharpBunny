using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class PullZone
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("originUrl")]
    public string OriginUrl { get; set; } = string.Empty;

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("hostnames")]
    public List<Hostname> Hostnames { get; set; } = new();

    [JsonPropertyName("storageZoneId")]
    public int? StorageZoneId { get; set; }

    [JsonPropertyName("zonePricingTier")]
    public int ZonePricingTier { get; set; }

    [JsonPropertyName("monthlyBandwidthLimit")]
    public long MonthlyBandwidthLimit { get; set; }

    [JsonPropertyName("monthlyBandwidthUsed")]
    public long MonthlyBandwidthUsed { get; set; }

    [JsonPropertyName("monthlyCharges")]
    public decimal MonthlyCharges { get; set; }

    [JsonPropertyName("addHostHeader")]
    public bool AddHostHeader { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("accessControlOriginHeaderExtensions")]
    public List<string> AccessControlOriginHeaderExtensions { get; set; } = new();

    [JsonPropertyName("enableAccessControlOriginHeader")]
    public bool EnableAccessControlOriginHeader { get; set; }

    [JsonPropertyName("disableCookies")]
    public bool DisableCookies { get; set; }

    [JsonPropertyName("budgetRedirectedCountries")]
    public List<string> BudgetRedirectedCountries { get; set; } = new();

    [JsonPropertyName("blockedCountries")]
    public List<string> BlockedCountries { get; set; } = new();

    [JsonPropertyName("enableOriginShield")]
    public bool EnableOriginShield { get; set; }

    [JsonPropertyName("originShieldEnableQueueing")]
    public bool OriginShieldEnableQueueing { get; set; }

    [JsonPropertyName("originShieldMaxQueueingSeconds")]
    public int OriginShieldMaxQueueingSeconds { get; set; }

    [JsonPropertyName("originShieldMaxQueueingRequests")]
    public int OriginShieldMaxQueueingRequests { get; set; }

    [JsonPropertyName("originShieldZoneCode")]
    public string OriginShieldZoneCode { get; set; } = string.Empty;

    [JsonPropertyName("originConnectTimeout")]
    public int OriginConnectTimeout { get; set; }

    [JsonPropertyName("originResponseTimeout")]
    public int OriginResponseTimeout { get; set; }

    [JsonPropertyName("useStaleWhileUpdating")]
    public bool UseStaleWhileUpdating { get; set; }

    [JsonPropertyName("useStaleWhileOffline")]
    public bool UseStaleWhileOffline { get; set; }

    [JsonPropertyName("originRetries")]
    public int OriginRetries { get; set; }

    [JsonPropertyName("enableSmartCache")]
    public bool EnableSmartCache { get; set; }

    [JsonPropertyName("cacheControlMaxAgeOverride")]
    public long CacheControlMaxAgeOverride { get; set; }

    [JsonPropertyName("cacheControlPublicMaxAgeOverride")]
    public long CacheControlPublicMaxAgeOverride { get; set; }

    [JsonPropertyName("cacheControlBrowserMaxAgeOverride")]
    public long CacheControlBrowserMaxAgeOverride { get; set; }

    [JsonPropertyName("addCanonicalHeader")]
    public bool AddCanonicalHeader { get; set; }

    [JsonPropertyName("enableLogging")]
    public bool EnableLogging { get; set; }

    [JsonPropertyName("ignoreBotTraffic")]
    public bool IgnoreBotTraffic { get; set; }

    [JsonPropertyName("logFormat")]
    public int LogFormat { get; set; }

    [JsonPropertyName("logAnonymizationType")]
    public int LogAnonymizationType { get; set; }

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

    [JsonPropertyName("logSamplingPercentage")]
    public double LogSamplingPercentage { get; set; }

    [JsonPropertyName("edgeScriptId")]
    public int EdgeScriptId { get; set; }

    [JsonPropertyName("enableEdgeRules")]
    public bool EnableEdgeRules { get; set; }

    [JsonPropertyName("edgeRules")]
    public List<EdgeRule> EdgeRules { get; set; } = new();

    [JsonPropertyName("enableWebPVary")]
    public bool EnableWebPVary { get; set; }

    [JsonPropertyName("enableAvifVary")]
    public bool EnableAvifVary { get; set; }

    [JsonPropertyName("enableCountryCodeVary")]
    public bool EnableCountryCodeVary { get; set; }

    [JsonPropertyName("enableHostnameVary")]
    public bool EnableHostnameVary { get; set; }

    [JsonPropertyName("enableMobileVary")]
    public bool EnableMobileVary { get; set; }

    [JsonPropertyName("cnameDomain")]
    public string CnameDomain { get; set; } = string.Empty;

    [JsonPropertyName("dateModified")]
    public DateTime DateModified { get; set; }

    [JsonPropertyName("dateCreated")]
    public DateTime DateCreated { get; set; }
}

public class Hostname
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("forceSSL")]
    public bool ForceSSL { get; set; }

    [JsonPropertyName("isSystemHostname")]
    public bool IsSystemHostname { get; set; }

    [JsonPropertyName("hasValidCertificate")]
    public bool HasValidCertificate { get; set; }

    [JsonPropertyName("certificateKey")]
    public string CertificateKey { get; set; } = string.Empty;

    [JsonPropertyName("certificate")]
    public string Certificate { get; set; } = string.Empty;
}

public class EdgeRule
{
    [JsonPropertyName("guid")]
    public string Guid { get; set; } = string.Empty;

    [JsonPropertyName("actionType")]
    public int ActionType { get; set; }

    [JsonPropertyName("actionParameter1")]
    public string ActionParameter1 { get; set; } = string.Empty;

    [JsonPropertyName("actionParameter2")]
    public string ActionParameter2 { get; set; } = string.Empty;

    [JsonPropertyName("triggers")]
    public List<EdgeRuleTrigger> Triggers { get; set; } = new();

    [JsonPropertyName("triggerMatchingType")]
    public int TriggerMatchingType { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}

public class EdgeRuleTrigger
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("patternMatches")]
    public List<string> PatternMatches { get; set; } = new();

    [JsonPropertyName("patternMatchingType")]
    public int PatternMatchingType { get; set; }

    [JsonPropertyName("parameter1")]
    public string Parameter1 { get; set; } = string.Empty;
}

public class CreatePullZoneRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("originUrl")]
    public string OriginUrl { get; set; } = string.Empty;

    [JsonPropertyName("storageZoneId")]
    public int? StorageZoneId { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; } = 0; // 0 = Standard, 1 = Volume

    [JsonPropertyName("zonePricingTier")]
    public int ZonePricingTier { get; set; } = 0; // 0 = Standard, 1 = High Volume
}

public class UpdatePullZoneRequest
{
    [JsonPropertyName("originUrl")]
    public string? OriginUrl { get; set; }

    [JsonPropertyName("allowedReferrers")]
    public List<string>? AllowedReferrers { get; set; }

    [JsonPropertyName("blockedReferrers")]
    public List<string>? BlockedReferrers { get; set; }

    [JsonPropertyName("blockedIps")]
    public List<string>? BlockedIps { get; set; }

    [JsonPropertyName("enableGeoZoneUS")]
    public bool? EnableGeoZoneUS { get; set; }

    [JsonPropertyName("enableGeoZoneEU")]
    public bool? EnableGeoZoneEU { get; set; }

    [JsonPropertyName("enableGeoZoneASIA")]
    public bool? EnableGeoZoneASIA { get; set; }

    [JsonPropertyName("zonePricingTier")]
    public int? ZonePricingTier { get; set; }

    [JsonPropertyName("addHostHeader")]
    public bool? AddHostHeader { get; set; }

    [JsonPropertyName("addCanonicalHeader")]
    public bool? AddCanonicalHeader { get; set; }

    [JsonPropertyName("enableLogging")]
    public bool? EnableLogging { get; set; }

    [JsonPropertyName("disableCookies")]
    public bool? DisableCookies { get; set; }

    [JsonPropertyName("enableOriginShield")]
    public bool? EnableOriginShield { get; set; }

    [JsonPropertyName("originShieldZoneCode")]
    public string? OriginShieldZoneCode { get; set; }

    [JsonPropertyName("cacheControlMaxAgeOverride")]
    public long? CacheControlMaxAgeOverride { get; set; }

    [JsonPropertyName("cacheControlPublicMaxAgeOverride")]
    public long? CacheControlPublicMaxAgeOverride { get; set; }

    [JsonPropertyName("cacheControlBrowserMaxAgeOverride")]
    public long? CacheControlBrowserMaxAgeOverride { get; set; }

    [JsonPropertyName("accessControlOriginHeaderExtensions")]
    public List<string>? AccessControlOriginHeaderExtensions { get; set; }

    [JsonPropertyName("enableAccessControlOriginHeader")]
    public bool? EnableAccessControlOriginHeader { get; set; }

    [JsonPropertyName("budgetRedirectedCountries")]
    public List<string>? BudgetRedirectedCountries { get; set; }

    [JsonPropertyName("blockedCountries")]
    public List<string>? BlockedCountries { get; set; }

    [JsonPropertyName("enableSmartCache")]
    public bool? EnableSmartCache { get; set; }

    [JsonPropertyName("useStaleWhileUpdating")]
    public bool? UseStaleWhileUpdating { get; set; }

    [JsonPropertyName("useStaleWhileOffline")]
    public bool? UseStaleWhileOffline { get; set; }

    [JsonPropertyName("originConnectTimeout")]
    public int? OriginConnectTimeout { get; set; }

    [JsonPropertyName("originResponseTimeout")]
    public int? OriginResponseTimeout { get; set; }

    [JsonPropertyName("originRetries")]
    public int? OriginRetries { get; set; }

    [JsonPropertyName("edgeScriptId")]
    public int? EdgeScriptId { get; set; }

    [JsonPropertyName("logFormat")]
    public int? LogFormat { get; set; }

    [JsonPropertyName("logAnonymizationType")]
    public int? LogAnonymizationType { get; set; }

    [JsonPropertyName("logForwardingEnabled")]
    public bool? LogForwardingEnabled { get; set; }

    [JsonPropertyName("logForwardingHostname")]
    public string? LogForwardingHostname { get; set; }

    [JsonPropertyName("logForwardingPort")]
    public int? LogForwardingPort { get; set; }

    [JsonPropertyName("logForwardingToken")]
    public string? LogForwardingToken { get; set; }

    [JsonPropertyName("logForwardingProtocol")]
    public int? LogForwardingProtocol { get; set; }

    [JsonPropertyName("logForwardingTemplate")]
    public string? LogForwardingTemplate { get; set; }

    [JsonPropertyName("logSamplingPercentage")]
    public double? LogSamplingPercentage { get; set; }

    [JsonPropertyName("monthlyBandwidthLimit")]
    public long? MonthlyBandwidthLimit { get; set; }

    [JsonPropertyName("ignoreBotTraffic")]
    public bool? IgnoreBotTraffic { get; set; }

    [JsonPropertyName("enableWebPVary")]
    public bool? EnableWebPVary { get; set; }

    [JsonPropertyName("enableAvifVary")]
    public bool? EnableAvifVary { get; set; }

    [JsonPropertyName("enableCountryCodeVary")]
    public bool? EnableCountryCodeVary { get; set; }

    [JsonPropertyName("enableHostnameVary")]
    public bool? EnableHostnameVary { get; set; }

    [JsonPropertyName("enableMobileVary")]
    public bool? EnableMobileVary { get; set; }

    [JsonPropertyName("cnameDomain")]
    public string? CnameDomain { get; set; }
}