// ServiceResultGeneric.cs
namespace kalamon_University.DTOs.Common
{
    public class ServiceResult<TData> : ServiceResult
    {
        public TData? Data { get; private set; }

        public static ServiceResult<TData> Succeeded(TData data, string message = "Data retrieved successfully.")
            => new ServiceResult<TData> { Success = true, Data = data, Message = message };

        public new static ServiceResult<TData> Failed(params string[] errors)
            => new ServiceResult<TData> { Success = false, Errors = errors.ToList() };

        public new static ServiceResult<TData> Failed(IEnumerable<string> errors)
            => new ServiceResult<TData> { Success = false, Errors = errors.ToList() };
    }
}



