
using System.Collections.Generic;
using System.Linq;
namespace kalamon_University.DTOs.Common
{
    //åĞÇ ÇáßáÇÓ íõÓÊÎÏã ááÚãáíÇÊ ÇáÊí áÇ ÊõÑÌÚ ÈíÇäÇÊ (ãËá: ÍĞİ ßæÑÓ¡ ÊÃßíÏ ÊÓÌíá¡ ..).
    public class ServiceResult
    {   //íÍÏÏ ãÇ ÅĞÇ ßÇäÊ ÇáÚãáíÉ äÇÌÍÉ (true) Ãã İÔáÊ (false). ÇáŞíãÉ ÇáÇİÊÑÇÖíÉ: İÔá
        public bool Success { get; protected set; } = false; // Default to false
                                                             //ŞÇÆãÉ ÈÇáÃÎØÇÁ ÇáÊí ÍÕáÊ ÃËäÇÁ ÊäİíĞ ÇáÚãáíÉ. Êßæä İÇÑÛÉ ÅĞÇ äÌÍÊ ÇáÚãáíÉ
        public List<string> Errors { get; protected set; } = new List<string>();
        //ÑÓÇáÉ ÚÇãÉ ÊæÖÍ äÊíÌÉ ÇáÚãáíÉ (ãËáÇğ: "Êã ÇáÍĞİ ÈäÌÇÍ")
        public string Message { get; protected set; } // General message
                                                      //ÊõÑÌÚ äÊíÌÉ äÇÌÍÉ ãÚ ÑÓÇáÉ
        public static ServiceResult Succeeded(string message = "Operation successful.")
        {
            return new ServiceResult { Success = true, Message = message };
        }
        //ÊõÑÌÚ äÊíÌÉ İÇÔáÉ¡ æÊÃÎĞ ÓáÓáÉ ãä ÇáÃÎØÇÁ ßãÕİæİÉ
        public static ServiceResult Failed(params string[] errors)
        {
            return new ServiceResult { Success = false, Errors = errors.ToList() };
        }
        //äİÓ ÇáÓÇÈŞÉ¡ áßä ÊŞÈá List Ãæ IEnumerable ÈÏá ãÕİæİÉ
        public static ServiceResult Failed(IEnumerable<string> errors)
        {
            return new ServiceResult { Success = false, Errors = errors.ToList() };
        }
        
        public void AddError(string error)
        {
            if (Errors == null) Errors = new List<string>();
            Errors.Add(error);
            Success = false; // Ãí ÎØÃ íÚäí Ãä ÇáÚãáíÉ ÇáßáíÉ áã ÊäÌÍ ÈÇáßÇãá
        }
        public void AddErrors(IEnumerable<string> errors)
        {
            if (Errors == null) Errors = new List<string>();
            Errors.AddRange(errors);
            Success = false;
        }
    }
    
}