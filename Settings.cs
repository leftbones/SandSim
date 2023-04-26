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
    public int SpeedSelected { get; set; } = 1;
    private int[] SpeedSettings = new int[] {100, 200, 9999};

    public Settings() { }

    public void CycleSimulationSpeed() {
        if (SpeedSelected + 1 > SpeedSettings.Length - 1) SpeedSelected = 0;
        else SpeedSelected += 1;
        SimulationSpeed = SpeedSettings[SpeedSelected];
        SetTargetFPS(SimulationSpeed);

        switch (SimulationSpeed) {
            case 100:
                Console.WriteLine("[SYSTEM] Simulation speed set to SLOW");
                break;
            case 200:
                Console.WriteLine("[SYSTEM] Simulation speed set to NORMAL");
                break;
            case 9999:
                Console.WriteLine("[SYSTEM] Simulation speed set to UNLIMITED");
                break;
        }
    }

    public void CycleWeatherElement() {
        if (WeatherSelected + 1 > WeatherElements.Length - 1) WeatherSelected = 0;
        else WeatherSelected += 1;
        Console.WriteLine("[SYSTEM] Weather element set to " + WeatherElements[WeatherSelected].ToUpper());
    }

    public void ChangeWeatherStrength(int dir) {
        WeatherStrength += dir;
        WeatherStrength = Math.Clamp(WeatherStrength, 1, MaxWeatherStrength);
        Console.WriteLine("[SYSTEM] Weather strength set to " + WeatherStrength + "/" + MaxWeatherStrength);
    }
}