using Archival.Core.Interfaces;
using Archival.Core.Model;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Application.Services
{
    public class DailyConversion : IDailyConversion
    {
        private readonly ILogManagerCustom _log;
        private readonly IProcessRecords<Daily> _processRecords;
        private readonly IFilterRecord<DailyItem> _filterRecord;
        private readonly IRepository<DailyItem> _repository;
        private readonly IMapper _mapper;
        private readonly ICSAccess<NAS> _csAccess;//remove this line later
        public DailyConversion(ICSAccess<NAS> csAccess, IRepository<DailyItem> repository, ILogManagerCustom log, IMapper mapper, IFilterRecord<DailyItem> filterRecord, IProcessRecords<Daily> processRecords)
        {
            _csAccess = csAccess;
            _repository = repository;
            _log = log;
            _filterRecord = filterRecord;
            _processRecords = processRecords;
            _mapper = mapper;
        }
        public async Task Execute()
        { 
            _log.info("Daily Conversion Started");
            List<DailyItem> allRecord = new List<DailyItem>();
            allRecord = await _repository.GetDailyRecords();
            List<DailyItem> recordUnderCompound = new List<DailyItem>();
            recordUnderCompound = await _repository.GetDailyRecordsUnderCompounds(); 
            var result = await _filterRecord.Filter(allRecord);
            var result_recordUnderCompound = await _filterRecord.Filter(recordUnderCompound);
            
            if(result.Success || result_recordUnderCompound.Success)
            {
                result.Response.AddRange(result_recordUnderCompound.Response);
            }

            if (result.Response.Count() > 0)
            {
                var dailyResult = _mapper.Map<List<DailyItem>, List<Daily>>(result.Response);
                await _processRecords.Execute(dailyResult);
            }
        }
    }
}
