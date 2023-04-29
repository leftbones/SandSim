using Newtonsoft.Json;

namespace SharpSand;

static class Atlas {
    public static Dictionary<string, Entry> Entries = JsonConvert.DeserializeObject<Dictionary<string, Entry>>(File.ReadAllText("elements/element_atlas.json"))!;
}

class Entry {
    public string DisplayName = "Unknown Element";
    public string Description = "This element doesn't exist!";
    public ElementType ElementType = ElementType.None;
}