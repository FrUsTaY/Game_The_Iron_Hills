namespace EpicBattle.Models
{
    public class GameSettings
    {
        public double MusicVolume { get; set; } = 0.3;
        public double SfxVolume { get; set; } = 0.8;

        public string DisplayMode { get; set; } = "В окне"; // "В окне", "В окне без рамки", "Во весь экран"
        public string Resolution { get; set; } = "1280x720";
    }
}
