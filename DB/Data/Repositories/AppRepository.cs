using DB.Data;
using Archival.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archival.Core.Model;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DB.Data.Repositories
{
    public class AppRepository<T> : IRepository<T>
    {
        private readonly DapperContext _context;
        private readonly OracleConnection _connection;
        //private readonly SqlConnection _connection;
        public AppRepository(DapperContext context)
        {
            _context = context;
        } 

        public async Task<int> AddEkrisRelated(long id, long convertedID)
        { 
            string sql = $"INSERT INTO EKRIS_RELATED(ITEM1,ITEM2,REMARKS) VALUES({id},{convertedID}, 'ekris2Related')";
          
            using (var connection = _context.CreateConnection())
            {
                var data = await connection.ExecuteAsync(sql);

                return data;
            }
        }

        public async Task<long> GetCompoundLocation(long id)
        {
            string sql = $"SELECT * FROM EKRIS_RELATED ER INNER JOIN DTREECORE DC ON DC.DataID = ER. ITEM1 WHERE {id} IN(ITEM1,ITEM2) AND DC.SUBTYPE=136";

            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<long>(sql, new { id });

                return data.First();
            }
        }

        public async Task<List<ConvertedRecord>> GetConverted_FirstCol(long DataID)
        {
            string sql = $"SELECT * FROM EKRIS_RELATED er INNER JOIN DTREE dc on er.item1 = dc.dataid WHERE er.item2 = {DataID} and deleted=0";
            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<ConvertedRecord>(sql);
                return data.ToList();
            }
        }

        public Task<int> GetConverted_SecCol(T nas)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DailyItem>> GetDailyRecords()
        {
            string sql = @"SELECT *
                        FROM Dtree dt
                        WHERE TRUNC( SYSDATE ) = TRUNC(dt.ModifyDate)
                        AND (dt.Subtype =144 OR dt.Subtype=749 )	 
                        AND dt.Deleted=0 and parentid=273282";
            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<DailyItem>(sql);
                return data.ToList();
            } 
        }
        public async Task<List<DailyItem>> GetDailyRecordsUnderCompounds()
        {
            string sql = @"SELECT *
                        FROM Dtree dt
                        WHERE ParentID IN (
                            SELECT DataID
                            FROM Dtree dt
                            WHERE TRUNC( SYSDATE ) = TRUNC(dt.ModifyDate)
                            AND (dt.Subtype =136)	 
                            AND dt.Deleted=0
                        )";
            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<DailyItem>(sql);

                return data.ToList();
            } 
        }

        public Task<int> GetEkrisRelated(T nas)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PDFItem>> GetPDFRecords()
        {
            string sql = @"select DT.DATAID, DT.USERID, RM.QDATE, RM.RSI, DT.PARENTID, C.Subtype as parent_subtype, DT.NAME from RM_DISPRESULTS RM
                            inner join dtree DT on DT.DATAID = RM.NODEID
                            inner join dtree C on DT.PARENTID = C.DATAID
                            where trunc(QDATE) = (select max(trunc(QDATE)) from RM_DISPRESULTS) AND RSI LIKE '%A'";
            
            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<PDFItem>(sql);

                return data.ToList();
            }
        }

        public Task<int> GetRecord(T nas)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FullPath>> GetRecordFullPath(long id)
        {
            string sql = $"with t(x, dataid, name, parentid, createdby) as(select 1 as x, dataid, name, parentid, createdby from dtreecore where dataid= {id} union all select x+1, d.dataid, d.name, d.parentid, d.createdby from t inner join dtreecore d on d.dataid = t.parentid) select t.x, t.dataid, t.name, t.parentid, k.name as createdby from t inner join kuaf k on k.id = t.createdby ORDER BY x desc";
             
            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<FullPath>(sql, new { id });

                return data.ToList();
            }
        }

        public Task<List<(string, string)>> GetRecordLocation(T nas)
        {
            throw new NotImplementedException();
        }

        public async Task<List<T>> GetRecords()
        { 
            string sql = @"SELECT DT.DATAID, DT.USERID, RM.QDATE, RM.RSI, DT.PARENTID, DT.Subtype as conversionType, C.Subtype as parentSubtype, DT.NAME FROM RM_DISPRESULTS RM
                            INNER JOIN DTREE DT ON DT.DATAID = RM.NODEID
                            INNER JOIN DTREE C ON  C.DATAID = DT.PARENTID 
                            WHERE TRUNC(QDATE) = (SELECT MAX(TRUNC(QDATE)) FROM RM_DISPRESULTS) AND RSI LIKE '%NAS'";
            
            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<T>(sql);
                return data.ToList();
            }
        }

        public async Task<long> GetUsername(long id)
        {
            string sql = $"SELECT er.* from EKRIS_RELATED er, DTREECORE dtc where er.ITEM1 = {id} and er.ITEM2 = dtc.DATAID and dtc.SUBTYPE = 136";

            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<long>(sql, new { id });

                return data.First();
            }
        }

        public async Task<bool> isEmailWithAttachment(long id)
        {
            string sql = $"SELECT er.* from EKRIS_RELATED er, DTREECORE dtc where er.ITEM1 = {id} and er.ITEM2 = dtc.DATAID and dtc.SUBTYPE = 136";
          
            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<bool>(sql, new { id });

                return data.FirstOrDefault();
            } 
        }
    }
}
