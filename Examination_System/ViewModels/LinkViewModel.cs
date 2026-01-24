namespace Examination_System.ViewModels
{
    public class LinkViewModel
    {
        public LinkViewModel(string? rel, string? href, string? method)
        {
            Rel = rel;
            Href = href;
            Method = method;
        }

        public string? Rel { get; set; }
        public string? Href { get; set; }
        public string? Method { get; set; }
    }
}
