namespace InsonusK.Shared.Model.Template;

public interface IFetchRequest
{

    // Номер страницы, по умолчанию 1
    int Page { get; }

    // Количество элементов на странице, по умолчанию 10
    int PageSize { get; }
}