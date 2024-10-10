using System;
using FunGames.Core;

[Serializable]
public class FGAdUnit
{
    public string unitId;
    public FGAdType adType;
    public FGPlatform platform;
}

public enum FGPlatform
{
    IOS,
    Android
}