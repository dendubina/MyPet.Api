using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class PagedRequestParameters
    {
        private const int MaxPageSize = 50;
        private int pageSize = 15;

        public int PageNumber { get; set; } = 1;

		public int PageSize
		{
			get => pageSize;
            set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
	}
}
