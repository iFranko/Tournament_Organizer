using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

// Ref Code https://www.youtube.com/watch?reload=9&v=O57nsLyZubc&ab_channel=CodeS

namespace Tournament_Organizer.Models
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrenPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager()
        {

        }
        public Pager(int totalItems, int page, int pageSize = 10) 
        {
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int currentPage = page;

            int startPage = currentPage - 5;
            int endPage = currentPage + 4;

            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }

            if (endPage > totalPages) 
            {
                endPage = totalPages;
                if (endPage > 10) 
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrenPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;

        }


    }

    

}
