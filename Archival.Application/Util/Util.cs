using Archival.Core.Configuration;
using Archival.Core.Interfaces;
using Archival.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Archival.Application.Util
{
    public class Util<T> : IUtil<T> where T : IRecord
    {
        private readonly ILogManagerCustom _log;  
        private readonly ConversionConfiguration _config;
        public Util(IOptions<ConversionConfiguration> config, ILogManagerCustom log)
        {
            _config = config.Value;
            _log = log;
        }
        public string GetValidFileName(string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }
        public string GetBufferPath(T record, bool cleanUp = false, bool isPDF = false)
        {
            string dt = DateTime.Now.ToString("ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
            string directoryPath = _config.BufferFolder + record.conversionType + @"\"+ dt;
            CreateTempStorage(directoryPath);

            string fileName = "", temp="", recordLocalPath=""; 
            if (isPDF) {
                temp = GetValidFileName(Path.GetFileNameWithoutExtension(record.name));
                if (Path.GetExtension(record.name).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    if (record.convertedRecord == null || (record.convertedRecord != null && record.convertedRecord.dataID != record.dataID))
                        temp += "_Converted";
                }
                fileName = temp + ".pdf";
                recordLocalPath = Path.Combine(directoryPath, fileName);
                record.convertedPath = recordLocalPath;
            } 
            else if (cleanUp)
            {
                recordLocalPath = directoryPath;
            }
            else
            {
                fileName = GetValidFileName(record.name);
                recordLocalPath = Path.Combine(directoryPath, fileName);
                record.localFilePath = recordLocalPath;
            }

            return recordLocalPath;  
        }
        public string GetReportPath(T record) 
        {
            string dt = DateTime.Now.ToString("ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
            string directoryPath = _config.BufferFolder + record.conversionType + @"\" + dt;
            CreateTempStorage(directoryPath);

            string fileName = "", reportPath = "";
            if (record.conversionType == "MAIN_NAS") 
            { 
                fileName = "Record" + ".csv";
                reportPath = Path.Combine(directoryPath, fileName);
                record.recordReportPath = reportPath;
            }
            else if (record.conversionType == "DAILYCONVERSION" || record.conversionType == "MAIN_A")
            {
                fileName = "Report" + ".csv";
                reportPath = Path.Combine(directoryPath, fileName);
                record.recordReportPath = reportPath;
            }  

            return reportPath;
        }
        public string GetFolderReportPath(Summary summary)
        {
            string dt = DateTime.Now.ToString("ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
            string directoryPath = _config.BufferFolder + summary.conversionType + @"\" + dt;
            CreateTempStorage(directoryPath);

            string fileName = "", reportPath = "";
            if (summary.conversionType == "MAIN_NAS")
            { 
                fileName = "Folder" + ".csv"; 
                reportPath = Path.Combine(directoryPath, fileName);
                summary.folderReportPath = reportPath;
            } 

            return reportPath;
        }
        public void CreateTempStorage(string folderPath)
        {
            bool exists = Directory.Exists(folderPath);
            if (!exists)
                Directory.CreateDirectory(folderPath);
        }
        /// <summary>
        /// Detects if a given office document is protected by a password or not.
        /// Supported formats: Word, Excel and PowerPoint (both legacy and OpenXml).
        /// </summary>
        /// <param name="fileName">Path to an office document.</param>
        /// <returns>True if document is protected by a password, false otherwise.</returns>
        public bool IsPasswordProtected(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
                return IsPasswordProtected(stream);
        }
        /// <summary>
        /// Detects if a given office document is protected by a password or not.
        /// Supported formats: Word, Excel and PowerPoint (both legacy and OpenXml).
        /// </summary>
        /// <param name="stream">Office document stream.</param>
        /// <returns>True if document is protected by a password, false otherwise.</returns>
        private bool IsPasswordProtected(Stream stream)
        {
            // minimum file size for office file is 4k
            if (stream.Length < 4096)
                return false;

            // read file header
            stream.Seek(0, SeekOrigin.Begin);
            var compObjHeader = new byte[0x20];
            ReadFromStream(stream, compObjHeader);

            // check if we have plain zip file
            if (compObjHeader[0] == 'P' && compObjHeader[1] == 'K')
            {
                // this is a plain OpenXml document (not encrypted)
                return false;
            }

            // check compound object magic bytes
            if (compObjHeader[0] != 0xD0 || compObjHeader[1] != 0xCF)
            {
                // unknown document format
                return false;
            }

            int sectionSizePower = compObjHeader[0x1E];
            if (sectionSizePower < 8 || sectionSizePower > 16)
            {
                // invalid section size
                return false;
            }
            int sectionSize = 2 << (sectionSizePower - 1);

            const int defaultScanLength = 32768;
            long scanLength = Math.Min(defaultScanLength, stream.Length);

            // read header part for scan
            stream.Seek(0, SeekOrigin.Begin);
            var header = new byte[scanLength];
            ReadFromStream(stream, header);

            // check if we detected password protection
            if (ScanForPassword(stream, header, sectionSize))
                return true;

            // if not, try to scan footer as well

            // read footer part for scan
            stream.Seek(-scanLength, SeekOrigin.End);
            var footer = new byte[scanLength];
            ReadFromStream(stream, footer);

            // finally return the result
            return ScanForPassword(stream, footer, sectionSize);
        }
        static void ReadFromStream(Stream stream, byte[] buffer)
        {
            int bytesRead, count = buffer.Length;
            while (count > 0 && (bytesRead = stream.Read(buffer, 0, count)) > 0)
                count -= bytesRead;
            if (count > 0) throw new EndOfStreamException();
        }
        static bool ScanForPassword(Stream stream, byte[] buffer, int sectionSize)
        {
            const string afterNamePadding = "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";

            try
            {
                string bufferString = Encoding.ASCII.GetString(buffer, 0, buffer.Length);

                // try to detect password protection used in new OpenXml documents
                // by searching for "EncryptedPackage" or "EncryptedSummary" streams
                const string encryptedPackageName = "E\0n\0c\0r\0y\0p\0t\0e\0d\0P\0a\0c\0k\0a\0g\0e" + afterNamePadding;
                const string encryptedSummaryName = "E\0n\0c\0r\0y\0p\0t\0e\0d\0S\0u\0m\0m\0a\0r\0y" + afterNamePadding;
                if (bufferString.Contains(encryptedPackageName) ||
                    bufferString.Contains(encryptedSummaryName))
                    return true;

                // try to detect password protection for legacy Office documents
                const int coBaseOffset = 0x200;
                const int sectionIdOffset = 0x74;

                // check for Word header
                const string wordDocumentName = "W\0o\0r\0d\0D\0o\0c\0u\0m\0e\0n\0t" + afterNamePadding;
                int headerOffset = bufferString.IndexOf(wordDocumentName, StringComparison.InvariantCulture);
                int sectionId;
                if (headerOffset >= 0)
                {
                    sectionId = BitConverter.ToInt32(buffer, headerOffset + sectionIdOffset);
                    int sectionOffset = coBaseOffset + sectionId * sectionSize;
                    const int fibScanSize = 0x10;
                    if (sectionOffset < 0 || sectionOffset + fibScanSize > stream.Length)
                        return false; // invalid document
                    var fibHeader = new byte[fibScanSize];
                    stream.Seek(sectionOffset, SeekOrigin.Begin);
                    ReadFromStream(stream, fibHeader);
                    short properties = BitConverter.ToInt16(fibHeader, 0x0A);
                    // check for fEncrypted FIB bit
                    const short fEncryptedBit = 0x0100;
                    return (properties & fEncryptedBit) == fEncryptedBit;
                }

                // check for Excel header
                const string workbookName = "W\0o\0r\0k\0b\0o\0o\0k" + afterNamePadding;
                headerOffset = bufferString.IndexOf(workbookName, StringComparison.InvariantCulture);
                if (headerOffset >= 0)
                {
                    sectionId = BitConverter.ToInt32(buffer, headerOffset + sectionIdOffset);
                    int sectionOffset = coBaseOffset + sectionId * sectionSize;
                    const int streamScanSize = 0x100;
                    if (sectionOffset < 0 || sectionOffset + streamScanSize > stream.Length)
                        return false; // invalid document
                    var workbookStream = new byte[streamScanSize];
                    stream.Seek(sectionOffset, SeekOrigin.Begin);
                    ReadFromStream(stream, workbookStream);
                    short record = BitConverter.ToInt16(workbookStream, 0);
                    short recordSize = BitConverter.ToInt16(workbookStream, sizeof(short));
                    const short bofMagic = 0x0809;
                    const short eofMagic = 0x000A;
                    const short filePassMagic = 0x002F;
                    if (record != bofMagic)
                        return false; // invalid BOF
                                      // scan for FILEPASS record until the end of the buffer
                    int offset = sizeof(short) * 2 + recordSize;
                    int recordsLeft = 16; // simple infinite loop check just in case
                    do
                    {
                        record = BitConverter.ToInt16(workbookStream, offset);
                        if (record == filePassMagic)
                            return true;
                        recordSize = BitConverter.ToInt16(workbookStream, sizeof(short) + offset);
                        offset += sizeof(short) * 2 + recordSize;
                        recordsLeft--;
                    } while (record != eofMagic && recordsLeft > 0);
                }

                // check for PowerPoint user header
                const string currentUserName = "C\0u\0r\0r\0e\0n\0t\0 \0U\0s\0e\0r" + afterNamePadding;
                headerOffset = bufferString.IndexOf(currentUserName, StringComparison.InvariantCulture);
                if (headerOffset >= 0)
                {
                    sectionId = BitConverter.ToInt32(buffer, headerOffset + sectionIdOffset);
                    int sectionOffset = coBaseOffset + sectionId * sectionSize;
                    const int userAtomScanSize = 0x10;
                    if (sectionOffset < 0 || sectionOffset + userAtomScanSize > stream.Length)
                        return false; // invalid document
                    var userAtom = new byte[userAtomScanSize];
                    stream.Seek(sectionOffset, SeekOrigin.Begin);
                    ReadFromStream(stream, userAtom);
                    const int headerTokenOffset = 0x0C;
                    uint headerToken = BitConverter.ToUInt32(userAtom, headerTokenOffset);
                    // check for headerToken
                    const uint encryptedToken = 0xF3D1C4DF;
                    return headerToken == encryptedToken;
                }
            }
            catch (Exception ex)
            {
                // BitConverter exceptions may be related to document format problems
                // so we just treat them as "password not detected" result
                if (ex is ArgumentException)
                    return false;
                // respect all the rest exceptions
                throw;
            }

            return false;
        }
        public bool IsInvalidFormat(string filePath)
        {
            string[] validExtension = { ".msg",".zip",".doc",".docm",".txt",".docx", ".pdf", ".906", ".907", ".dwg",".dxf", ".dwf",".dwfx",".tg4",".prt",".cgm",".g3",".g4",".rnl",".cmi",".mi",".000",".hgl",".plt",".hpgl",".cit", ".rle", ".icd", ".dgn", ".psd"
                    ,".cal",".ps",".eps",".emf",".img",".gif",".hdp",".afp",".ica",".gp4",".mil",".cg4",".iso",".jpg",".jpeg",".jp2",".jpm",".pcx",".png",".tif",".tiff",".bmp",".wdp",".wmf",".numbers",".key",".pages",".csv",".xml"
                    ,".eml",".xls",".xlt",".xlsx",".xlsm",".xlsb",".xltx",".xltm",".xlw",".pps",".ppt",".pptx",".pptm",".ppsx",".ppsm",".potx",".potm",".vsd",".vdx",".vsx",".vtx",".vsdx",".csf",".and",".xps",".rnd",".cdr",".shw",".wp"
                    ,".wp5",".wp6",".wpd",".wpf",".wpg",".wpg2",".dbf",".fax",".flw",".fmt",".fwk",".prz",".hwp",".html",".htm",".pcd",".wk1",".wk3",".pic",".pict",".pct",".drw",".accdb",".accde",".accda",".acs",".wmf",".mpp",".wdb",".wri"
                    ,".p2",".pp7",".wb1",".wb2",".wq1",".rtf",".tga",".vst",".manu",".vw",".xbm",".ws" };
            bool isInvalid = true;
            string fileExtension = Path.GetExtension(filePath);

            if (validExtension.Contains(fileExtension))
            {
                isInvalid = false;
            } 
            return isInvalid;
        }
        public void EnsureFilesAreCleanedUp(string dirPath)
        {
            try
            {
                CleanupFiles(dirPath);
            }
            catch (Exception ex)
            {
                _log.debug("Cleanup task failure" + ex.Message + ex.StackTrace);
            }
        }
        private bool CleanupFiles(string dirPath)
        {
            bool sourceFileIsDeleted = false;
            try
            {
                if (Directory.Exists(System.IO.Path.GetFullPath(dirPath)))
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(dirPath);
                    int total = 0;
                    foreach (FileInfo file in di.GetFiles())
                    {
                        total++;
                        file.Delete();
                    }
                    _log.debug("Total files deleted from local : " + total);
                    Directory.Delete(dirPath);
                }
                sourceFileIsDeleted = true;
            }
            catch (Exception e)
            {
                _log.debug("Deletion failure of " + dirPath + ": " + e.Message + e.StackTrace);
            }
            return sourceFileIsDeleted;
        }
    }
}
