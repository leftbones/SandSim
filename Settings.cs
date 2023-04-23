using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpSand;

class Settings {
    // Info Display Settings
    public bool DisplayHelpText { get; set; } = false;
    public bool ShowElementName { get; set; } = false;
    public bool DrawChunkBorders { get; set; } = false;

    // Weather Effects
    public bool WeatherEnabled { get; set; } = false;
    public int WeatherStrength { get; set; } = 1;
    public int WeatherSelected { get; set; } = 0;
    public string[] WeatherElements { get; } = new string[] { "Water", "Snow", "Ember" };
    private int MaxWeatherStrength = 10;

    // Simulation Speed
    public int SimulationSpeed { get; set; } = 200;
    public int SpeedSelected { get; set; } = 0;
    private int[] SpeedSettings = new int[] {25, 50, 100, 200, 400, 800, 1600, 9999};

    public Settings() { }

    public void CycleSimulationSpeed() {
        if (SpeedSelected + 1 > SpeedSettings.Length) SpeedSelected = 0;
        else SpeedSelected += 1;
        SimulationSpeed = SpeedSettings[SpeedSelected];
        SetTargetFPS(SimulationSpeed);
    }

    public void CycleWeatherElement() {
        if (WeatherSelected + 1 > WeatherElements.Length) WeatherSelected = 0;
        else WeatherSelected += 1;
    }

    public void ChangeWeatherStrength(int dir) {
        WeatherStrength += dir;
        WeatherStrength = Math.Clamp(WeatherStrength, 1, MaxWeatherStrength);
    }
}