namespace City
{
    public class MapAccessMode
    {
        public string Label { get; set; }
        public string Background { get; set; }

        public static MapAccessMode EditMode = new MapAccessMode("EDIT MODE", "Red");
        public static MapAccessMode ViewMode = new MapAccessMode("VIEW MODE", "GreenYellow");

        public MapAccessMode(string label, string background)
        {
            Label = label;
            Background = background;
        }
    }
}