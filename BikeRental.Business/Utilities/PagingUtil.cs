using BikeRental.Business.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public static class PagingUtil<TEntity> where TEntity : class
    {
        public static List<TEntity> Paging(List<TEntity> source, int pageNum)
        {
            int groupNum = GlobalConstants.GROUP_ITEM_NUM;
            if (source == null)
            {
                return null;
            }
            int availablePageNum = source.Count / groupNum;

            if (pageNum <= 0)
            {
                pageNum = 1;
            }
            else if (pageNum > availablePageNum)
            {
                pageNum = availablePageNum;
            }

            return source.GetRange((groupNum * pageNum) - groupNum, groupNum);
        }
    }
}
