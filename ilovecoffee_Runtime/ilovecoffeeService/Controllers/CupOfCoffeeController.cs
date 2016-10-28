using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using ilovecoffeeService.DataObjects;
using ilovecoffeeService.Models;
using System.Security.Principal;
using System.Security.Claims;

namespace ilovecoffeeService.Controllers
{
    [Authorize]
    public class CupOfCoffeeController : TableController<CupOfCoffee>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            ilovecoffeeContext context = new ilovecoffeeContext();
            DomainManager = new EntityDomainManager<CupOfCoffee>(context, Request);
        }


        string GetSid(IPrincipal user)
        {
            ClaimsPrincipal claimsUser = (ClaimsPrincipal)user;

            string provider = claimsUser.FindFirst("http://schemas.microsoft.com/identity/claims/identityprovider").Value;
            string sid = claimsUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            // The above assumes WEBSITE_AUTH_HIDE_DEPRECATED_SID is true. Otherwise, use the stable_sid claim:
            // string sid = claimsUser.FindFirst("stable_sid").Value; 

            return $"{provider}|{sid}";
        }

        // GET tables/CupOfCoffee
        public IQueryable<CupOfCoffee> GetAllCupOfCoffee()
        {

            var sid = GetSid(User);

            

            return Query().Where(c => c.UserId == sid); 
        }

        // GET tables/CupOfCoffee/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<CupOfCoffee> GetCupOfCoffee(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/CupOfCoffee/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<CupOfCoffee> PatchCupOfCoffee(string id, Delta<CupOfCoffee> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/CupOfCoffee
        public async Task<IHttpActionResult> PostCupOfCoffee(CupOfCoffee item)
        {
            var sid = GetSid(User);
            item.UserId = sid;

            CupOfCoffee current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/CupOfCoffee/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCupOfCoffee(string id)
        {
             return DeleteAsync(id);
        }
    }
}
