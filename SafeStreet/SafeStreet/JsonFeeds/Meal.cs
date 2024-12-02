using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SafeStreet.Models
{
    public class Meal
    {
        [JsonProperty("idMeal")]
        public string IdMeal { get; set; }

        [JsonProperty("strMeal")]
        public string MealName { get; set; }

        [JsonProperty("strDrinkAlternate")]
        public string DrinkAlternate { get; set; }

        [JsonProperty("strCategory")]
        public string Category { get; set; }

        [JsonProperty("strArea")]
        public string Area { get; set; }

        [JsonProperty("strInstructions")]
        public string Instructions { get; set; }

        [JsonProperty("strMealThumb")]
        public string MealThumbnail { get; set; }

        [JsonProperty("strTags")]
        public string Tags { get; set; }

        [JsonProperty("strYoutube")]
        public string YoutubeLink { get; set; }

        [JsonProperty("strSource")]
        public string Source { get; set; }

        [JsonProperty("strImageSource")]
        public string ImageSource { get; set; }

        [JsonProperty("strCreativeCommonsConfirmed")]
        public string CreativeCommonsConfirmed { get; set; }

        [JsonProperty("dateModified")]
        public string DateModified { get; set; }

        [JsonProperty("strIngredient1")]
        public string Ingredient1 { get; set; }

        [JsonProperty("strIngredient2")]
        public string Ingredient2 { get; set; }

        [JsonProperty("strIngredient3")]
        public string Ingredient3 { get; set; }

        [JsonProperty("strIngredient4")]
        public string Ingredient4 { get; set; }

        [JsonProperty("strIngredient5")]
        public string Ingredient5 { get; set; }

        [JsonProperty("strIngredient6")]
        public string Ingredient6 { get; set; }

        [JsonProperty("strIngredient7")]
        public string Ingredient7 { get; set; }

        [JsonProperty("strIngredient8")]
        public string Ingredient8 { get; set; }

        [JsonProperty("strIngredient9")]
        public string Ingredient9 { get; set; }

        [JsonProperty("strIngredient10")]
        public string Ingredient10 { get; set; }

        [JsonProperty("strMeasure1")]
        public string Measure1 { get; set; }

        [JsonProperty("strMeasure2")]
        public string Measure2 { get; set; }

        [JsonProperty("strMeasure3")]
        public string Measure3 { get; set; }

        [JsonProperty("strMeasure4")]
        public string Measure4 { get; set; }

        [JsonProperty("strMeasure5")]
        public string Measure5 { get; set; }

        [JsonProperty("strMeasure6")]
        public string Measure6 { get; set; }

        [JsonProperty("strMeasure7")]
        public string Measure7 { get; set; }

        [JsonProperty("strMeasure8")]
        public string Measure8 { get; set; }

        [JsonProperty("strMeasure9")]
        public string Measure9 { get; set; }

        [JsonProperty("strMeasure10")]
        public string Measure10 { get; set; }

        // Method to deserialize a single Meal object from JSON
        internal static Meal FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<Meal>(jsonString);
        }
    }

    public class Meals
    {
        [JsonProperty("meals")]
        public List<Meal> MealList { get; set; }

        // Method to deserialize a Meals object from JSON
        public static Meals FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<Meals>(jsonString);
        }
    }
}