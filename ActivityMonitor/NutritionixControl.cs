using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nutritionix;

namespace ActivityMonitor
{
    class NutritionixControl
    {
        private string nutritionixId = "3ab46b4a";
        private string nutritionixKey = "f32e934535e5f94711e3951b71980737	";

        private List<Item> foodItems = new List<Item>();
        private NutritionixClient nutritionix = new NutritionixClient();

        //metoda nawiązuje połączenie z web API, wysyła i obdiera zapytanie;
        //zwraca pierwszy z odebranych produktów
        public Item LookupNutritionInfo(string searchString)
        {
            //zainicjalizowanie połączenia z API kluczem i id
            nutritionix.Initialize(nutritionixId, nutritionixKey);

            //stworzenie obiektu reprezentującego pojedyncze zapytanie do wyszukania
            SearchRequest searchRequest = new SearchRequest();
            searchRequest.Query = searchString;

            //obiekt przechodujący odebraną z API odpowiedź
            var searchResult = new SearchResponse();
            searchResult =  nutritionix.SearchItems(searchRequest);

            //wydobycie z odpowiedzi pierwszego obiektu reprezentujacego pojedynczy produkt
            var item = nutritionix.RetrieveItem(searchResult.Results[0].Item.Id);
            return item;
        }
    }
}
