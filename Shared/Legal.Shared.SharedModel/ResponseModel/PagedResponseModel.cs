using Legal.Service.Infrastructure.Interface;

namespace Legal.Shared.SharedModel.ResponseModel;

public class PagedResponseModel<T> : IResponseModel where T : IResponseModel
{
    public PagedResponseModel()
    {
    }

    public IEnumerable<T> Data { get; private set; } = Enumerable.Empty<T>();

    public int Count { get; private set; }

    public int PageNumber { get; private set; }

    public int TotalPage { get; set; }

    public int PageSize { get; private set; }

    public void Add(IEnumerable<T> items)
    {
        var list = Data.ToList();
        list.AddRange(items);
        Data = list;
        Count = list.Count;
        PageNumber = 1;
        PageSize = list.Count;
        TotalPage = 1;
    }

    public void Add(IEnumerable<T> items, int count = -1, int pageNumber = 1, int pageSize = 10)
    {
        var list = Data.ToList();
        list.AddRange(items);

        if (list.Count > count)
        {
            Count = list.Count;
        }
        else
        {
            Count = count;
        }

        Data = list;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Count) / Convert.ToDouble(pageSize)));
    }
}