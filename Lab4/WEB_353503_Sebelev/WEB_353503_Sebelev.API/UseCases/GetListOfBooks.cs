using MediatR;
using Microsoft.EntityFrameworkCore;
using WEB_353503_Sebelev.API.Data;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.API.UseCases;

public sealed record GetListOfBooks(
    string? CategoryNormalizedName,
    int PageNo = 1,
    int PageSize = 3) : IRequest<ResponseData<ListModel<Book>>>;

public class GetListOfBooksHandler(AppDbContext db) :
    IRequestHandler<GetListOfBooks, ResponseData<ListModel<Book>>>
{
    private readonly int _maxPageSize = 20;
    
    public async Task<ResponseData<ListModel<Book>>> Handle(GetListOfBooks request, CancellationToken cancellationToken)
    {
        var pageSize = request.PageSize > _maxPageSize ? _maxPageSize : request.PageSize;

        var query = db.Books
            .Include(b => b.Category)
            .Where(b => request.CategoryNormalizedName == null ||
                        b.Category.NormalizedName.Equals(request.CategoryNormalizedName));
        
       var totalItems = await query.CountAsync(cancellationToken); 
        
       var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

       var currentPage = request.PageNo;

       if (currentPage > totalPages && totalPages > 0)
       {
           currentPage = totalPages;
       }

       if (currentPage <= 0)
       {
           currentPage = 1;
       }
       
       var pageItems = await query
           .Skip((currentPage - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync(cancellationToken);

       var listModel = new ListModel<Book>
       {
           Items = pageItems,
           CurrentPage = currentPage,
           TotalPages = totalPages
       };
       
       return ResponseData<ListModel<Book>>.Success(listModel);
    }
}