using Newtonsoft.Json;

var barList = new List<FooClass>();
for (var i = 0; i < 20; i++)
{
    barList.Add(new FooClass { Index = i, Name = $"Name{i}" });
}

var jsonStr = JsonConvert.SerializeObject(barList);

var jsonStrRead = File.ReadAllText("/foobar.json");
var deserializedBarList = JsonConvert.DeserializeObject<List<FooClass>>(jsonStrRead);

class FooClass
{
    public int Index { get; set; }

    public string Name { get; set; }
}