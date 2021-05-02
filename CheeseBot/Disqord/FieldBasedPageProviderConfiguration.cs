namespace CheeseBot.Disqord
{
    public class FieldBasedPageProviderConfiguration
    {
        public static FieldBasedPageProviderConfiguration Default =>
            new FieldBasedPageProviderConfiguration().WithFieldsPerPage(5).AutoGenerateTitles();
        
        public string Content { get; set; }
        
        public int FieldsPerPage { get; set; }
        
        public bool AutoGeneratePageTitles { get; set; }

        public FieldBasedPageProviderConfiguration WithFieldsPerPage(int fieldsPerPage)
        {
            FieldsPerPage = fieldsPerPage;
            return this;
        }

        public FieldBasedPageProviderConfiguration AutoGenerateTitles()
        {
            AutoGeneratePageTitles = true;
            return this;
        }

        public FieldBasedPageProviderConfiguration WithContent(string content)
        {
            Content = content;
            return this;
        }
    }
}