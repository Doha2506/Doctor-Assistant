namespace Doctor_Assistant.Models
{
    public class Report
    {
        public Ray DR { get; set; } = new Ray();
        public Ray BrainTumor { get; set; } = new Ray();
        public Ray Alzehimer { get; set; } = new Ray();
        public StrokeDisease Stroke { get; set; } = new StrokeDisease();
    }
}
