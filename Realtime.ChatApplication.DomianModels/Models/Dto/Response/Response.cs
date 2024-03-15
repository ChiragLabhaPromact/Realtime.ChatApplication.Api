using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realtime.ChatApplication.DomianModels.Models.Dto.Response
{
    public class Response
    {
        public bool IsSuccess { get; }
        public object Data { get; }

        public Response(object data)
        {
            IsSuccess = true;
            Data = data;
        }

        public Response(string errorMessage)
        {
            IsSuccess = false;
            Data = errorMessage;
        }
    }
}
