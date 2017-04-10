namespace HappyKids.Models.DataTranferObjects
{
    public class StudentResourceParameters
    {
        public static readonly int maxPageSize = 10;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string Name { get; set; }
    }

}
