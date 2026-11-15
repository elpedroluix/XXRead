using SQLite;

namespace XXRead.Storage.Entities
{
    [Table("Author")]
    internal class Author
    {
        /// <summary>
        /// Key : the URL is set by every source, the Id is not (HDS has none).
        /// </summary>
        [PrimaryKey]
        public string Url { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
    }
}
