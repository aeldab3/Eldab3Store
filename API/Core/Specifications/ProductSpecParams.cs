using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductSpecParams
    {
        public const int MaxPageSize = 50;
        public int pageIndex { get; set; } = 1;

        public int _pageSize = 6;
        public int pageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public string? Sort {  get; set; }

        private string? _search {  get; set; }
        public string? Search
        {
            get => _search;
            set => _search = value?.ToLower();
        }

    }
}
