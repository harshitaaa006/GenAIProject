using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SafeStreet.JsonFeeds
{
    public class NutrifyMe
    {
        [JsonPropertyName("meals")]
        public List<Meal> Meals { get; set; }
    }

    public class Meal
    {
        [JsonPropertyName("idMeal")]
        public string IdMeal { get; set; }

        [JsonPropertyName("strMeal")]
        public string StrMeal { get; set; }

        [JsonPropertyName("strDrinkAlternate")]
        public string StrDrinkAlternate { get; set; }

        [JsonPropertyName("strCategory")]
        public string StrCategory { get; set; }

        [JsonPropertyName("strArea")]
        public string StrArea { get; set; }

        [JsonPropertyName("strInstructions")]
        public string StrInstructions { get; set; }

        [JsonPropertyName("strMealThumb")]
        public string StrMealThumb { get; set; }

        [JsonPropertyName("strTags")]
        public string StrTags { get; set; }

        [JsonPropertyName("strYoutube")]
        public string StrYoutube { get; set; }

        [JsonPropertyName("strIngredient1")]
        public string StrIngredient1 { get; set; }

        [JsonPropertyName("strIngredient2")]
        public string StrIngredient2 { get; set; }

        [JsonPropertyName("strIngredient3")]
        public string StrIngredient3 { get; set; }

        [JsonPropertyName("strIngredient4")]
        public string StrIngredient4 { get; set; }

        [JsonPropertyName("strIngredient5")]
        public string StrIngredient5 { get; set; }

        [JsonPropertyName("strIngredient6")]
        public string StrIngredient6 { get; set; }

        [JsonPropertyName("strIngredient7")]
        public string StrIngredient7 { get; set; }

        [JsonPropertyName("strIngredient8")]
        public string StrIngredient8 { get; set; }

        [JsonPropertyName("strIngredient9")]
        public string StrIngredient9 { get; set; }

        [JsonPropertyName("strIngredient10")]
        public string StrIngredient10 { get; set; }

        [JsonPropertyName("strMeasure1")]
        public string StrMeasure1 { get; set; }

        [JsonPropertyName("strMeasure2")]
        public string StrMeasure2 { get; set; }

        [JsonPropertyName("strMeasure3")]
        public string StrMeasure3 { get; set; }

        [JsonPropertyName("strMeasure4")]
        public string StrMeasure4 { get; set; }

        [JsonPropertyName("strMeasure5")]
        public string StrMeasure5 { get; set; }

        [JsonPropertyName("strMeasure6")]
        public string StrMeasure6 { get; set; }

        [JsonPropertyName("strMeasure7")]
        public string StrMeasure7 { get; set; }

        [JsonPropertyName("strMeasure8")]
        public string StrMeasure8 { get; set; }

        [JsonPropertyName("strSource")]
        public string StrSource { get; set; }

        [JsonPropertyName("strImageSource")]
        public string StrImageSource { get; set; }

        [JsonPropertyName("strCreativeCommonsConfirmed")]
        public string StrCreativeCommonsConfirmed { get; set; }

        [JsonPropertyName("dateModified")]
        public string DateModified { get; set; }
    }
}
