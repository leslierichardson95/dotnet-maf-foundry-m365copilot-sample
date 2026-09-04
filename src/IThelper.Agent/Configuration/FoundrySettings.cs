namespace ITHelper;

public sealed record FoundrySettings(
    Uri ProjectEndpoint,
    string ModelDeploymentName)
{
    public static FoundrySettings FromEnvironment()
    {
        var endpoint = Environment.GetEnvironmentVariable("FOUNDRY_PROJECT_ENDPOINT")
            ?? Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT")
            ?? throw new InvalidOperationException(
                "Set FOUNDRY_PROJECT_ENDPOINT (or AZURE_AI_PROJECT_ENDPOINT) " +
                "to your Microsoft Foundry project endpoint.");

        var model = Environment.GetEnvironmentVariable("AZURE_AI_MODEL_DEPLOYMENT_NAME")
            ?? throw new InvalidOperationException(
                "Set AZURE_AI_MODEL_DEPLOYMENT_NAME to the model deployment name.");

        return new FoundrySettings(new Uri(endpoint), model);
    }
}
