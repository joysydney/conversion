using Archival.Core.Interfaces;
using Archival.Core.Model;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NASConversion : INASConversion
    {
        private readonly ICSAccess<NAS> _csAccess;
        private readonly IRepository<NASItem> _repository;
        private readonly IFilterRecord<NASItem> _filterRecord;
        private readonly IProcessRecords<NAS> _processRecords;
        private readonly IMapper _mapper;
        public NASConversion(ICSAccess<NAS> csAccess, IRepository<NASItem> repository, IFilterRecord<NASItem> filterRecord, IMapper mapper, IProcessRecords<NAS> processRecords)
        {
            _csAccess = csAccess;
            _repository = repository;
            _filterRecord = filterRecord;
            _processRecords = processRecords;
            _mapper = mapper;
        }

        public async Task Execute()
        { 
            List<NASItem> allRecord = await _repository.GetRecords(); 
            var result = await _filterRecord.Filter(allRecord);
            NASItem item = new NASItem();

            foreach(var record in result.Response)
            {
                if (record.noPDFId > 0)
                {
                    var res = await _csAccess.GetNode(record.noPDFId);
                    if (res.Success)
                    {
                        item.parentID = res.Response.data.parentID;
                        item.dataID = res.Response.data.id;
                        item.name = res.Response.data.name;
                        result.Response.Add(item);
                    }
                }
            }

            if (result.Response.Count() > 0)
            {
                var nasResult = _mapper.Map<List<NASItem>, List<NAS>>(result.Response);
                await _processRecords.Execute(nasResult);
            }
        }
    }
}
