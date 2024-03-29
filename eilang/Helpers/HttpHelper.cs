﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Helpers;

public static class HttpHelper
{
    private static readonly HttpClient HttpClient = new HttpClient(); // allow renewing the HttpClient inside a script

    public static ValueBase Get(State state, string url, string headers)
    {
        var parsedHeaders = ParseHeaders(headers);
        SetHeaders(parsedHeaders);
        var result = HttpClient.GetAsync(url).Result;
        var content = result.Content.ReadAsStringAsync().Result;
        return state.ValueFactory.String(content);
    }

    public static ValueBase Post(State state, string url, string headers, string postContent)
    {
        var parsedHeaders = ParseHeaders(headers);
        SetHeaders(parsedHeaders);
        using var json = new StringContent(postContent, Encoding.UTF8, "application/json");
        var result = HttpClient.PostAsync(url, json)
            .Result;
        var content = result.Content.ReadAsStringAsync().Result;
        return state.ValueFactory.String(content);
    }

    private static void SetHeaders(IEnumerable<Header> parsedHeaders)
    {
        HttpClient.DefaultRequestHeaders.Clear();
        foreach (var header in parsedHeaders)
        {
            HttpClient.DefaultRequestHeaders.Add(header.Name, header.Value);
        }
    }

    private static IEnumerable<Header> ParseHeaders(string headers)
    {
        var parsed = new List<Header>();
        var lines = headers.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var dividerIndex = line.IndexOf(':');
            if (dividerIndex < 0)
                throw new InvalidOperationException($"Unable to parse header line '{line}'.");
            parsed.Add(new Header(line.Substring(0, dividerIndex).Trim(), line.Substring(dividerIndex + 1).Trim()));
        }

        return parsed;
    }

    private class Header
    {
        public Header(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}
