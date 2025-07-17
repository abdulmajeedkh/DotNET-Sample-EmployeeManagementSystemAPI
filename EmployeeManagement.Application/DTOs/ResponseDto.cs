using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs
{
    public class ResponseDto<T>
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure { get; private set; }
        public T? Value { get; private set; }
        public string? Errors { get; private set; }

        private ResponseDto() { }

        public static ResponseDto<T> Success(T value) => new ResponseDto<T>() { IsSuccess = true, IsFailure = false, Value = value };
        public static ResponseDto<T> Failure(string error) => new ResponseDto<T>() { IsSuccess = false, IsFailure = true, Errors = error };

    }
}
