using SQLite;

namespace XXRead.Storage.Entities
{
    internal class Category
    {
        public string Title { get; set; }
        public string Source { get; set; }
        [PrimaryKey]
        public string Url { get; set; }
    }
}
