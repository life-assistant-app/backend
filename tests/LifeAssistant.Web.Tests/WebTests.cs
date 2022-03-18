using Xunit;

namespace LifeAssistant.Web.Tests;

[Collection("Database Test")]
public class WebTests
{
    protected DataFactory dataFactory = new DataFactory();
}