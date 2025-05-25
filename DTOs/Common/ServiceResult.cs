namespace kalamon_University.DTOs.Common
using System.Collections.Generic;
using System.Linq;
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
}
//åĞÇ ßáÇÓ Generic ãÈäí Úáì ServiceResult¡ áßä íõÓÊÎÏã ÚäÏãÇ ÊÑÌÚ ÇáÚãáíÉ ÈíÇäÇÊ (ãËáÇğ: ÌáÈ ŞÇÆãÉ ØáÇÈ¡ ßæÑÓ ãÚíä¡ ...)
public class ServiceResult<TData> : ServiceResult
{  //ÊãËá ÇáÈíÇäÇÊ ÇáÊí Êã ÅÑÌÇÚåÇ ãä ÇáÚãáíÉ (ßÇÆä¡ ŞÇÆãÉ¡ ÑŞã¡ ÅáÎ)
    public TData? Data { get; private set; }
    //ÊÑÌÚ äÊíÌÉ äÇÌÍÉ æÊÍÊæí Úáì ÇáÈíÇäÇÊ
    public static ServiceResult<TData> Succeeded(TData data, string message = "Data retrieved successfully.")
    {
        return new ServiceResult<TData> { Success = true, Data = data, Message = message };
    }
    //äİÓ ØÑíŞÉ Failed İí ÇáßáÇÓ ÇáÃÓÇÓí¡ áßäåÇ ÊÑÌÚ ServiceResult<TData>
    public new static ServiceResult<TData> Failed(params string[] errors)
    {
        return new ServiceResult<TData> { Success = false, Errors = errors.ToList() };
    }
    public new static ServiceResult<TData> Failed(IEnumerable<string> errors)
    {
        return new ServiceResult<TData> { Success = false, Errors = errors.ToList() };
    }
}