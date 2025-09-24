using System.Collections.Generic;

namespace Domain.Model
{
  public class Response<T>
  {
    public int? Status { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<Errors> Error { get; set; }
  }

  public class Errors
  {
    public string InternalCode { get; set; }
    public string Message { get; set; }
  }
}