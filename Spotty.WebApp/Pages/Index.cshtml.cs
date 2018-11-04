using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Spotty.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private Authorization Authorization { get; }

        public IndexModel()
        {
            Authorization = new Authorization(
                "",
                "",
                WebUtility.UrlEncode("http://localhost:56081/index/authorizationcallback"));
        }

        public void OnGet()
        {

        }

        public async Task OnGetAuthorizationCallback(string code)
        {
            var token = await Authorization.GetToken(code).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            return Redirect(Authorization.GetRedirectUriForCode("app-remote-control").AbsoluteUri);
        }
    }
}
