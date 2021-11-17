using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class AdPagedRequestParameters
    {
		private int _pageSize = 10;
		const int maxPageSize = 20;
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
