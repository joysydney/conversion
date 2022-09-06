using Archival.Core.Interfaces;
using Archival.Core.Model;
using Archival.Core.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Application.Services
{
    public class FilterRecord<T> : IFilterRecord<T> where T : IFilter
    {
        private readonly IRepository<T> _repository;
        public FilterRecord(IRepository<T> repository)
        {  
            _repository = repository;
        } 
        public async Task<FilterResponse<T>> Filter(List<T> allRecord)
        {
            var result = new List<T>();
            foreach (T record in allRecord)
            {
                var fullPath = await _repository.GetRecordFullPath(record.dataID);
                string location = "";
                for (int i = 0; i < fullPath.Count(); i++)
                {
                    location += fullPath[i].name + "/";
                    if (i == fullPath.Count() - 1) record.createdBy = fullPath[i].createdBy;
                }
                record.recordLocation = location;

                var convID_FirstCol = await _repository.GetConverted_FirstCol(record.dataID);
                if (convID_FirstCol.Count() != 0)
                {
                    foreach (var value in convID_FirstCol)
                    {
                        string originalExt = Path.GetExtension(record.name);
                        string crossExt = Path.GetExtension(value.name);
                        bool isEmailWithAtt = await _repository.isEmailWithAttachment(value.dataID);

                        if (isEmailWithAtt)
                        {
                            record.isEmail = true;
                            record.parentID = await _repository.GetCompoundLocation(value.dataID);
                        }
                        else if (originalExt != ".pdf" && crossExt != ".pdf")
                        {
                            record.noPDFId = value.dataID;
                        }
                        else if ((originalExt != ".pdf" && crossExt == ".pdf"))
                        {
                            record.convertedRecord = new ConvertedRecord();
                            record.convertedRecord.dataID = value.dataID;
                        }
                        else if ((originalExt == ".pdf" && crossExt != ".pdf"))
                        {
                            record.convertedRecord = new ConvertedRecord();
                            record.convertedRecord.dataID = record.dataID;
                        }
                        else if (originalExt == ".pdf" && crossExt == ".pdf")
                        {
                            record.convertedRecord = new ConvertedRecord();

                            int originalCheck = value.name.IndexOf("_Converted");
                            int crossCheck = value.name.IndexOf("_Converted");
                            if (originalCheck != -1)
                            { 
                                record.convertedRecord.dataID = record.dataID;
                            }
                            else if (crossCheck != -1)
                            {
                                record.convertedRecord.dataID = value.dataID;
                            }

                        }
                    }
                }

                result.Add(record);
            }
            return new FilterResponse<T>(result);
        }
    }
}
