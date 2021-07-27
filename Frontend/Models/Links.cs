namespace Frontend.Models
{
    public static class Links
    {
        public static class HeadteacherBoard
        {
            public static readonly LinkItem Preview = new LinkItem
                {PageName = "/TaskList/HtbDocument/Preview", BackText = "Back to preview page"};
        }
    }

    public class LinkItem
    {
        public string PageName { get; set; }
        public string BackText { get; set; }
    }
}