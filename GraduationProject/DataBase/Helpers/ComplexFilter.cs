namespace GraduationProject.DataBase.Helpers
{
    public class ComplexFilter
    {
        public string? SearchQuery { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? Sort { get; set; } //key
        public string? Order { get; set; }//asc desc
    }
}
