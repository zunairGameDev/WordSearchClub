using System;
using FunGames.Mediation;

public class FGAdInfo
{
    private string _defaultPlacement = FGMediationManager.DEFAULT_PLACEMENT_NAME;

    public string AdUnitIdentifier { get; set; }
    public string AdFormat { get; private set; }
    public string NetworkName    { get; private set; }
    public string NetworkPlacement    { get; private set; }

    public string Placement
    {
        get => GetDefault();
        set => SetDefault(value);
    }

    public string CreativeIdentifier { get; private set; }
    public double Revenue { get; private set; }
    public string RevenuePrecision { get; private set; }

    public FGAdInfo()
    {
        Reset();
    }
    
    public FGAdInfo(string AdUnitIdentifier, string AdFormat, string NetworkName, string NetworkPlacement,
        string Placement, string CreativeIdentifier, double Revenue, string RevenuePrecision)
    {
        this.AdUnitIdentifier = AdUnitIdentifier;
        this.AdFormat = AdFormat;
        this.NetworkName = NetworkName;
        this.NetworkPlacement = NetworkPlacement;
        this.Placement = Placement;
        this.CreativeIdentifier = CreativeIdentifier;
        this.Revenue = Revenue;
        this.RevenuePrecision = RevenuePrecision;
    }

    public void Reset()
    {
        this.AdUnitIdentifier = String.Empty;
        this.AdFormat = String.Empty;
        this.NetworkName = String.Empty;
        this.NetworkPlacement = String.Empty;
        this.Placement = FGMediationManager.DEFAULT_PLACEMENT_NAME;
        this.CreativeIdentifier = String.Empty;
        this.Revenue = 0;
        this.RevenuePrecision = String.Empty;
    }


    private string GetDefault()
    {
        if (string.IsNullOrEmpty(_defaultPlacement)) return FGMediationManager.DEFAULT_PLACEMENT_NAME;
        return _defaultPlacement;
    }
    
    private void SetDefault(string value)
    {
        if (string.IsNullOrEmpty(value)) _defaultPlacement = FGMediationManager.DEFAULT_PLACEMENT_NAME;
        else _defaultPlacement = value;
    }

    public void Set(FGAdInfo adInfo)
    {
        this.AdUnitIdentifier = adInfo.AdUnitIdentifier;
        this.AdFormat = adInfo.AdFormat;
        this.NetworkName = adInfo.NetworkName;
        this.NetworkPlacement = adInfo.NetworkPlacement;
        this.Placement = adInfo.Placement;
        this.CreativeIdentifier = adInfo.CreativeIdentifier;
        this.Revenue = adInfo.Revenue;
        this.RevenuePrecision = adInfo.RevenuePrecision;
    }

    public override string ToString()
    {
        return "[AdInfo adUnitIdentifier: " + AdUnitIdentifier +
               ", adFormat: " + AdFormat +
               ", networkName: " + NetworkName +
               ", networkPlacement: " + NetworkPlacement +
               ", creativeIdentifier: " + CreativeIdentifier +
               ", placement: " + Placement +
               ", revenue: " + Revenue +
               ", revenuePrecision: " + RevenuePrecision + "]";
    }
}