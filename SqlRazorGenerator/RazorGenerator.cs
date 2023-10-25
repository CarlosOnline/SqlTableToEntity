﻿using RazorEngineCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SqlRazorGenerator;

public class RazorGenerator<T> where T : class, new()
{
    private IRazorEngineCompiledTemplate<TemplateModel<T>> template;

    public RazorGenerator(string templateFilePath)
    {
        this.template = CompileTemplate(templateFilePath);
    }

    private IRazorEngineCompiledTemplate<TemplateModel<T>> CompileTemplate(string templateFilePath)
    {
        var razorEngine = new RazorEngine();

        var templateText = File.ReadAllText(templateFilePath);
        return razorEngine.Compile<TemplateModel<T>>(templateText, builder =>
        {
            builder.AddAssemblyReferenceByName("System.CodeDom");
            builder.AddAssemblyReferenceByName("System.Collections");
            builder.AddAssemblyReferenceByName("System.ComponentModel.DataAnnotations");
            builder.AddAssemblyReferenceByName("System.Configuration");
            builder.AddAssemblyReferenceByName("System.Data");
            builder.AddAssemblyReferenceByName("System.IO");
            builder.AddAssemblyReferenceByName("System.Linq.Expressions");
            builder.AddAssemblyReferenceByName("System.Linq");
            builder.AddAssemblyReferenceByName("System.Reflection");
            builder.AddAssemblyReferenceByName("System.Security.Principal");
            builder.AddAssemblyReferenceByName("System.Threading.Tasks");
        });
    }

    public string Generate(T model)
    {
        return this.template.Run(instance => {
            instance.Model = model;
            instance.EnvironmentVariables = GetEnvironmentVariables();
        });
    }

    private Dictionary<string, string> GetEnvironmentVariables()
    {
        var results = new Dictionary<string, string>();

        foreach (DictionaryEntry pair in Environment.GetEnvironmentVariables())
        {
            if (pair.Key != null && pair.Value != null)
            {
                var key = pair.Key.ToString();
                var value = pair.Value.ToString();

                if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                {
                    results.Add(key, value);
                }
            }
        }

        return results;
    }
}