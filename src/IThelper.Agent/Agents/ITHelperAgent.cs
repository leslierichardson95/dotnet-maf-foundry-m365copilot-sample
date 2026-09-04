using Azure.AI.Projects;
using ITHelper.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace ITHelper.Agents;

public static class ITHelperAgent
{
    public static AIAgent Create(
        AIProjectClient projectClient,
        string modelDeploymentName,
        HelpdeskTools helpdesk)
    {
        return projectClient.AsAIAgent(
            model: modelDeploymentName,
            name: "it-helper",
            description: "An internal IT helpdesk triage assistant.",
            instructions: """
                You are an internal IT helpdesk triage assistant.

                Your goals are to diagnose common IT problems, explain safe
                troubleshooting steps, and create or escalate tickets when needed.

                Rules:
                - Use SearchKnowledgeBase for company-specific setup or policy questions.
                - Use CheckServiceStatus before blaming a known service.
                - Use LookupUserTickets before creating a duplicate ticket.
                - Ask for confirmation before CreateTicket or EscalateToOnCall.
                - Reuse non-sensitive details already provided in the current
                  conversation, such as the user's email address. Do not ask
                  for the same detail again unless it is missing or ambiguous.
                - Do not claim to remember details across conversations.
                - Never claim that a ticket was created or escalated unless the tool
                  returned a successful result.
                - Cite the KB article title when SearchKnowledgeBase supplied the answer.
                - Do not request passwords, authentication codes, or other secrets.
                """,
            tools:
            [
                AIFunctionFactory.Create(
                    helpdesk.CheckServiceStatus,
                    "CheckServiceStatus",
                    "Checks the current status of an internal service."),
                AIFunctionFactory.Create(
                    helpdesk.LookupUserTickets,
                    "LookupUserTickets",
                    "Lists existing helpdesk tickets for a user email address."),
                AIFunctionFactory.Create(
                    helpdesk.CreateTicket,
                    "CreateTicket",
                    "Creates a new IT helpdesk ticket after the user confirms."),
                AIFunctionFactory.Create(
                    helpdesk.EscalateToOnCall,
                    "EscalateToOnCall",
                    "Escalates a high-impact incident after the user confirms."),
                AIFunctionFactory.Create(
                    helpdesk.SearchKnowledgeBase,
                    "SearchKnowledgeBase",
                    "Searches internal IT knowledge-base articles and returns citations.")
            ]);
    }
}
