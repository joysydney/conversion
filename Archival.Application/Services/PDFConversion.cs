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
    public class PDFConversion : IPDFConversion
    {
        private readonly ICSAccess<NAS> _csAccess;
        private readonly ILogManagerCustom _log;
        private readonly IRepository<PDFItem> _repository;
        private readonly IFilterRecord<PDFItem> _filterRecord;
        private readonly IProcessRecords<PDF> _processRecords;
        private readonly IMapper _mapper;
        public PDFConversion(ICSAccess<NAS> csAccess, IRepository<PDFItem> repository, ILogManagerCustom log, IFilterRecord<PDFItem> filterRecord, IMapper mapper, IProcessRecords<PDF> processRecords)
        {
            _csAccess = csAccess;
            _repository = repository;
            _filterRecord = filterRecord;
            _processRecords = processRecords;
            _mapper = mapper;
            _log = log;
        }
        public async Task Execute()
        {
            _log.info("PDF Conversion Started");
            List<PDFItem> allRecord = new List<PDFItem>();
            allRecord = await _repository.GetPDFRecords();
            var result = await _filterRecord.Filter(allRecord);
            PDFItem item = new PDFItem();

            foreach (var record in result.Response)
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
                var pdfResult = _mapper.Map<List<PDFItem>, List<PDF>>(result.Response);
                await _processRecords.Execute(pdfResult);
            }
        }
    }
}
