using HappyKids.Helper;

namespace HappyKids.Models.DataTranferObjects
{
    public class PaginationHeader
    {
        public string PreviousPageLink { get; set; }
        public string NextPageLink { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
