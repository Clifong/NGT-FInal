using System;
using UnityEngine;
using Newtonsoft.Json;

public abstract class GameDataSerializer<T> : JsonConverter<T> where T : GameData
{
    public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
    {
        value.Serialize(writer);
    }

    public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (hasExistingValue)
        {
            existingValue.Deserialize(reader);
        }
        return existingValue;
    }

}

public abstract class GameData
{
    protected GameDataSerializer<GameData> customSerializer;

    public GameDataSerializer<GameData> GetCustomSerializer()
    {
        return customSerializer;
    }

    public abstract void Serialize(JsonWriter writer);
    public abstract void Deserialize(JsonReader reader);
}
