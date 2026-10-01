using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Filters;

namespace Sentinela.Api.Attributes;

public class AgentKeyAttribute : TypeFilterAttribute
{
    public AgentKeyAttribute() : base(typeof(AgentKeyFilter))
    {
    }
}
