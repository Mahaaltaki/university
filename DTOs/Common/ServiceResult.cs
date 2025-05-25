namespace kalamon_University.DTOs.Common
using System.Collections.Generic;
using System.Linq;
//��� ������ ������� �������� ���� �� ����� ������ (���: ��� ���ӡ ����� ����� ..).
public class ServiceResult
{   //���� �� ��� ���� ������� ����� (true) �� ���� (false). ������ ����������: ���
    public bool Success { get; protected set; } = false; // Default to false
    //����� �������� ���� ���� ����� ����� �������. ���� ����� ��� ���� �������
    public List<string> Errors { get; protected set; } = new List<string>();
    //����� ���� ���� ����� ������� (�����: "�� ����� �����")
    public string Message { get; protected set; } // General message
    //����� ����� ����� �� �����
    public static ServiceResult Succeeded(string message = "Operation successful.")
    {
        return new ServiceResult { Success = true, Message = message };
    }
    //����� ����� ����ɡ ����� ����� �� ������� �������
    public static ServiceResult Failed(params string[] errors)
    {
        return new ServiceResult { Success = false, Errors = errors.ToList() };
    }
    //��� ������ɡ ��� ���� List �� IEnumerable ��� ������
    public static ServiceResult Failed(IEnumerable<string> errors)
    {
        return new ServiceResult { Success = false, Errors = errors.ToList() };
    }
}
//��� ���� Generic ���� ��� ServiceResult� ��� ������� ����� ���� ������� ������ (�����: ��� ����� ���ȡ ���� ���� ...)
public class ServiceResult<TData> : ServiceResult
{  //���� �������� ���� �� ������� �� ������� (���� ����ɡ ��� ���)
    public TData? Data { get; private set; }
    //���� ����� ����� ������ ��� ��������
    public static ServiceResult<TData> Succeeded(TData data, string message = "Data retrieved successfully.")
    {
        return new ServiceResult<TData> { Success = true, Data = data, Message = message };
    }
    //��� ����� Failed �� ������ ������� ����� ���� ServiceResult<TData>
    public new static ServiceResult<TData> Failed(params string[] errors)
    {
        return new ServiceResult<TData> { Success = false, Errors = errors.ToList() };
    }
    public new static ServiceResult<TData> Failed(IEnumerable<string> errors)
    {
        return new ServiceResult<TData> { Success = false, Errors = errors.ToList() };
    }
}