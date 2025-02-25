
using CbClient.Services;
using Couchbase.Lite.Query;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CbClient.Pages.Sites;

public class IndexModel : PageModel
{
    public string SiteId { get; set; } = string.Empty;

    public CbProvider Cb { get; private set; }
    public CbProvider1 Cb1 { get; private set; }

    public List<RuleViewModel> Rules { get; set; } = [];

    public List<RuleViewModel> Rules1 { get; set; } = [];

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(CbProvider cb, CbProvider1 cb1, ILogger<IndexModel> logger)
    {
        _logger = logger;
        Cb = cb;
        Cb1 = cb1;
    }

    public void OnGet(string siteId)
    {
        SiteId = siteId;

        var rulesCollection = Cb.Db.GetCollection("promo_rule");
        var rulesCollection1 = Cb1.Db.GetCollection("promo_rule");

        using var query = QueryBuilder.Select(
            //SelectResult.All()
            SelectResult.Expression(Meta.ID),
            SelectResult.Property("expression")
            )
            .From(DataSource.Collection(rulesCollection));

        using var query1 = QueryBuilder.Select(
            //SelectResult.All()
            SelectResult.Expression(Meta.ID),
            SelectResult.Property("expression")
            )
            .From(DataSource.Collection(rulesCollection1));

        var data = query.Execute().ToList();
        var data1 = query1.Execute().ToList();

        foreach (var item in data)
        {
            var id = item.GetString("id");
            var expression = item.GetString("expression");
            
            Rules.Add(new()
            {
                Id = id,
                Expression = expression
            });
        }

        foreach (var item in data1)
        {
            var id = item.GetString("id");
            var expression = item.GetString("expression");
            
            Rules1.Add(new()
            {
                Id = id,
                Expression = expression
            });
        }
    }
}
