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

        public Item LookupNutritionInfo(string searchString)
        {
            nutritionix.Initialize(nutritionixId, nutritionixKey);

            SearchRequest searchRequest = new SearchRequest();
            searchRequest.Query = searchString;

            var searchResult = new SearchResponse();
            searchResult =  nutritionix.SearchItems(searchRequest);

            var item = nutritionix.RetrieveItem(searchResult.Results[0].Item.Id);
            return item;
        }
    }
}
