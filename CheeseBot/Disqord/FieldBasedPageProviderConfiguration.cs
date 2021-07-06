namespace CheeseBot.Disqord
{
    public class FieldBasedPageProviderConfiguration
    {
        public static FieldBasedPageProviderConfiguration Default =>
            new FieldBasedPageProviderConfiguration().WithFieldsPerPage(5);
        
        public string Content { get; set; }
        
        public int FieldsPerPage { get; set; }

        public FieldBasedPageProviderConfiguration WithFieldsPerPage(int fieldsPerPage)
        {
            FieldsPerPage = fieldsPerPage;
            return this;
        }

        public FieldBasedPageProviderConfiguration WithContent(string content)
        {
            Content = content;
            return this;
        }
    }
}