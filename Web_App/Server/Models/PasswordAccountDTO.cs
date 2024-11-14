namespace Shared.Models;

using System.Collections.Generic;
using CsvHelper.Configuration;
using HandyBlazorComponents.Interfaces;


public class PasswordAccountDTO : IHandyGridEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public string? Username { get; set; }
    public string Password { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public object? DisplayPropertyInGrid(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Id):
                return Id;
            case nameof(Title):
                return Title;
            case nameof(UserId):
                return UserId;
            case nameof(Username):
                return Username;
            case nameof(Password):
                return Password;
            case nameof(CreatedAt):
                return CreatedAt is null ? DateTime.Now.ToString("yyyy-MM-dd") : CreatedAt.GetValueOrDefault().ToString("yyyy-MM-dd");
            case nameof(LastUpdatedAt):
                return LastUpdatedAt is null ? DateTime.Now.ToString("yyyy-MM-dd") : LastUpdatedAt.GetValueOrDefault().ToString("yyyy-MM-dd");
            default:
                throw new Exception("Invalid property name");
        }
    }

    public int GetPrimaryKey()
    {
        return Id;
    }

    public void ParsePropertiesFromCSV(Dictionary<string, object> properties)
    {
        foreach (var property in properties)
        {
            // Use reflection to set the property if it exists and is writable
            var propInfo = this.GetType().GetProperty(property.Key);
            if (propInfo != null && propInfo.CanWrite)
            {
                var propType = propInfo.PropertyType;

                // Check if the property is a List<string> and the value is a delimited string
                if (typeof(IEnumerable<string>).IsAssignableFrom(propType))
                {
                    var stringValue = ((IEnumerable<string>) property.Value).First();
                    // Convert the delimited string to a List<string>
                    var listValue = stringValue.Split('+').ToList();
                    propInfo.SetValue(this, listValue);
                }
                else
                {
                    // Set the property directly if it's not a List<string> or delimited string
                    propInfo.SetValue(this, property.Value);
                }
            }
        }
    }

    public void SetPrimaryKey(int id)
    {
        Id = id;
    }

    public void SetProperties(Dictionary<string, object> properties)
    {
        foreach (var property in properties)
        {
            // Use reflection to set the property if it exists and is writable
            var propInfo = this.GetType().GetProperty(property.Key);
            if (propInfo != null && propInfo.CanWrite)
            {
                propInfo.SetValue(this, property.Value);
            }
        }
    }

    public override string ToString()
    {
        return $"{nameof(Id)}:{Id}, {nameof(Title)}:{Title}, {nameof(Username)}:{Username}, {nameof(Password)}:{Password}, {nameof(UserId)}:{UserId}";
    }
}


public class PasswordsMapper : ClassMap<PasswordAccountDTO>
{
    public PasswordsMapper()
    {
        Map(m => m.Title).Name("Title");
        Map(m => m.Username).Name("Username");
        Map(m => m.Password).Name("Password");
    }
}