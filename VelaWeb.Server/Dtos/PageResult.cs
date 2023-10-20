namespace VelaWeb.Server.Dtos
{
    public class PageResult<T>
    {
        public int Total { get; set; }
        public T[] Datas { get; set; }
    }
}
