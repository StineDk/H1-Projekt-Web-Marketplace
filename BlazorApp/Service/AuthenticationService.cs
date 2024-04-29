using DomainModels;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Service
{
    // Denne klasse er ansvarlig for at håndtere, om brugeren er logget ind eller ej.
    public class AuthenticationService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly DatabaseService _databaseService;

        public AuthenticationService(IJSRuntime jsRuntime, DatabaseService databaseService)
        {
            _jsRuntime = jsRuntime;
            _databaseService = databaseService;
        }

        //Henter variablerne for at de kan bruges i den her fil
        public bool IsAuthenticated { get; private set; }
        public User CurrentUser { get; private set; }
        public string CurrentUserID { get; private set; }

        public async Task Login(User user)
        {
            //Sætter IsAuthenticated til sandt, fordi brugeren logger ind
            IsAuthenticated = true;
            //Gemmer brugerens oplysninger i CurrentUser
            CurrentUser = user;
            //Konverterer brugerens ID til en streng og gemmer det i CurrentUserID
            CurrentUserID = user.userID.ToString();
            //Her fortæller vi hvad variable CurrentUserId som ligger i databaseservice hvilken UserID brugeren har
            _databaseService.SetCurrentUserID(CurrentUserID); 
            //gemmer Brugerens status i Cokkies
            await SaveToLocalStorage();
        }

        //denne funktion kaldes Når brugeren logger ud
        public async Task Logout()
        {
            //Sætter IsAuthenticated til false, fordi brugeren logger ud
            IsAuthenticated = false;
            // Sletter oplysningerne om brugeren.
            CurrentUser = null;
            //sletter brugerns ID
            CurrentUserID = null;
            //sletter cookies
            await ClearLocalStorage();
            
        }

        //Den her funktion gemmer brugerens status og oplysninger i cookies
        private async Task SaveToLocalStorage()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "IsAuthenticated", IsAuthenticated.ToString());
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "CurrentUser", CurrentUser != null ? CurrentUser.name : "");
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "CurrentUserId", CurrentUserID); 
        }


        //Den her Funktion indlæser brugerens status og oplysninger fra Cookies
        public async Task LoadFromLocalStorage()
        {
            //indlæser fra cookie om brugeren er logget ind
            string isAuthenticatedString = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "IsAuthenticated");
            // hvis brugeren er logget ind
            if (!string.IsNullOrEmpty(isAuthenticatedString))
            {
                //sætter variablen til sandt
                IsAuthenticated = bool.Parse(isAuthenticatedString);
            }

            //indlæser fra Cookies CurrentUser
            string userName = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "CurrentUser");
            //hvis ikke username er tom
            if (!string.IsNullOrEmpty(userName))
            {
                //Opretter et objekt og gemmer brugernavnet
                CurrentUser = new User
                { name = userName };
            }
            //Indlæser brugerens id fra Cookies
            CurrentUserID = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "CurrentUserId");
        }

        //den her funktion sletter brugerens oplysninger fra Cookies
        private async Task ClearLocalStorage()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "IsAuthenticated");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "CurrentUser");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "CurrentUserId");
        }

        //den her funktion sletter Cookies når siden lukkes
        private async Task ClearLocalStorageOnPageExit()
        {
            await _jsRuntime.InvokeVoidAsync("eval", @"window.addEventListener('beforeunload'', 
                function() {localStorage.clear();});");
        }
    }
}
