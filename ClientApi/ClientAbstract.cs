using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClientAbstract {

    protected string url;
    protected string method;
    public void SetUrl(string url, string method)
    {
        this.url = url;
        this.method = method;
    }

    public string DeleteInvalidCharsIntoJson(string json )
    {
        var charsToRemove = new string[] { "$" };
        foreach (var c in charsToRemove)
        {
            json = json.Replace(c, string.Empty);
        }

        return json;
    }

    protected abstract WWW GetRequest();
}
