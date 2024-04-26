using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace RoyalCode.SmartProblems.Descriptions;

/// <summary>
/// A class that contains all the information about a problem details.
/// </summary>
public partial class ProblemDetailsDescriptor
{
    private readonly Dictionary<string, ProblemDetailsDescription> descriptions = DescriptionFactory();
    private readonly Dictionary<ProblemCategory, ProblemDetailsDescription> genericErrorDescriptions = GenericProblemsDescriptionFactory();

    /// <summary>
    /// <para>
    ///     Obtains the default descriptions of the generic errors.
    /// </para>
    /// </summary>
    /// <param name="category">The category of the problem.</param>
    /// <returns>The default description of the generic error.</returns>
    public ProblemDetailsDescription GetDescriptionByCategory(ProblemCategory category)
    {
        return genericErrorDescriptions[category];
    }

    /// <summary>
    /// <para>
    ///     Try to get the descriptionsToAdd of a problem details by its code.
    /// </para>
    /// </summary>
    /// <param name="typeId">The type id of the problem details.</param>
    /// <param name="description">The descriptionsToAdd of the problem details.</param>
    /// <returns>True if the descriptionsToAdd was found, otherwise false.</returns>
    public bool TryGetDescription(string typeId, [NotNullWhen(true)] out ProblemDetailsDescription? description)
    {
        return descriptions.TryGetValue(typeId, out description);
    }

    /// <summary>
    /// <para>
    ///     Adds a new problem details description.
    /// </para>
    /// </summary>
    /// <param name="description">The description of the problem details.</param>
    /// <returns>Same instance of <see cref="ProblemDetailsDescriptor"/>.</returns>
    public ProblemDetailsDescriptor Add(ProblemDetailsDescription description)
    {
        descriptions.Add(description.TypeId, description);
        return this;
    }

    /// <summary>
    /// <para>
    ///     Adds many problem details descriptionsToAdd.
    /// </para>
    /// </summary>
    /// <param name="descriptions">A collection of descriptionsToAdd of problem details.</param>
    /// <returns>Same instance of <see cref="ProblemDetailsDescriptor"/>.</returns>
    public ProblemDetailsDescriptor AddRange(IEnumerable<ProblemDetailsDescription> descriptions)
    {
        foreach (var description in descriptions)
        {
            this.descriptions.Add(description.TypeId, description);
        }

        return this;
    }

    /// <summary>
    /// <para>
    ///     Adds many problem details descriptionsToAdd from a JSON string.
    /// </para>
    /// </summary>
    /// <param name="json">A JSON string with the descriptionsToAdd of problem details.</param>
    /// <returns>Same instance of <see cref="ProblemDetailsDescriptor"/>.</returns>
    /// <exception cref="ProblemDetailsDescriptorDeserializationException">
    ///     If the JSON string is invalid.
    /// </exception>
    public ProblemDetailsDescriptor AddFromJson(string json)
    {
        try
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            var descriptionsToAdd = JsonSerializer.Deserialize<IEnumerable<ProblemDetailsDescription>>(json, jsonOptions);
            if (descriptionsToAdd is not null)
            {
                AddRange(descriptionsToAdd);
            }
            return this;
        }
        catch(Exception ex)
        {
            throw new ProblemDetailsDescriptorDeserializationException(json, ex);
        }
    }

    /// <summary>
    /// <para>
    ///     Adds many problem details descriptionsToAdd from a JSON file.
    /// </para>
    /// </summary>
    /// <param name="path">The path of the JSON file with the descriptionsToAdd of problem details.</param>
    /// <returns>Same instance of <see cref="ProblemDetailsDescriptor"/>.</returns>
    /// <exception cref="ProblemDetailsDescriptorDeserializationException">
    ///     If the JSON file is invalid.
    /// </exception>
    public ProblemDetailsDescriptor AddFromJsonFile(string path)
    {
        try
        {
            var json = File.ReadAllText(path);
            return AddFromJson(json);
        }
        catch (ProblemDetailsDescriptorDeserializationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ProblemDetailsDescriptorDeserializationException(string.Empty, ex);
        }
    }
}
