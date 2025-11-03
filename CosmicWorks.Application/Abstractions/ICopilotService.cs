namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// End-user friendly façade that groups catalog chat and discount commands behind one interface.
/// </summary>
public interface ICopilotService : ICatalogChat, IDiscountCommands, ICategoryQueries, ISystemPromptTemplate
{    
}