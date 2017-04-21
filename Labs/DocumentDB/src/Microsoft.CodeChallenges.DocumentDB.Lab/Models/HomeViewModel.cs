namespace Microsoft.CodeChallenges.DocumentDB.Lab.Models
{
    public class HomeViewModel
    {
        public HomeViewModel(string[] availableRegions)
        {
            AvailableRegions = availableRegions;
        }
        public string[] AvailableRegions { get; set; }
    }
}