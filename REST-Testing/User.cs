using System.Text.Json.Serialization;

namespace REST_Testing
{
    public class User
    {

        [JsonPropertyName("age")]
        public int Age { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("sex")]
        public string Sex { get; set; }
        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; }

        public User(int age, string name, Enums.Sex sex, string zipCode)
        {
            this.Name = name;
            this.Age = age;
            this.Sex = sex.ToString();
            this.ZipCode = zipCode;
        }

        public User(string name, Enums.Sex sex)
        {
            this.Name = name;
            this.Sex = sex.ToString();
        }
    }
}
