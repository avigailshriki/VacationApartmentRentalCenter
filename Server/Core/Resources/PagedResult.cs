using System;
using System.Collections.Generic;

namespace Core.Resources
{
    // עטיפה כללית לתוצאות עם דפדוף (pagination) - משמשת לרשימות שעלולות לגדול (כרגע: נכסים)
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    }
}
