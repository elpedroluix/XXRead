using SQLite;

namespace XXRead.Storage.Entities
{
    [Table("Author")]
    internal class Author
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
