namespace Frontend.Models
{
    public static class Links
    {
        public static class HeadteacherBoard
        {
            public static readonly LinkItem Preview = new LinkItem
                {PageName = "/TaskList/HtbDocument/Preview", BackText = "Back to preview page"};
        }
        
        public static class ProjectType
        {
            public static readonly LinkItem Index = new LinkItem {BackText = "Back to project type", PageName = "/ProjectType/Index"};
        }

        public static class ProjectList
        {
            public static readonly LinkItem Index = new LinkItem { PageName = "/Home/Index" };
        }
        public static class LegalRequirements
        {
            public static readonly LinkItem Index = new LinkItem { PageName = "/Projects/LegalRequirements/Index" };
        }
    }

    public class LinkItem
    {
        public string PageName { get; set; }
        public string BackText { get; set; }
    }
}
