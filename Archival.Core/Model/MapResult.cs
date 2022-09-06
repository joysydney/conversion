using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Archival.Core.Model
{
    public class MapResult : Profile
    {
        public MapResult()
        {
            CreateMap<DailyItem, Daily>(); 
            CreateMap<NASItem, NAS>(); 
            CreateMap<PDFItem, PDF>(); 
        }
    }
}
