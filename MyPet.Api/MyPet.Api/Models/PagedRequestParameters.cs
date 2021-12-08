using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class PagedRequestParameters
    {
		private int _pageSize = 20;
		const int maxPageSize = 50;
		public int PageNumber { get; set; } = 1;

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
	}
}
