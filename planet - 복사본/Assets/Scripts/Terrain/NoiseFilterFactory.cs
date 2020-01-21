using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory
{ 
    public static INoiseFilter CreateNoiseFIlter(NoiseSettings settings)
    {
        switch(settings.filterType)
        {
            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(settings.simpleNoiseSettings);
            case NoiseSettings.FilterType.Ridgid:
                return new RidgidNoiseFilter(settings.ridgidNoiseSettings);
        }

        return null;
    }
}
