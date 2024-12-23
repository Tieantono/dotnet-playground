using CbClient.Services;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CbClient.Pages;

public class CreateModel(CbProvider cb) : PageModel
{
    public CbProvider Cb { get; private set; } = cb;

    public List<RuleViewModel> Rules { get; set; } = [];

    [BindProperty]
    public CreateRuleFormModel Form { get; set; } = new();

    public void OnGet()
    {
        var rulesCollection = Cb.Db.GetCollection("rules", "promotions");

        using var query = QueryBuilder.Select(
            //SelectResult.All()
            SelectResult.Expression(Meta.ID),
            SelectResult.Property("expression")
            )
            .From(DataSource.Collection(rulesCollection));

        var data = query.Execute();

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
    }

    public void OnPost()
    {
        var rulesCollection = Cb.Db.GetCollection("rules", "promotions");

        using var mutableDoc = new MutableDocument(Guid.NewGuid().ToString());

        mutableDoc.SetString("expression", Form.Expression);

        rulesCollection.Save(mutableDoc);

        RedirectToPage();
    }
}

public class RuleViewModel
{
    public string Id { get; set; } = string.Empty;

    public string Expression { get; set; } = string.Empty;
}

public class CreateRuleFormModel
{
    public string Expression { get; set; } = string.Empty;
}